using System;
using Kondor.Domain.Models;

namespace Kondor.Service.Leitner
{
    public interface ILeitnerService
    {
        int BoxCleanUp();
        CardState GetCardStateById(int cardId);
        CardState GetCardForExam(int telegramUserId);
        Tuple<int, DateTime> GetNextExamInformation(int telegramUserId);
        Example GetExample(int telegramUserId);
        Card AddOneNewCardToBox(int telegramUserId);
        void MoveBack(int cardStateId, bool overStoppingMode = false);
        void MoveNext(int cardStateId);
        int GetNumberOfCardsReadyToTry(int telegramUserId);
    }
}