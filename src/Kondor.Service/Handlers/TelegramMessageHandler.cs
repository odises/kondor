using System;
using System.IO;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.SettingModels;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Managers;
using Kondor.Service.Processors;
using Newtonsoft.Json;

namespace Kondor.Service.Handlers
{
    public class TelegramMessageHandler : ITelegramMessageHandler
    {
        private readonly IUserApi _userApi;
        private readonly ITelegramApiManager _telegramApiManager;
        private readonly IDbContext _context;
        private readonly ISettingHandler _settingHandler;
        private readonly ITextManager _textManager;


        public TelegramMessageHandler(IUserApi userApi, ITelegramApiManager telegramApiManager, IDbContext context, ISettingHandler settingHandler, ITextManager textManager)
        {
            _userApi = userApi;
            _telegramApiManager = telegramApiManager;
            this._context = context;
            _settingHandler = settingHandler;
            _textManager = textManager;
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
                var fromId = -1;

                if (update.Message != null)
                {
                    updateType = UpdateType.Message;
                    fromId = update.Message.From.Id;
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
                    fromId = update.CallbackQuery.From.Id;
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
                        FromId = fromId,
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
            var queryProcessor = ObjectManager.GetInstance<IQueryProcessor>();
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
                var welcomeMessage = _telegramApiManager.SendMessage(message.Chat.Id, _textManager.GetText(StringResources.WelcomeMessage));
                
                    var user = _context.Set<ApplicationUser>().FirstOrDefault(p => p.TelegramUserId == message.From.Id);
                    if (user != null)
                    {
                        user.WelcomeMessageId = int.Parse(welcomeMessage.MessageId);
                        _context.SaveChanges();
                    }
                
            }

            if (!_userApi.IsRegisteredUser(message.From.Id))
            {
                var registrationBaseUri = _settingHandler.GetSettings<GeneralSettings>().RegistrationBaseUri;
                var cipherKey = _settingHandler.GetSettings<GeneralSettings>().CipherKey;

                // send registration link
                _telegramApiManager.SendMessage(message.Chat.Id, _textManager.GetText(StringResources.RegistrationMessage),
                    TelegramHelper.GetInlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = _textManager.GetText(StringResources.KeyboardEnterTitle),
                                CallbackData = QueryData.NewQueryString("Enter", null, null)
                            },
                            new InlineKeyboardButton
                            {
                                Text = _textManager.GetText(StringResources.KeyboardRegistrationTitle),
                                Url = _userApi.GetRegistrationLink(message.From.Id, message.From.Username, registrationBaseUri, cipherKey)
                            }
                        }
                    }));
            }
            else
            {
                _telegramApiManager.SendMessage(message.Chat.Id,
                    _textManager.GetText(StringResources.ExampleBoardMessage),
                    TelegramHelper.GetInlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = _textManager.GetText(StringResources.ExampleBoardRefreshKeyboardTitle),
                                CallbackData = QueryData.NewQueryString("ExampleBoardRefresh", null, null)
                            }
                        }
                    }));

                _telegramApiManager.SendMessage(message.Chat.Id, _textManager.GetText(StringResources.AlreadyRegistered),
                    TelegramHelper.GetInlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = _textManager.GetText(StringResources.KeyboardEnterTitle),
                                CallbackData = QueryData.NewQueryString("Enter", null, null)
                            }
                        }
                    }));
            }
        }
    }
}
