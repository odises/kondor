using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Extensions;
using Kondor.Service.Leitner;
using Kondor.Service.Managers;

namespace Kondor.Service.Processors
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IUserApi _userApi;
        private readonly ITelegramApiManager _telegramApiManager;
        private readonly ILeitnerService _leitnerService;
        private readonly ITextManager _textManager;
        private readonly IViews _views;

        public QueryProcessor(IUserApi userApi, 
                              ITelegramApiManager telegramApiManager, 
                              ILeitnerService leitnerService, 
                              ITextManager textManager, 
                              IViews views)
        {
            _userApi = userApi;
            _telegramApiManager = telegramApiManager;
            _leitnerService = leitnerService;
            _textManager = textManager;
            _views = views;
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
                case "Examples":
                    ProcessExamplesCommand(callbackQuery);
                    break;
                case "Refresh":
                    ProcessRefreshCommand(callbackQuery);
                    break;
            }
        }

        private void ProcessRefreshCommand(CallbackQuery callbackQuery)
        {
            ProcessExamplesCommand(callbackQuery);
        }

        private void ProcessExamplesCommand(CallbackQuery callbackQuery)
        {
            try
            {
                var example = _leitnerService.GetExample(callbackQuery.From.Id);

                var messageBody =
                    $"{example.Card.DeserializeCardData().GetFrontExamView()}{Environment.NewLine}{Environment.NewLine}{example.Sentence}";

                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id,
                int.Parse(callbackQuery.Message.MessageId),
                messageBody,
                "Markdown",
                true,
                _views.Examples().Keyboards);

            }
            catch (IndexOutOfRangeException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.NoExampleYet), true);
                ProcessBackCommand(callbackQuery);
            }
            
        }

        protected virtual void ProcessBackCommand(CallbackQuery callbackQuery)
        {
            _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, 
                int.Parse(callbackQuery.Message.MessageId), 
                _textManager.GetText(StringResources.BackMessage), 
                "Markdown", 
                true, 
                _views.Index().Keyboards);
        }
        protected virtual void ProcessAnswerCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            var cardStateId = int.Parse(queryData.Data);

            if (queryData.Action == "Reject")
            {
                _leitnerService.MoveBack(cardStateId);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.MovedBackMessage), false);
            }
            else if (queryData.Action == "Accept")
            {
                _leitnerService.MoveNext(cardStateId);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.MovedNextMessage), false);
            }
            else if (queryData.Action == "Again")
            {
                _leitnerService.Again(cardStateId);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.AgainMessage), false);
            }
            else
            {
                throw new InvalidDataException();
            }

            ProcessExamCommand(null, callbackQuery);
        }
        protected virtual void ProcessEnterCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            if (_userApi.IsRegisteredUser(callbackQuery.From.Id))
            {
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), _textManager.GetText(StringResources.BackMessage), "Markdown", true, _views.Index().Keyboards);
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

                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, 
                    int.Parse(callbackQuery.Message.MessageId), 
                    response, "Markdown", 
                    false, 
                    _views.Learn().Keyboards);
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

                var position = string.Empty;
                for (var i = 0; i < (int)cardState.CardPosition + 1; i++)
                {
                    position += _textManager.GetText(StringResources.Star);
                }

                var response = $"{_textManager.GetText(StringResources.RemainingCards)} {_leitnerService.GetNumberOfCardsReadyToTry(callbackQuery.From.Id)}\n\n{position}\n{cardState.Card.DeserializeCardData().GetFrontExamView()}";

                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, 
                    int.Parse(callbackQuery.Message.MessageId), 
                    response, "Markdown", 
                    true, 
                    _views.Exam(cardState.Id).Keyboards);

            }
            catch (IndexOutOfRangeException)
            {
                if (queryData == null)
                {
                    ProcessBackCommand(callbackQuery);
                }
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
                    _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, 
                        _textManager.GetText(StringResources.NoCardForExam), 
                        true);
                }
            }
            catch (ValidationException)
            {
                if (queryData == null)
                {
                    ProcessBackCommand(callbackQuery);
                }
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, 
                    _textManager.GetText(StringResources.UserIsNotValidMessage), 
                    true);
            }

        }
        protected virtual void ProcessDisplayCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            try
            {
                var cardStateId = int.Parse(queryData.Data);
                var cardState = _leitnerService.GetCardStateById(cardStateId);
                var response = cardState.Card.DeserializeCardData().GetBackExamView();

                var isDifficult = _leitnerService.IsDifficult(cardState);

                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, 
                    int.Parse(callbackQuery.Message.MessageId), 
                    response, "Markdown", 
                    true, 
                    _views.Display(isDifficult, cardStateId).Keyboards);
            }
            catch (NullReferenceException)
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, _textManager.GetText(StringResources.NoCardToDisplay), true);
            }

        }
    }
}
