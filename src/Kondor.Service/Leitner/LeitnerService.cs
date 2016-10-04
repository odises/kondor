﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.SettingModels;
using Kondor.Service.Extensions;
using Kondor.Service.Handlers;

namespace Kondor.Service.Leitner
{
    public class LeitnerService : ILeitnerService
    {
        private readonly IDbContext _context;
        protected int OverStoppingTolerance;
        private readonly TimeUnit _timeUnit;
        protected int FirstBoxCapacity;
        private readonly ISettingHandler _settingHandler;

        /// <summary>
        /// Initialization
        /// </summary>
        public LeitnerService(IDbContext context, ISettingHandler settingHandler)
        {
            _context = context;
            _settingHandler = settingHandler;

            FirstBoxCapacity = _settingHandler.GetSettings<GeneralSettings>().FirstBoxCapacity;
            _timeUnit = _settingHandler.GetSettings<GeneralSettings>().LeitnerTimeUnit;
            OverStoppingTolerance = _settingHandler.GetSettings<GeneralSettings>().LeitnerOverstoppingTolerance;
        }

        /// <summary>
        /// Validates maximum card in first position
        /// </summary>
        /// <param name="userId"></param>
        /// <exception cref="OverflowException">Maximum card in first position exceeded.</exception>
        protected virtual void ValidateMaximumCardInFirstPosition(string userId)
        {
            var count =
                _context.CardStates.Count(
                    p => p.UserId == userId && p.CardPosition == Position.First && p.Status == InboxCardsStatus.NewInPosition);
            if (count > FirstBoxCapacity)
            {
                throw new OverflowException();
            }
        }

        /// <summary>
        /// Cleans up box
        /// </summary>
        /// <returns></returns>
        public int BoxCleanUp()
        {
            var count = 0;
            var cards =
                _context.CardStates.Where(
                    p => p.Status == InboxCardsStatus.NewInPosition && p.CardPosition != Position.First && p.CardPosition != Position.Finished && p.ExaminationDateTime <= DateTime.Now).ToList();
            foreach (var card in cards)
            {
                var diff = DateTime.Now.GetRounded() - card.ExaminationDateTime;
                if (diff.TotalMinutes > 0)
                {
                    var positionStopTime = GetStopTimeForPositionInMinute(card.CardPosition, _timeUnit);
                    var tolerance = positionStopTime * (double)OverStoppingTolerance / 100;

                    if (diff.TotalMinutes > tolerance)
                    {
                        MoveBack(card, true);
                        count++;
                    }
                }

            }

            return count;
        }

        /// <summary>
        /// Adds a new card to user's leitner box
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">There is no new Mem</exception>
        /// <exception cref="ValidationException">UserId is not valid</exception>
        /// <exception cref="OverflowException">Maximum card in first position exceeded.</exception>
        public Card AddOneNewCardToBox(int telegramUserId)
        {
            var userId = GetUserIdByTelegramUserId(telegramUserId);

            ValidateMaximumCardInFirstPosition(userId);

            var mems = _context.Cards.Where(m => !_context.CardStates.Any(p => p.CardId == m.Id) && m.UserId == userId).ToList();

            if (mems.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            var mem = mems.GetRandom();


            var newCard = AddCardInFirstState(Position.First, userId, mem.Id);


            // todo Not now

            //foreach (var example in mem.Examples)
            //{
            //    if (!_context.ExampleViews.Any(p => p.ExampleId == example.Id && p.UserId == example.Mem.UserId))
            //    {
            //        _context.ExampleViews.Add(new ExampleView
            //        {
            //            ExampleId = example.Id,
            //            UserId = example.Mem.UserId,
            //            Views = 0
            //        });
            //    }
            //}

            _context.CardStates.Add(newCard);
            _context.SaveChanges();

            return mem;
        }

        /// <summary>
        /// Gets a card for examination
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">There is not card for examination</exception>
        /// <exception cref="ValidationException">UserId is not valid</exception>
        public CardState GetCardForExam(int telegramUserId)
        {
            var userId = GetUserIdByTelegramUserId(telegramUserId);

            var card = _context.CardStates.FirstOrDefault(p => p.Status == InboxCardsStatus.NewInPosition && p.CardPosition != Position.Finished && p.UserId == userId && p.ExaminationDateTime <= DateTime.Now);
            if (card == null)
            {
                throw new IndexOutOfRangeException();
            }
            return card;
        }

        public Tuple<int, DateTime> GetNextExamInformation(int telegramUserId)
        {
            var userId = GetUserIdByTelegramUserId(telegramUserId);
            var cards = _context.CardStates.Where(p => p.Status == InboxCardsStatus.NewInPosition && p.CardPosition != Position.Finished && p.UserId == userId);
            if (!cards.Any())
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                var groupedByExaminationDateTime = cards.GroupBy(p => p.ExaminationDateTime).OrderBy(p => p.Key);
                var count = groupedByExaminationDateTime.FirstOrDefault().Count();
                var date = groupedByExaminationDateTime.FirstOrDefault().Key;

                return new Tuple<int, DateTime>(count, date);
            }
        }

        public CardState GetCard(int cardId)
        {
            var card = _context.CardStates.FirstOrDefault(p => p.Id == cardId);
            return card;
        }

        /// <summary>
        /// Moves the card one step next
        /// </summary>
        /// <param name="card"></param>
        public void MoveNext(CardState card)
        {
            card.Status = InboxCardsStatus.Passed;
            card.ModifiedDateTime = DateTime.Now;

            var nextPosition = GetNextPosition(card.CardPosition);
            var newCard = AddCardInFirstState(nextPosition, card.UserId, card.CardId);

            _context.CardStates.Add(newCard);
            _context.SaveChanges();
        }

