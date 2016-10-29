namespace Kondor.Service
{
    public interface IViews
    {
        RenderedViewModel Index();
        RenderedViewModel Login(string registrationUrl);
        RenderedViewModel Learn();
        RenderedViewModel Exam(int cardStateId);
        RenderedViewModel Display(bool isDifficult, int cardStateId);
    }
}