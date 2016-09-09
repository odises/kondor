using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using Kondor.Data;
using Kondor.Data.ApiModels;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Extensions;
using Kondor.Service.Leitner;
using Newtonsoft.Json;

namespace Kondor.Service
{
    public class TelegramMessageHandler
    {
        private readonly LeitnerService _leitnerService;
        private readonly string _apiKey;
        private readonly string _directory;
        private readonly List<Tuple<int, Card>> _userActiveCard;
        private readonly string _cipherKey;
        private readonly string _registrationBaseUri;
        private readonly UserApi _userApi;

        public TelegramMessageHandler(string apiKey, string directory, string cipherKey, string registrationBaseUri, UserApi userApi, LeitnerService leitnerService)
        {
            _userActiveCard = new List<Tuple<int, Card>>();
            _apiKey = apiKey;
            _directory = directory;
            _cipherKey = cipherKey;
            _registrationBaseUri = registrationBaseUri;
            _userApi = userApi;
            _leitnerService = leitnerService;
        }

        protected void SendMessage(int chatId, string text, string replyMarkup = null)
        {
            if (string.IsNullOrEmpty(replyMarkup))
            {
                var webClient = new WebClient();
                var response = webClient.DownloadString(
                    $"https://api.telegram.org/{_apiKey}/sendMessage?chat_id={chatId}&text={text}&parse_mode=Markdown");
            }
            else
            {
                var webClient = new WebClient();
                var response = webClient.DownloadString(
                    $"https://api.telegram.org/{_apiKey}/sendMessage?chat_id={chatId}&text={text}&parse_mode=Markdown&reply_markup={replyMarkup}");
            }
        }

        public int GetMessages()
        {
            var count = 0;

            using (var entities = new EntityContext())
            {
                var lastUpdateId = 0;
                var firstOrDefault = entities.Updates.OrderByDescending(p => p.UpdateId).FirstOrDefault();
                if (firstOrDefault != null)
                {
                    lastUpdateId = firstOrDefault.UpdateId;
                }

                var webClient = new WebClient();

                string response;

                if (lastUpdateId > 0)
                {
                    response = webClient.DownloadString(
                        $"https://api.telegram.org/{_apiKey}/getupdates?offset={lastUpdateId}");
                }
                else
                {
                    response = webClient.DownloadString(
                        $"https://api.telegram.org/{_apiKey}/getupdates");
                }

                try
                {
                    var responseModel = JsonConvert.DeserializeObject<GetUpdatesResult>(response);
                    count = responseModel.Result.Count;
                    foreach (var item in responseModel.Result)
                    {
                        UpdateType updateType;

                        if (item.Message != null)
                        {
                            updateType = UpdateType.Message;
                        }
                        else if (item.EditedMessage != null)
                        {
                            updateType = UpdateType.EditedMessage;
                        }
                        else if (item.InlineQuery != null)
                        {
                            updateType = UpdateType.InlineQuery;
                        }
                        else if (item.ChosenInlineResult != null)
                        {
                            updateType = UpdateType.ChosenInlineResult;
                        }
                        else if (item.CallbackQuery != null)
                        {
                            updateType = UpdateType.CallbackQuery;
                        }
                        else
                        {
                            updateType = UpdateType.Unclear;
                        }

                        if (!entities.Updates.Any(p => p.UpdateId == item.UpdateId))
                        {
                            entities.Updates.Add(new Data.DataModel.Update
                            {
                                UpdateId = item.UpdateId,
                                Status = UpdateStatus.Unprocessed,
                                CreationDatetime = DateTime.Now,
                                ModifiedDatetime = DateTime.Now,
                                UpdateType = updateType,
                                SerializedUpdate = JsonConvert.SerializeObject(item, Formatting.None, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                })
                            });
                        }
                    }
                }
                catch (JsonReaderException exception)
                {
                    Console.WriteLine("invalid json format");
                }

                entities.SaveChanges();

                return count;
            }
        }
        public string GenerateExamMarkdown(Card card)
        {
            return GenerateMemMarkdown(card.Mem);
        }
        public string GenerateMemMarkdown(Mem mem)
        {
            var result = $"*{mem.MemBody}*\n_{mem.MemBody}_";
            return result;
        }
        public int ProcessMessages()
        {
            using (var entities = new EntityContext())
            {
                var unprocessed = entities.Updates.Where(p => p.Status == UpdateStatus.Unprocessed).ToList();

                foreach (var update in unprocessed)
                {
                    var model = JsonConvert.DeserializeObject<Data.TelegramTypes.Update>(update.SerializedUpdate);
                    try
                    {
                        if (model.Message != null)
                        {
                            MessageProcessor(model.Message);
                        }
                        else if (model.EditedMessage != null)
                        {
                            EditedMessageProcessor(model.Message);
                        }
                        else if (model.InlineQuery != null)
                        {
                            InlineQueryProcessor(model.InlineQuery);
                        }
                        else if (model.ChosenInlineResult != null)
                        {
                            ChosenInlineResultProcessor(model.ChosenInlineResult);
                        }
                        else if (model.CallbackQuery != null)
                        {
                            CallbackQueryProcessor(model.CallbackQuery);
                        }
                        else
                        {
                            throw new InvalidDataException();
                        }
                    }
                    catch (Exception exception)
                    {

                    }
                    update.Status = UpdateStatus.Processed;
                }
                entities.SaveChanges();
            }


            return 0;

        }

