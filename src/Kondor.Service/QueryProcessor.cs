﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Kondor.Data.DataModel;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Leitner;

namespace Kondor.Service
{
    public class QueryProcessor
    {
        private readonly UserApi _userApi;
        private readonly TelegramApiManager _telegramApiManager;
        private readonly LeitnerService _leitnerService;
        private readonly List<Tuple<int, Card>> _userActiveCard;


        public QueryProcessor(UserApi userApi, TelegramApiManager telegramApiManager, LeitnerService leitnerService, List<Tuple<int, Card>> userActiveCard)
        {
            _userApi = userApi;
            _telegramApiManager = telegramApiManager;
            _leitnerService = leitnerService;
            _userActiveCard = userActiveCard;
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
                case "Ignore":
                    ProcessIgnoreCommand(queryData, callbackQuery);
                    break;
            }
        }

        protected virtual void ProcessAnswerCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            var card = _leitnerService.GetCard(int.Parse(queryData.Data));

            if (queryData.Action == "Reject")
            {
                _leitnerService.MoveBack(card);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "Moved back", true);
            }
            else if (queryData.Action == "Accept")
            {
                _leitnerService.MoveNext(card);
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "Moved next", true);
            }
            else
            {
                throw new InvalidDataException();
            }

            _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), "What do you want to do?", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null, DateTime.Now.Ticks)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null, DateTime.Now.Ticks)}
                    }}));
        }
        protected virtual void ProcessIgnoreCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), "What do you want to do?", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null, DateTime.Now.Ticks)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null, DateTime.Now.Ticks)}
                    }}));
        }
        protected virtual void ProcessEnterCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            if (_userApi.IsRegisteredUser(callbackQuery.From.Id))
            {
                // todo: check if user has entered once

                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), "What do you want to do?", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null, DateTime.Now.Ticks)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null, DateTime.Now.Ticks)}
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
                var response = newMem.ToMarkdown();
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), response, "Markdown", false, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null, DateTime.Now.Ticks)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null, DateTime.Now.Ticks)}
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
            Card card;
            var userActiveCard = _userActiveCard.FirstOrDefault(p => p.Item1 == callbackQuery.From.Id);
            if (userActiveCard != null)
            {
                card = userActiveCard.Item2;
            }
            else
            {
                try
                {
                    card = _leitnerService.GetCardForExam(callbackQuery.From.Id);
                    _userActiveCard.Add(new Tuple<int, Card>(callbackQuery.From.Id, card));
                }
                catch (IndexOutOfRangeException)
                {
                    _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "There is not card for exam yet.", true);
                    return;
                }
            }

            _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), $"*{card.Mem.MemBody}*", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
            {
                new InlineKeyboardButton
                {
                    Text = "Display",
                    CallbackData = QueryData.NewQueryString("Display", null, card.Id.ToString(), DateTime.Now.Ticks)
                },
                new InlineKeyboardButton
                {
                    Text = "Ignore",
                    CallbackData = QueryData.NewQueryString("Ignore", null, null, 0)
                }
            }}));
        }
        protected virtual void ProcessDisplayCommand(QueryData queryData, CallbackQuery callbackQuery)
        {
            var datetime = new DateTime(queryData.Ticks);
            if (datetime < DateTime.Now.AddSeconds(-30))
            {
                _telegramApiManager.AnswerCallbackQuery(callbackQuery.Id, "This thread is expired.", true);
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), "What do you want to do?", "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[] {new []
                    {
                        new InlineKeyboardButton {Text = "Learn", CallbackData = QueryData.NewQueryString("Learn", null, null, DateTime.Now.Ticks)},
                        new InlineKeyboardButton {Text = "Exam", CallbackData = QueryData.NewQueryString("Exam", null, null, DateTime.Now.Ticks)}
                    }}));
            }
            else
            {
                var card = _leitnerService.GetCard(int.Parse(queryData.Data));
                _telegramApiManager.EditMessageText(callbackQuery.Message.Chat.Id, int.Parse(callbackQuery.Message.MessageId), card.Mem.ToMarkdown(), "Markdown", true, TelegramHelper.GetInlineKeyboardMarkup(new[]
                {
                      new []
                      {
                          new InlineKeyboardButton {Text = "Accept", CallbackData = QueryData.NewQueryString("Answer", "Accept", card.Id.ToString(), DateTime.Now.Ticks)},
                          new InlineKeyboardButton {Text = "Reject", CallbackData = QueryData.NewQueryString("Answer", "Reject", card.Id.ToString(), DateTime.Now.Ticks)}
                      }
                    }));
            }
        }
    }
}