        public int GetNumberOfCardsReadyToTry(int telegramUserId)
        {
            var userId = GetUserIdByTelegramUserId(telegramUserId);
            var cards = _context.CardStates.Where(p => p.Status == InboxCardsStatus.NewInPosition && p.CardPosition != Position.Finished && p.UserId == userId && p.ExaminationDateTime <= DateTime.Now);
            return cards.Count();
        }

        /// <summary>
        /// Moves the card one step next
        /// </summary>
        /// <param name="cardId"></param>
        public void MoveNext(int cardId)
        {
            var card = GetCard(cardId);
            MoveNext(card);
        }

        /// <summary>
        /// Moves the card one step back
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="overStoppingMode"></param>
        public void MoveBack(int cardId, bool overStoppingMode = false)
        {
            var card = GetCard(cardId);
            MoveBack(card, overStoppingMode);
        }

        /// <summary>
        /// Moves the card one step back
        /// </summary>
        /// <param name="card"></param>
        /// <param name="overStoppingMode"></param>
        public void MoveBack(CardState card, bool overStoppingMode = false)
        {
            if (overStoppingMode)
            {
                card.Status = InboxCardsStatus.CleanedUp;
            }
            else
            {
                card.Status = InboxCardsStatus.Failed;
            }
            card.ModifiedDateTime = DateTime.Now;

            var prevPosition = GetPreviousPosition(card.CardPosition);
            var newCard = AddCardInFirstState(prevPosition, card.UserId, card.CardId);

            _context.CardStates.Add(newCard);
            _context.SaveChanges();
        }

        public Example GetExample(int telegramUserId)
        {
            var user = GetUserByTelegramId(telegramUserId);

            var exampleView =
                _context.ExampleViews.Where(p => p.UserId == user.Id).GroupBy(p => p.Views).OrderBy(p => p.Key).FirstOrDefault().GetRandom();
            if (exampleView == null)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                exampleView.Views = exampleView.Views + 1;
                _context.SaveChanges();

                return exampleView.Example;
            }

        }

        /// <summary>
        /// Gets next position
        /// </summary>
        /// <param name="cardPosition"></param>
        /// <returns></returns>
        protected virtual Position GetNextPosition(Position cardPosition)
        {
            switch (cardPosition)
            {
                case Position.First:
                    return Position.Second;
                case Position.Second:
                    return Position.Third;
                case Position.Third:
                    return Position.Fourth;
                case Position.Fourth:
                    return Position.Fifth;
                case Position.Fifth:
                    return Position.Finished;
                case Position.Finished:
                    throw new ArgumentOutOfRangeException(nameof(cardPosition), cardPosition, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardPosition), cardPosition, null);
            }
        }

        /// <summary>
        /// Gets previous position
        /// </summary>
        /// <param name="cardPosition"></param>
        /// <returns></returns>
        protected virtual Position GetPreviousPosition(Position cardPosition)
        {
            switch (cardPosition)
            {
                case Position.First:
                    return Position.First;
                case Position.Second:
                    return Position.First;
                case Position.Third:
                    return Position.First;
                case Position.Fourth:
                    return Position.First;
                case Position.Fifth:
                    return Position.First;
                case Position.Finished:
                    throw new ArgumentOutOfRangeException(nameof(cardPosition), cardPosition, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardPosition), cardPosition, null);
            }
        }

        /// <summary>
        /// Gets time-stop for a specific position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        protected virtual int GetStopTimeForPositionInMinute(Position position, TimeUnit timeUnit)
        {
            int stopTime;
            switch (position)
            {
                case Position.First:
                    stopTime = 1;
                    break;
                case Position.Second:
                    stopTime = 2;
                    break;
                case Position.Third:
                    stopTime = 4;
                    break;
                case Position.Fourth:
                    stopTime = 8;
                    break;
                case Position.Fifth:
                    stopTime = 15;
                    break;
                case Position.Finished:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }

            switch (timeUnit)
            {
                case TimeUnit.Minute:
                    return stopTime;
                case TimeUnit.Hour:
                    return stopTime * 60;
                case TimeUnit.Day:
                    return stopTime * 60 * 24;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, null);
            }
        }

        /// <summary>
        /// Add a card to first state
        /// </summary>
        /// <param name="position"></param>
        /// <param name="userId"></param>
        /// <param name="memId"></param>
        /// <returns></returns>
        protected virtual CardState AddCardInFirstState(Position position, string userId, int memId)
        {
            var stopTime = GetStopTimeForPositionInMinute(position, _timeUnit);

            var cardState = new CardState
            {
                CardPosition = position,
                CreationDateTime = DateTime.Now,
                ExaminationDateTime = DateTime.Now.AddMinutes(stopTime).GetRounded(),
                Status = InboxCardsStatus.NewInPosition,
                UserId = userId,
                CardId = memId
            };

            return cardState;
        }

        /// <summary>
        /// Returns user id by his/her Telegram user id
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns>User Id</returns>
        /// <exception cref="ValidationException">There is no user with passed Telegram Id</exception>
        protected virtual string GetUserIdByTelegramUserId(int telegramUserId)
        {

            var user = _context.Set<ApplicationUser>().FirstOrDefault(p => p.TelegramUserId == telegramUserId);
            if (user == null)
            {
                throw new ValidationException();
            }
            else
            {
                return user.Id;
            }
        }

        protected virtual ApplicationUser GetUserByTelegramId(int telegramUserId)
        {
            var user = _context.Set<ApplicationUser>().FirstOrDefault(p => p.TelegramUserId == telegramUserId);
            if (user == null)
            {
                throw new ValidationException();
            }
            else
            {
                return user;
            }
        }
    }
}