        private void CallbackQueryProcessor(CallbackQuery callbackQuery)
        {
            var query = callbackQuery.Data.Split(new[] {':'}, StringSplitOptions.None);

            var telegramApiManager = new TelegramApiManager(_apiKey);

            if (query[0] == "Enter")
            {
                if (_userApi.IsRegisteredUser(callbackQuery.From.Id))
                {
                    // todo: check if user has entered once

                    SendMessage(callbackQuery.Message.Chat.Id, "What do you want to do?",
                        TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                }
                else
                {
                    telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "Your are not registered yet.", true);
                }
            }
            else if (query[0] == "Display")
            {
                var ticks = query[1];
                var cardId = query[2];
                var datetime = new DateTime(long.Parse(ticks));
                if (datetime < DateTime.Now.AddSeconds(-30))
                {
                    telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "This thread is expired.", false);
                    //SendMessage(callbackQuery.Message.Chat.Id, "This thread is expired.", TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                }
                else
                {
                    var card = _leitnerService.GetCard(int.Parse(cardId));
                    SendMessage(callbackQuery.Message.Chat.Id, GenerateMemMarkdown(card.Mem), TelegramHelper.GetInlineKeyboardMarkup(new []
                    {
                      new []
                      {
                          new InlineKeyboardButton {Text = "Accept", CallbackData = $"Exam:{card.Id}:Accept:{datetime.Ticks}"},
                          new InlineKeyboardButton {Text = "Reject", CallbackData = $"Exam:{card.Id}:Reject:{datetime.Ticks}"}  
                      }  
                    }));
                }
            }
            else if (query[0] == "Exam")
            {
                var ticks = query[3];
                var datetime = new DateTime(long.Parse(ticks));
                if (datetime < DateTime.Now.AddSeconds(-30))
                {
                    telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "This thread is expired.", false);
                }
                else
                {
                    var cardId = int.Parse(query[1]);
                    var card = _leitnerService.GetCard(cardId);
                    if (query[2] == "Accept")
                    {
                        _leitnerService.MoveNext(card);
                        SendMessage(callbackQuery.Message.Chat.Id, "The card moved one step forward.", TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                    }
                    else
                    {
                        _leitnerService.MoveBack(card);
                        SendMessage(callbackQuery.Message.Chat.Id, "The card moved one step backward.", TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                    }
                }
            }
        }

        private void ChosenInlineResultProcessor(ChosenInlineResult chosenInlineResult)
        {
            throw new NotImplementedException();
        }

        private void InlineQueryProcessor(InlineQuery inlineQuery)
        {
            throw new NotImplementedException();
        }

        private void EditedMessageProcessor(Message message)
        {
            throw new NotImplementedException();
        }

        public void MessageProcessor(Message message)
        {
            if (!_userApi.IsRegisteredUser(message.From.Id))
            {
                // send introduction message

                // send registration link
                SendMessage(message.Chat.Id, "Register",
                    TelegramHelper.GetInlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton()
                            {
                                Text = "Enter",
                                CallbackData = "Enter"
                            },
                            new InlineKeyboardButton()
                            {
                                Text = "Register",
                                Url = _userApi.GetRegistrationLink(message.From.Id, message.From.Username, _registrationBaseUri, _cipherKey)
                            }
                        }
                    }));
            }
            else
            {
                switch (message.Text)
                {
                    case "Learn":
                        try
                        {
                            var newVocab = _leitnerService.GetNewMem(message.From.Id);
                            var response = GenerateMemMarkdown(newVocab);
                            SendMessage(message.Chat.Id, response, TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            SendMessage(message.Chat.Id, "There is no new Mem.", TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                        }
                        catch (ValidationException)
                        {
                            SendMessage(message.Chat.Id, "UserId is not valid.", TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                        }
                        catch (OverflowException)
                        {
                            SendMessage(message.Chat.Id, "Maximum card in first position exceeded.", TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                        }
                        break;
                    case "Exam":
                        Card card;
                        var userActiveCard = _userActiveCard.FirstOrDefault(p => p.Item1 == message.From.Id);
                        if (userActiveCard != null)
                        {
                            card = userActiveCard.Item2;
                        }
                        else
                        {
                            try
                            {
                                card = _leitnerService.GetCardForExam(message.From.Id);
                                _userActiveCard.Add(new Tuple<int, Card>(message.From.Id, card));
                            }
                            catch (IndexOutOfRangeException)
                            {
                                SendMessage(message.Chat.Id, "There is no card for exam yet", TelegramHelper.GenerateReplyKeyboardMarkup("Learn", "Exam"));
                                break;
                            }
                        }
                        SendMessage(message.Chat.Id, $"*{card.Mem.MemBody}*", TelegramHelper.GetInlineKeyboardMarkup(new[]
                        {
                            new [] {new InlineKeyboardButton
                            {
                                Text = "Display Definition",
                                CallbackData = $"Display:{DateTime.Now.Ticks}:{card.Id}"
                            }}
                        }));
                        break;
                    case "/start":
                        break;
                }
            }
        }
    }
}
