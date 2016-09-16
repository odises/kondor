using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Service.Extensions;

namespace Kondor.Service.Leitner
{
    public class LeitnerService
    {
        private readonly EntityContext _entityContext;
        protected int OverStoppingTolerance;
        private readonly TimeUnit _timeUnit;
        protected int MaximumCardInFirstPosition;

        /// <summary>
        /// Initialization
        /// </summary>
        public LeitnerService(int overStoppingTolerance, int maximumCardInFirstPosition, TimeUnit timeUnit)
        {
            _entityContext = new EntityContext();
            MaximumCardInFirstPosition = maximumCardInFirstPosition;
            _timeUnit = timeUnit;
            OverStoppingTolerance = overStoppingTolerance;
        }

        /// <summary>
        /// Validates maximum card in first position
        /// </summary>
        /// <param name="userId"></param>
        /// <exception cref="OverflowException">Maximum card in first position exceeded.</exception>
        protected virtual void ValidateMaximumCardInFirstPosition(string userId)
        {
            var count =
                _entityContext.Cards.Count(
                    p => p.UserId == userId && p.CardPosition == Position.First && p.Status == CardStatus.NewInPosition);
            if (count > MaximumCardInFirstPosition)
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
                _entityContext.Cards.Where(
                    p => p.Status == CardStatus.NewInPosition && p.CardPosition != Position.First && p.CardPosition != Position.Finished && p.ExaminationDateTime <= DateTime.Now).ToList();
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
        /// Gets new Mem
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">There is no new Mem</exception>
        /// <exception cref="ValidationException">UserId is not valid</exception>
        /// <exception cref="OverflowException">Maximum card in first position exceeded.</exception>
        public Mem GetNewMem(int telegramUserId)
        {
            var userId = GetUserIdByTelegramUserId(telegramUserId);

            ValidateMaximumCardInFirstPosition(userId);

            var mems = _entityContext.Mems.Where(m => !_entityContext.Cards.Any(p => p.MemId == m.Id) && m.UserId == userId).ToList();

            if (mems.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            var mem = mems.GetRandom();


            var newCard = GenerateNewCard(Position.First, userId, mem.Id);


            foreach (var example in mem.Examples)
            {
                _entityContext.ExampleViews.Add(new ExampleView
                {
                    ExampleId = example.Id,
                    UserId = example.Mem.UserId,
                    Views = 0
                });
            }

            _entityContext.Cards.Add(newCard);
            _entityContext.SaveChanges();

            return mem;
        }

        /// <summary>
        /// Gets a card for examination
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">There is not card for examination</exception>
        /// <exception cref="ValidationException">UserId is not valid</exception>
        public Card GetCardForExam(int telegramUserId)
        {
            var userId = GetUserIdByTelegramUserId(telegramUserId);

            var card = _entityContext.Cards.FirstOrDefault(p => p.Status == CardStatus.NewInPosition && p.CardPosition != Position.Finished && p.UserId == userId && p.ExaminationDateTime <= DateTime.Now);
            if (card == null)
            {
                throw new IndexOutOfRangeException();
            }
            return card;
        }

        public Card GetCard(int cardId)
        {
            var card = _entityContext.Cards.FirstOrDefault(p => p.Id == cardId);
            return card;
        }

        /// <summary>
        /// Moves the card one step next
        /// </summary>
        /// <param name="card"></param>
        public void MoveNext(Card card)
        {
            card.Status = CardStatus.Passed;
            card.ModifiedDateTime = DateTime.Now;

            var nextPosition = GetNextPosition(card.CardPosition);
            var newCard = GenerateNewCard(nextPosition, card.UserId, card.MemId);

            _entityContext.Cards.Add(newCard);
            _entityContext.SaveChanges();
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
        public void MoveBack(Card card, bool overStoppingMode = false)
        {
            if (overStoppingMode)
            {
                card.Status = CardStatus.CleanedUp;
            }
            else
            {
                card.Status = CardStatus.Failed;
            }
            card.ModifiedDateTime = DateTime.Now;

            var prevPosition = GetPreviousPosition(card.CardPosition);
            var newCard = GenerateNewCard(prevPosition, card.UserId, card.MemId);

            _entityContext.Cards.Add(newCard);
            _entityContext.SaveChanges();
        }

        public string GetExample(int telegramUserId)
        {
            var user = GetUserByTelegramId(telegramUserId);
            using (var entities = new EntityContext())
            {
                var example =
                    entities.ExampleViews.Where(p => p.UserId == user.Id).GroupBy(p => p.Views).OrderBy(p => p.Key).FirstOrDefault().GetRandom();
                if (example == null)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    example.Views = example.Views + 1;
                    entities.SaveChanges();

                    return example.Example.Sentence;
                }
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
        /// Generates new card
        /// </summary>
        /// <param name="position"></param>
        /// <param name="userId"></param>
        /// <param name="memId"></param>
        /// <returns></returns>
        protected virtual Card GenerateNewCard(Position position, string userId, int memId)
        {
            var stopTime = GetStopTimeForPositionInMinute(position, _timeUnit);

            var newCard = new Card
            {
                CardPosition = position,
                CreationDateTime = DateTime.Now,
                ExaminationDateTime = DateTime.Now.AddMinutes(stopTime).GetRounded(),
                Status = CardStatus.NewInPosition,
                UserId = userId,
                MemId = memId
            };

            return newCard;
        }

        /// <summary>
        /// Returns user id by his/her Telegram user id
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns>User Id</returns>
        /// <exception cref="ValidationException">There is no user with passed Telegram Id</exception>
        protected virtual string GetUserIdByTelegramUserId(int telegramUserId)
        {
            var user = _entityContext.Users.FirstOrDefault(p => p.TelegramUserId == telegramUserId);
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
            var user = _entityContext.Users.FirstOrDefault(p => p.TelegramUserId == telegramUserId);
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
