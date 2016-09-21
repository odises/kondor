using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Leitner;
using Newtonsoft.Json;

namespace Kondor.Service
{
    public class TelegramMessageHandler
    {
        private readonly LeitnerService _leitnerService;
        private readonly string _directory;
        private readonly string _cipherKey;
        private readonly string _registrationBaseUri;
        private readonly UserApi _userApi;
        private readonly TelegramApiManager _telegramApiManager;
        private readonly List<Tuple<int, Card>> _userActiveCard;


        public TelegramMessageHandler(string directory, string cipherKey, string registrationBaseUri, UserApi userApi, LeitnerService leitnerService, TelegramApiManager telegramApiManager)
        {
            _userActiveCard = new List<Tuple<int, Card>>();
            _directory = directory;
            _cipherKey = cipherKey;
            _registrationBaseUri = registrationBaseUri;
            _userApi = userApi;
            _leitnerService = leitnerService;
            _telegramApiManager = telegramApiManager;
            _telegramApiManager.MessageSent += _telegramApiManager_MessageSent;
        }

        private void _telegramApiManager_MessageSent(object sender, MessageSentEventArgs e)
        {
            using (var entites = new EntityContext())
            {
                entites.Responses.Add(new Response
                {
                    ChatId = e.ChatId,
                    MessageId = e.MessageId,
                    Status = ResponseStatus.New
                });

                entites.SaveChanges();
            }
        }

        public void SaveUpdates()
        {
            using (var entities = new EntityContext())
            {
                int? lastUpdateId = null;
                var lastUpdate = entities.Updates.OrderByDescending(p => p.UpdateId).FirstOrDefault();
                if (lastUpdate != null)
                {
                    lastUpdateId = lastUpdate.UpdateId;
                }

                var updates = _telegramApiManager.GetUpdates(lastUpdateId);

                foreach (var update in updates)
                {
                    UpdateType updateType;

                    if (update.Message != null)
                    {
                        updateType = UpdateType.Message;
                    }
                    else if (update.EditedMessage != null)
                    {
                        updateType = UpdateType.EditedMessage;
                    }
                    else if (update.InlineQuery != null)
                    {
                        updateType = UpdateType.InlineQuery;
                    }
                    else if (update.ChosenInlineResult != null)
                    {
                        updateType = UpdateType.ChosenInlineResult;
                    }
                    else if (update.CallbackQuery != null)
                    {
                        updateType = UpdateType.CallbackQuery;
                    }
                    else
                    {
                        updateType = UpdateType.Unclear;
                    }

                    if (!entities.Updates.Any(p => p.UpdateId == update.UpdateId))
                    {
                        entities.Updates.Add(new Data.DataModel.Update
                        {
                            UpdateId = update.UpdateId,
                            Status = UpdateStatus.Unprocessed,
                            CreationDatetime = DateTime.Now,
                            ModifiedDatetime = DateTime.Now,
                            UpdateType = updateType,
                            SerializedUpdate = update.ToJson()
                        });
                    }
                }

                entities.SaveChanges();
            }
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
                        // log error
                        Console.WriteLine(exception.Message);
                    }
                    update.Status = UpdateStatus.Processed;
                }
                entities.SaveChanges();
            }


            return 0;

        }

        private void CallbackQueryProcessor(CallbackQuery callbackQuery)
        {
            var queryProcessor = new QueryProcessor(_userApi, _telegramApiManager, _leitnerService, _userActiveCard);
            queryProcessor.Process(callbackQuery);
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
            if (message.Text == "/start")
            {
                var welcomeMessage = _telegramApiManager.SendMessage(message.Chat.Id, "Welcome *message* from config");
                using (var entities = new EntityContext())
                {
                    var user = entities.Users.FirstOrDefault(p => p.TelegramUserId == message.From.Id);
                    if (user != null)
                    {
                        user.WelcomeMessageId = int.Parse(welcomeMessage.MessageId);
                        entities.SaveChanges();
                    }
                }
            }

            if (!_userApi.IsRegisteredUser(message.From.Id))
            {
                // send registration link
                _telegramApiManager.SendMessage(message.Chat.Id, "Register",
                    TelegramHelper.GetInlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "Enter",
                                CallbackData = QueryData.NewQueryString("Enter", null, null)
                            },
                            new InlineKeyboardButton
                            {
                                Text = "Register",
                                Url = _userApi.GetRegistrationLink(message.From.Id, message.From.Username, _registrationBaseUri, _cipherKey)
                            }
                        }
                    }));
            }
            else
            {
                _telegramApiManager.SendMessage(message.Chat.Id,
                    "*Example Board*\n\nIn this board you can see a lot of example by tapping on Refresh key.",
                    TelegramHelper.GetInlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "Refresh",
                                CallbackData = QueryData.NewQueryString("ExampleBoardRefresh", null, null)
                            }
                        }
                    }));

                _telegramApiManager.SendMessage(message.Chat.Id, "You are already registered in our system. Please Enter.",
                    TelegramHelper.GetInlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton()
                            {
                                Text = "Enter",
                                CallbackData = QueryData.NewQueryString("Enter", null, null)
                            }
                        }
                    }));
            }
        }
    }
}
