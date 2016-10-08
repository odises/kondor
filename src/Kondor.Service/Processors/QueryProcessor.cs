using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Kondor.Data.Enums;
using Kondor.Data.SettingModels;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Extensions;
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
        private readonly ITextManager _textManager;

        public QueryProcessor(IUserApi userApi, ITelegramApiManager telegramApiManager, ILeitnerService leitnerService, ISettingHandler settingHandler, ITextManager textManager)
        {
            _userApi = userApi;
            _telegramApiManager = telegramApiManager;
            _leitnerService = leitnerService;
            _settingHandler = settingHandler;
            _textManager = textManager;
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

                var messageFormatter = _textManager.GetText(StringResources.ExampleBoardMessageFormatter);
                var messageBody = string.Format(messageFormatter, example.Sentence);


                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId),
                    messageBody, "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[]
                {
                        new[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = _textManager.GetText(StringResources.ExampleBoardRefreshKeyboardTitle),
                                CallbackData = QueryData.NewQueryString("ExampleBoardRefresh", null, null)
                            },
                            new InlineKeyboardButton
                            {
                                Text = _textManager.GetText(StringResources.ExampleBoardSpeakKeyboardTitle),
                                Url = string.Format(_settingHandler.GetSettings<GeneralSettings>().SpeakBaseUri, example.Id, TextType.Example)
                            }
                        }
                }));
            }
            catch (IndexOutOfRangeException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.NoExampleYet), true);
            }
        }

        protected virtual void ProcessBackCommand(CallbackQuery callbackQuery)
        {
            _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), _textManager.GetText(StringResources.BackMessage), "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardLearnTitle), CallbackData = QueryData.NewQueryString("Learn", null, null)},
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardExamTitle), CallbackData = QueryData.NewQueryString("Exam", null, null)}
                    }}));
        }

        protected virtual void ProcessAnswerCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            var cardId = int.Parse(queryData.Data);

            if (queryData.Action == _textManager.GetText(StringResources.KeyboardRejectTitle))
            {
                _leitnerService.MoveBack(cardId);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.MovedBackMessage), true);
            }
            else if (queryData.Action == _textManager.GetText(StringResources.KeyboardAcceptTitle))
            {
                _leitnerService.MoveNext(cardId);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.MovedNextMessage), true);
            }
            else
            {
                throw new InvalidDataException();
            }

            _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), _textManager.GetText(StringResources.BackMessage), "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardLearnTitle), CallbackData = QueryData.NewQueryString("Learn", null, null)},
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardExamTitle), CallbackData = QueryData.NewQueryString("Exam", null, null)}
                    }}));
        }

        protected virtual void ProcessEnterCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            if (_userApi.IsRegisteredUser(callbackQuery.From.Id))
            {
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), _textManager.GetText(StringResources.BackMessage), "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardLearnTitle), CallbackData = QueryData.NewQueryString("Learn", null, null)},
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardExamTitle), CallbackData = QueryData.NewQueryString("Exam", null, null)}
                    }}));
            }
            else
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.NotRegisteredYetMessage), true);
            }
        }
        protected virtual void ProcessLearnCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            try
            {
                var card = _leitnerService.AddOneNewCardToBox(callbackQuery.From.Id);

                var response = card.DeserializeCardData().GetLearnView();

                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), response, "Markdown", false, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardImagesTitle), Url = string.Format(_settingHandler.GetSettings<GeneralSettings>().ImagesBaseUri, card.Id)},
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardBackTitle), CallbackData = QueryData.NewQueryString("Back", null, null)}
                    }}));
            }
            catch (IndexOutOfRangeException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.NoNewMemMessage), true);
            }
            catch (ValidationException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.UserIsNotValidMessage), true);
            }
            catch (OverflowException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.MaximumCardInFirstPositionExceeded), true);
            }
        }
        protected virtual void ProcessExamCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            try
            {
                var cardState = _leitnerService.GetCardForExam(callbackQuery.From.Id);

                var response = cardState.Card.DeserializeCardData().GetFrontExamView();

                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), response, "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
            {
                new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardImagesTitle), Url = string.Format(_settingHandler.GetSettings<GeneralSettings>().ImagesBaseUri, cardState.Card.Id)},
                new InlineKeyboardButton
                {
                    Text = _textManager.GetText(StringResources.KeyboardDisplayTitle),
                    CallbackData = QueryData.NewQueryString("Display", null, cardState.Id.ToString())
                },
                new InlineKeyboardButton
                {
                    Text = _textManager.GetText(StringResources.KeyboardBackTitle),
                    CallbackData = QueryData.NewQueryString("Back", null, null)
                }
            }}));

            }
            catch (IndexOutOfRangeException)
            {
                try
                {
                    var nextExamInformations = _leitnerService.GetNextExamInformation(callbackQuery.From.Id);
                    var count = nextExamInformations.Item1;
                    var timeInfoTuple = nextExamInformations.Item2.Humanize();
                    var time = _textManager.GetText(timeInfoTuple.Item2).FormatWith(timeInfoTuple.Item1);

                    _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.NextExamInformation).FormatWith(count, time), true);
                }
                catch (IndexOutOfRangeException)
                {
                    _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.NoCardForExam), true);
                }
            }
            catch (ValidationException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.UserIsNotValidMessage), true);
            }

        }
        protected virtual void ProcessDisplayCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            var datetime = new DateTime(queryData.Ticks);
            if (datetime < DateTime.Now.AddSeconds(-30))
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.ExpiredThreadMessage), true);
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), _textManager.GetText(StringResources.BackMessage), "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardLearnTitle), CallbackData = QueryData.NewQueryString("Learn", null, null)},
                        new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardExamTitle), CallbackData = QueryData.NewQueryString("Exam", null, null)}
                    }}));
            }
            else
            {
                var cardState = _leitnerService.GetCard(int.Parse(queryData.Data));
                var response = cardState.Card.DeserializeCardData().GetBackExamView();

                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), response, "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[]
                {
                      new []
                      {
                          new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardAcceptTitle), CallbackData = QueryData.NewQueryString("Answer", "Accept", cardState.Id.ToString())},
                          new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardRejectTitle), CallbackData = QueryData.NewQueryString("Answer", "Reject", cardState.Id.ToString())},
                          new InlineKeyboardButton {Text = _textManager.GetText(StringResources.KeyboardBackTitle), CallbackData = QueryData.NewQueryString("Back", null, null)}
                      }
                }));
            }
        }
    }
}
