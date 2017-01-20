using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Kondor.Data.SettingModels;
using Kondor.Domain;
using Kondor.Domain.Enums;
using Kondor.Domain.Models;
using Kondor.Service.Extensions;
using Kondor.Service.Handlers;

namespace Kondor.Service.Leitner
{
    public class LeitnerService : ILeitnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        protected int OverStoppingTolerance;
        private readonly TimeUnit _timeUnit;
        protected int FirstBoxCapacity;
        private readonly ISettingHandler _settingHandler;

        /// <summary>
        /// Initialization
        /// </summary>
        public LeitnerService(ISettingHandler settingHandler, IUnitOfWork unitOfWork)
        {
            _settingHandler = settingHandler;
            _unitOfWork = unitOfWork;

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
            var userFirstBoxCount = _unitOfWork.CardStateRepository
                .Count(p => p.UserId == userId && p.CardPosition == Position.First && p.Status == InboxCardsStatus.NewInPosition);

            if (userFirstBoxCount > FirstBoxCapacity)
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

            var expiredCardStates =
                _unitOfWork.CardStateRepository.Get(
                    p => p.Status == InboxCardsStatus.NewInPosition &&
                    p.CardPosition != Position.First &&
                    p.CardPosition != Position.Finished &&
                    p.ExaminationDateTime <= DateTime.Now);

            foreach (var card in expiredCardStates)
            {
                var diff = DateTime.Now.GetRounded() - card.ExaminationDateTime;
                if (diff.TotalMinutes > 0)
                {
                    if (diff.TotalMinutes > 10080) // fixed value for 7 days in minutes
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

            var randomCard = _unitOfWork.CardRepository.GetRandomlyCardToLearn(userId);

            if (randomCard == null)
            {
                throw new IndexOutOfRangeException();
            }

            var cardState = AddCardInFirstState(Position.First, userId, randomCard.Id);

            foreach (var example in randomCard.Examples)
            {
                if (!_unitOfWork.ExampleViewRepository.Any(p => p.ExampleId == example.Id && p.UserId == example.Card.UserId))
                {
                    var newExampleView = new ExampleView
                    {
                        ExampleId = example.Id,
                        UserId = example.Card.UserId,
                        Views = 0
                    };

                    _unitOfWork.ExampleViewRepository.Insert(newExampleView);
                }
            }

            _unitOfWork.CardStateRepository.Insert(cardState);

            _unitOfWork.Save();

            return randomCard;
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

            var card =
                _unitOfWork.CardStateRepository.Random(
                    p =>
                        p.Status == InboxCardsStatus.NewInPosition &&
                        p.CardPosition != Position.Finished &&
                        p.UserId == userId &&
                        p.ExaminationDateTime <= DateTime.Now, 1).FirstOrDefault();

            if (card == null)
            {
                throw new IndexOutOfRangeException();
            }

            return card;
        }
        public Tuple<int, DateTime> GetNextExamInformation(int telegramUserId)
        {
            var userId = GetUserIdByTelegramUserId(telegramUserId);

            var groupedByCardStates = _unitOfWork
                                    .CardStateRepository
                                    .FilterThenGroupBy(p => p.Status == InboxCardsStatus.NewInPosition 
                                    && p.CardPosition != Position.Finished 
                                    && p.UserId == userId, g => g.ExaminationDateTime)
                                    .OrderBy(p => p.Key)
                                    .ToList();

            if (!groupedByCardStates.Any())
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                var count = groupedByCardStates.FirstOrDefault().Count();
                var date = groupedByCardStates.FirstOrDefault().Key;

                return new Tuple<int, DateTime>(count, date);
            }
        }
        public CardState GetCardStateById(int id)
        {
            var cardState = _unitOfWork.CardStateRepository.GetById(id);
            if (cardState == null)
            {
                throw new NullReferenceException();
            }
            return cardState;
        }
        public int GetNumberOfCardsReadyToTry(int telegramUserId)
        {
            var userId = GetUserIdByTelegramUserId(telegramUserId);
            var cards = _unitOfWork.CardStateRepository.Get(p => p.Status == InboxCardsStatus.NewInPosition && p.CardPosition != Position.Finished && p.UserId == userId && p.ExaminationDateTime <= DateTime.Now);
            return cards.Count();
        }
        public bool IsDifficult(CardState cardState)
        {
            return _unitOfWork.CardRepository.IsDifficult(cardState.CardId);
        }
        public void Again(int cardStateId)
        {
            var cardState = GetCardStateById(cardStateId);
            cardState.Status = InboxCardsStatus.Again;
            cardState.ModifiedDateTime = DateTime.Now;
            _unitOfWork.CardStateRepository.Update(cardState);

            var nextPosition = Position.First;

            var newCardState = new CardState
            {
                CardPosition = nextPosition,
                CreationDateTime = DateTime.Now,
                ExaminationDateTime = DateTime.Now.AddSeconds(-5),
                Status = InboxCardsStatus.NewInPosition,
                UserId = cardState.UserId,
                CardId = cardState.CardId
            };

            _unitOfWork.CardStateRepository.Insert(newCardState);
            _unitOfWork.Save();
        }
        public Example GetExample(int telegramUserId)
        {
            var user = GetUserByTelegramId(telegramUserId);

            // todo refactor
            var exampleView =
                _unitOfWork.ExampleViewRepository.FilterThenGroupBy(p => p.UserId == user.Id, p => p.Views).OrderBy(p => p.Key).FirstOrDefault().GetRandom();

            if (exampleView == null)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                exampleView.Views = exampleView.Views + 1;
                _unitOfWork.ExampleViewRepository.Update(exampleView);
                _unitOfWork.Save();

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
            return Position.First;
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

            var user = _unitOfWork.UserRepository.GetUserByTelegramId(telegramUserId);
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
            var user = _unitOfWork.UserRepository.GetUserByTelegramId(telegramUserId);
            if (user == null)
            {
                throw new ValidationException();
            }
            else
            {
                return user;
            }
        }
        /// <summary>
        /// Moves the card one step next
        /// </summary>
        /// <param name="cardStateId"></param>
        public void MoveNext(int cardStateId)
        {
            var cardState = GetCardStateById(cardStateId);
            if (!cardState.ModifiedDateTime.HasValue)
            {
                var cardId = cardState.CardId;
                if (!IsDuplicateCardState(cardId))
                {
                    MoveNext(cardState);
                }
                else
                {
                    // todo log
                    Console.WriteLine($"Duplicated {cardId}");
                }
            }
            else
            {
                Console.WriteLine($"Already answered {cardState.CardId}");
            }
        }
        /// <summary>
        /// Moves the card one step back
        /// </summary>
        /// <param name="cardStateId"></param>
        /// <param name="overStoppingMode"></param>
        public void MoveBack(int cardStateId, bool overStoppingMode = false)
        {
            var cardState = GetCardStateById(cardStateId);
            if (!cardState.ModifiedDateTime.HasValue)
            {
                var cardId = cardState.CardId;
                if (!IsDuplicateCardState(cardId))
                {
                    MoveBack(cardState, overStoppingMode);
                }
                else
                {
                    // todo log
                    Console.WriteLine($"Duplicated {cardId}");
                }
            }
            else
            {
                Console.WriteLine($"Already answered {cardState.CardId}");
            }
        }
        protected bool IsDuplicateCardState(int cardId)
        {
            var now = DateTime.Now;

            if (_unitOfWork.CardStateRepository.Any(
                p => p.CardId == cardId && DbFunctions.DiffSeconds(p.CreationDateTime, now) < 10))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Moves the card one step next
        /// </summary>
        /// <param name="cardState"></param>
        protected void MoveNext(CardState cardState)
        {
            cardState.Status = InboxCardsStatus.Passed;
            cardState.ModifiedDateTime = DateTime.Now;
            _unitOfWork.CardStateRepository.Update(cardState);

            var nextPosition = GetNextPosition(cardState.CardPosition);
            if (nextPosition != Position.Finished)
            {
                var newCard = AddCardInFirstState(nextPosition, cardState.UserId, cardState.CardId);

                _unitOfWork.CardStateRepository.Insert(newCard);
            }
            
            _unitOfWork.Save();
        }
        /// <summary>
        /// Moves the card one step back
        /// </summary>
        /// <param name="cardState"></param>
        /// <param name="overStoppingMode"></param>
        protected void MoveBack(CardState cardState, bool overStoppingMode = false)
        {
            if (overStoppingMode)
            {
                cardState.Status = InboxCardsStatus.CleanedUp;
            }
            else
            {
                cardState.Status = InboxCardsStatus.Failed;
            }
            cardState.ModifiedDateTime = DateTime.Now;
            _unitOfWork.CardStateRepository.Update(cardState);

            
            var newCard = AddCardInFirstState(Position.First, cardState.UserId, cardState.CardId);
            if (overStoppingMode)
            {
                newCard.ExaminationDateTime = DateTime.Now;
            }

            _unitOfWork.CardStateRepository.Insert(newCard);
            _unitOfWork.Save();
        }
    }
}
