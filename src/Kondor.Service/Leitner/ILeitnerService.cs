using System;
using Kondor.Domain.Models;

namespace Kondor.Service.Leitner
{
    public interface ILeitnerService
    {
        int BoxCleanUp();
        CardState GetCardState(int cardId);
        CardState GetCardForExam(int telegramUserId);
        Tuple<int, DateTime> GetNextExamInformation(int telegramUserId);
        Example GetExample(int telegramUserId);
        Card AddOneNewCardToBox(int telegramUserId);
        void MoveBack(int cardId, bool overStoppingMode = false);
        void MoveBack(CardState cardState, bool overStoppingMode = false);
        void MoveNext(int cardId);
        void MoveNext(CardState cardState);
        int GetNumberOfCardsReadyToTry(int telegramUserId);
    }
}