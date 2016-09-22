using Kondor.Data.DataModel;

namespace Kondor.Service.Leitner
{
    public interface ILeitnerService
    {
        int BoxCleanUp();
        Card GetCard(int cardId);
        Card GetCardForExam(int telegramUserId);
        string GetExample(int telegramUserId);
        Mem GetNewMem(int telegramUserId);
        void MoveBack(int cardId, bool overStoppingMode = false);
        void MoveBack(Card card, bool overStoppingMode = false);
        void MoveNext(int cardId);
        void MoveNext(Card card);
        int GetNumberOfCardsReadyToTry(int telegramUserId);
    }
}