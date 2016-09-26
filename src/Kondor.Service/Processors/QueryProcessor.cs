using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Kondor.Data.Enums;
using Kondor.Data.SettingModels;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Handlers;
using Kondor.Service.Leitner;
using Kondor.Service.Managers;

namespace Kondor.Service.Processors
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IUserApi _userApi;
        private readonly ITelegramApiManager _telegramApiManager;
        private readonly ILeitnerService _leitnerService;
        private readonly ISettingHandler _settingHandler;

        public QueryProcessor(IUserApi userApi, ITelegramApiManager telegramApiManager, ILeitnerService leitnerService, ISettingHandler settingHandler)
        {
            _userApi = userApi;
            _telegramApiManager = telegramApiManager;
            _leitnerService = leitnerService;
            _settingHandler = settingHandler;
        }

        public void Process(CallbackQuery callbackQuery)
        {
            var queryData = QueryData.Parse(callbackQuery.Data);

            switch (queryData.Command)
            {
                case "Enter":
                    ProcessEnterCommand(queryData, callbackQuery);
                    break;
                case "Learn":
                    ProcessLearnCommand(queryData, callbackQuery);
                    break;
                case "Exam":
                    ProcessExamCommand(queryData, callbackQuery);
                    break;
                case "Display":
                    ProcessDisplayCommand(queryData, callbackQuery);
                    break;
                case "Answer":
                    ProcessAnswerCommand(queryData, callbackQuery);
                    break;
                case "Back":
                    ProcessBackCommand(callbackQuery);
                    break;
                case "ExampleBoardRefresh":
                    ProcessExampleBoardRefreshCommand(queryData, callbackQuery);
                    break;
            }
        }

        protected virtual void ProcessExampleBoardRefreshCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            try
            {
                var example = _leitnerService.GetExample(callbackQuery.From.Id);
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId),
                    $"*Example Board*\n\n{example.ToBoldedString()}", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[]
                {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "Refresh",
                                CallbackData = QueryData.NewQueryString("ExampleBoardRefresh", null, null)
                            },
                            new InlineKeyboardButton
                            {
                                Text = "Speak",
                                Url = string.Format(_settingHandler.GetSettings<GeneralSettings>().SpeakBaseUri, example.Id, TextType.Example)
                            }
                        }
                }));
            }
            catch (IndexOutOfRangeException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "There is not example yet.", true);
            }
        }

        protected virtual void ProcessBackCommand(CallbackQuery callbackQuery)
        {
            _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), "What do you want to do?", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null)}
                    }}));
        }

        protected virtual void ProcessAnswerCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            var cardId = int.Parse(queryData.Data);

            if (queryData.Action == "Reject")
            {
                _leitnerService.MoveBack(cardId);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "Moved back", true);
            }
            else if (queryData.Action == "Accept")
            {
                _leitnerService.MoveNext(cardId);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "Moved next", true);
            }
            else
            {
                throw new InvalidDataException();
            }

            _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), "What do you want to do?", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null)}
                    }}));
        }

        protected virtual void ProcessEnterCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            if (_userApi.IsRegisteredUser(callbackQuery.From.Id))
            {
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), "What do you want to do?", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null)}
                    }}));
            }
            else
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "Your are not registered yet.", true);
            }
        }
        protected virtual void ProcessLearnCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            try
            {
                var newMem = _leitnerService.GetNewMem(callbackQuery.From.Id);
                var response = newMem.GenerateCardView();
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), response, "Markdown", false, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Images", Url = string.Format(_settingHandler.GetSettings<GeneralSettings>().ImagesBaseUri, newMem.Id)},
                        new InlineKeyboardButton {Text = "Back", CallbackData = QueryData.NewQueryString("Back", null, null)}
                    }}));
            }
            catch (IndexOutOfRangeException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "There is no new Mem.", true);
            }
            catch (ValidationException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "UserId is not valid.", true);
            }
            catch (OverflowException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "Maximum card in first position exceeded.", true);
            }
        }
        protected virtual void ProcessExamCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            try
            {
                var card = _leitnerService.GetCardForExam(callbackQuery.From.Id);
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), $"{card.Mem.MemBody}", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
            {
                new InlineKeyboardButton {Text = "Images", Url = string.Format(_settingHandler.GetSettings<GeneralSettings>().ImagesBaseUri, card.Mem.Id)},
                new InlineKeyboardButton
                {
                    Text = "Display",
                    CallbackData = QueryData.NewQueryString("Display", null, card.Id.ToString())
                },
                new InlineKeyboardButton
                {
                    Text = "Back",
                    CallbackData = QueryData.NewQueryString("Back", null, null)
                }
            }}));

            }
            catch (IndexOutOfRangeException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "There is no card for exam yet.", true);
                return;
            }
            catch (ValidationException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "UserId is not valid.", true);
                return;
            }

        }
        protected virtual void ProcessDisplayCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            var datetime = new DateTime(queryData.Ticks);
            if (datetime < DateTime.Now.AddSeconds(-30))
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "This thread is expired.", true);
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), "What do you want to do?", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null)}
                    }}));
            }
            else
            {
                var card = _leitnerService.GetCard(int.Parse(queryData.Data));
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), card.Mem.GenerateCardView(), "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[]
                {
                      new []
                      {
                          new InlineKeyboardButton {Text = "Accept", CallbackData = QueryData.NewQueryString("Answer", "Accept", card.Id.ToString())},
                          new InlineKeyboardButton {Text = "Reject", CallbackData = QueryData.NewQueryString("Answer", "Reject", card.Id.ToString())},
                          new InlineKeyboardButton {Text = "Back", CallbackData = QueryData.NewQueryString("Back", null, null)}
                      }
                }));
            }
        }
    }
}
