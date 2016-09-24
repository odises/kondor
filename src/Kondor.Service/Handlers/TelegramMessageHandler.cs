﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Leitner;
using Kondor.Service.Managers;
using Kondor.Service.Processors;
using Newtonsoft.Json;

namespace Kondor.Service.Handlers
{
    public class TelegramMessageHandler : ITelegramMessageHandler
    {
        private readonly ILeitnerService _leitnerService;
        private readonly string _cipherKey;
        private readonly string _registrationBaseUri;
        private readonly IUserApi _userApi;
        private readonly ITelegramApiManager _telegramApiManager;
        private readonly IList<Tuple<int, Card>> _userActiveCard;
        private readonly IDbContext _context;


        public TelegramMessageHandler(string cipherKey, string registrationBaseUri, IUserApi userApi, ILeitnerService leitnerService, ITelegramApiManager telegramApiManager, IList<Tuple<int, Card>> userActiveCard, IDbContext context)
        {
            _cipherKey = cipherKey;
            _registrationBaseUri = registrationBaseUri;
            _userApi = userApi;
            _leitnerService = leitnerService;
            _telegramApiManager = telegramApiManager;
            _userActiveCard = userActiveCard;
            this._context = context;
        }

        public void SaveUpdates()
        {

            int? lastUpdateId = null;
            var lastUpdate = _context.Updates.OrderByDescending(p => p.UpdateId).FirstOrDefault();
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

                if (!_context.Updates.Any(p => p.UpdateId == update.UpdateId))
                {
                    _context.Updates.Add(new Data.DataModel.Update
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

            _context.SaveChanges();

        }

        public int ProcessMessages()
        {

            var unprocessed = _context.Updates.Where(p => p.Status == UpdateStatus.Unprocessed).ToList();

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
            _context.SaveChanges();

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
                
                    var user = _context.Set<ApplicationUser>().FirstOrDefault(p => p.TelegramUserId == message.From.Id);
                    if (user != null)
                    {
                        user.WelcomeMessageId = int.Parse(welcomeMessage.MessageId);
                        _context.SaveChanges();
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
