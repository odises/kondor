using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using Kondor.Data;
using Kondor.Data.ApiModels;
using Kondor.Data.DataModel;
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
        private readonly string _baseUri;

        public TelegramMessageHandler(string apiKey, string directory, string cipherKey, string baseUri)
        {
            _userActiveCard = new List<Tuple<int, Card>>();
            _leitnerService = new LeitnerService(20, 15);
            _apiKey = apiKey;
            _directory = directory;
            _cipherKey = cipherKey;
            _baseUri = baseUri;
        }

        protected string GenerateKeyboardMarkup(params string[] input)
        {
            var result = new ReplyMarkupModel();
            result.keyboard = new List<List<Keyboard>>();


            foreach (var s in input)
            {
                var keys = new List<Keyboard>();
                keys.Add(new Keyboard { text = s });
                result.keyboard.Add(keys);
            }

            return JsonConvert.SerializeObject(result);
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
        public string GetRegistrationLink(int telegramUserId, string telegramUsername, string baseUri)
        {
            var encrypted = StringCipher.Encrypt($"{telegramUserId}:{telegramUsername}", _cipherKey);
            var base64Encoded = encrypted.GetBase64Encode();
            return $"{baseUri}/{base64Encoded}";
        }
        public int GetMessages()
        {
            var count = 0;

            using (var entities = new EntityContext())
            {
                var lastUpdateId = 0;
                var firstOrDefault = entities.Messages.OrderByDescending(p => p.UpdateId).FirstOrDefault();
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
                    count = responseModel.result.Count;
                    foreach (var item in responseModel.result)
                    {
                        if (!entities.Messages.Any(p => p.UpdateId == item.update_id))
                        {
                            entities.Messages.Add(new Message
                            {
                                UpdateId = item.update_id,
                                Status = 1,
                                SerializedResult = JsonConvert.SerializeObject(item.message),
                                MessageText = item.message.text ?? "empty",
                                ChatId = item.message.chat.id,
                                ChatType = item.message.chat.type,
                                UserId = item.message.from.id,
                                Username = item.message.@from.username ?? ""
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
        public string GenerateExamHtml(Card card)
        {
            return GenerateMemHtml(card.Mem);
        }
        public string GenerateMemHtml(Mem mem)
        {
            var result = "<b>This is just a test</b><i>Believe me</i>";
            return result;
        }
        public int ProcessMessages()
        {
            using (var entities = new EntityContext())
            {
                var unprocessed = entities.Messages.Where(p => p.Status == 1).ToList();

                foreach (var message in unprocessed)
                {
                    if (message.ChatType == "group")
                    {
                    }
                    else
                    {
                        try
                        {
                            if (message.MessageText == "New")
                            {
                                try
                                {
                                    var newVocab = _leitnerService.GetNewMem(message.UserId);
                                    var response = GenerateMemHtml(newVocab);
                                    SendMessage(message.ChatId, response, GenerateKeyboardMarkup("New", "Exam", "Status"));
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    SendMessage(message.UserId, "There is no new Mem.", GenerateKeyboardMarkup("Status"));
                                }
                                catch (ValidationException)
                                {
                                    SendMessage(message.UserId, "UserId is not valid.", GenerateKeyboardMarkup("Status"));
                                }
                                catch (OverflowException)
                                {
                                    SendMessage(message.UserId, "Maximum card in first position exceeded.", GenerateKeyboardMarkup("Exam", "Status"));
                                }
                            }
                            else if (message.MessageText == "Exam")
                            {
                                Card card;
                                var userActiveCard = _userActiveCard.FirstOrDefault(p => p.Item1 == message.UserId);
                                if (userActiveCard != null)
                                {
                                    card = userActiveCard.Item2;
                                }
                                else
                                {
                                    try
                                    {
                                        card = _leitnerService.GetCardForExam(message.UserId);
                                        _userActiveCard.Add(new Tuple<int, Card>(message.UserId, card));
                                    }
                                    catch (IndexOutOfRangeException)
                                    {
                                        SendMessage(message.ChatId, "There is no card for exam yet", GenerateKeyboardMarkup("New", "Status"));
                                        message.Status = 0;
                                        continue;
                                    }
                                }

                                SendMessage(message.ChatId, GenerateExamHtml(card), GenerateKeyboardMarkup("Yes", "No"));
                            }
                            else if (message.MessageText == "Register")
                            {
                                var registrationLink = GetRegistrationLink(message.UserId, message.Username, _baseUri);
                                SendMessage(message.ChatId, $"[Registration Link]({registrationLink})", TelegramHelper.GenerateReplyKeyboardMarkup(new KeyboardButton[,] { { new KeyboardButton() { Text = "Hi" }, new KeyboardButton() { Text = "Phone", RequestContact = true }, }, { new KeyboardButton() { Text = "Bye" }, new KeyboardButton() { Text = "Test" } } }, false, false, false));
                            }
                            else
                            {
                                SendMessage(message.ChatId, "Test",
                                    TelegramHelper
                                    .GetInlineKeyboardMarkup(new[,]
                                    {
                                        {
                                            new InlineKeyboardButton{ Text = "inline test", CallbackData = "salam"}
                                        }
                                    }));
                            }
                        }
                        catch (Exception exception)
                        {
                            using (var writer = new StreamWriter(_directory + "/" + DateTime.Now.ToString("yy-MM-dd") + ".txt", true))
                            {
                                writer.WriteLine(exception.ToString());
                            }
                        }
                    }
                    message.Status = 0;
                }
                entities.SaveChanges();
            }


            return 0;

        }
    }
}
