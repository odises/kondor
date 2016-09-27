namespace Kondor.Service.Managers
{
    public interface ITextManager
    {
        string GetText(string groupCode, string userId = null);
    }
}
