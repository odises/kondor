using Kondor.Data.TelegramTypes;
using Kondor.Service.Managers;

namespace Kondor.Service
{
    public class Views : IViews
    {
        private readonly ITextManager _textManager;

        public Views(ITextManager textManager)
        {
            _textManager = textManager;
        }

        public RenderedViewModel Index()
        {
            var result = new RenderedViewModel();

            result.MessageBody = _textManager.GetText(StringResources.AlreadyRegistered);

            var keyboards = new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = _textManager.GetText(StringResources.KeyboardLearnTitle),
                        CallbackData = QueryData.NewQueryString("Learn", null, null)
                    },
                    new InlineKeyboardButton
                    {
                        Text = _textManager.GetText(StringResources.KeyboardExamTitle),
                        CallbackData = QueryData.NewQueryString("Exam", null, null)
                    }
                }
            };

            result.Keyboards = TelegramHelper.GetInlineKeyboardMarkup(keyboards);

            return result;
        }

        public RenderedViewModel Login(string registrationUrl)
        {
            var result = new RenderedViewModel();

            result.MessageBody = _textManager.GetText(StringResources.RegistrationMessage);

            var keyboards = new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = _textManager.GetText(StringResources.KeyboardEnterTitle),
                        CallbackData = QueryData.NewQueryString("Enter", null, null)
                    },
                    new InlineKeyboardButton
                    {
                        Text = _textManager.GetText(StringResources.KeyboardRegistrationTitle),
                        Url = registrationUrl
                    }
                }
            };

            result.Keyboards = TelegramHelper.GetInlineKeyboardMarkup(keyboards);

            return result;
        }

        public RenderedViewModel Learn()
        {
            var result = new RenderedViewModel();

            var keyboards = new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = _textManager.GetText(StringResources.KeyboardBackTitle),
                        CallbackData = QueryData.NewQueryString("Back", null, null)
                    }
                }
            };

            result.Keyboards = TelegramHelper.GetInlineKeyboardMarkup(keyboards);
            return result;
        }

        public RenderedViewModel Exam(int cardStateId)
        {
            var result = new RenderedViewModel();

            var keyboards = new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = _textManager.GetText(StringResources.KeyboardDisplayTitle),
                        CallbackData = QueryData.NewQueryString("Display", null, cardStateId.ToString())
                    },
                    new InlineKeyboardButton
                    {
                        Text = _textManager.GetText(StringResources.KeyboardNextMemTitle),
                        CallbackData = QueryData.NewQueryString("Exam", null, null)
                    }

                },
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = _textManager.GetText(StringResources.KeyboardBackTitle),
                        CallbackData = QueryData.NewQueryString("Back", null, null)
                    }
                }
            };

            result.Keyboards = TelegramHelper.GetInlineKeyboardMarkup(keyboards);

            return result;
        }

        public RenderedViewModel Display(bool isDifficult, int cardStateId)
        {
            var result = new RenderedViewModel();

            InlineKeyboardButton[][] keyboards;
            if (isDifficult)
            {
                keyboards = new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton
                        {
                            Text = _textManager.GetText(StringResources.KeyboardRejectTitle),
                            CallbackData = QueryData.NewQueryString("Answer", "Reject", cardStateId.ToString())
                        },
                        new InlineKeyboardButton
                        {
                            Text = _textManager.GetText(StringResources.KeyboardAgainTitle),
                            CallbackData = QueryData.NewQueryString("Answer", "Again", cardStateId.ToString())
                        }
                    }
                };
            }
            else
            {
                keyboards = new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton
                        {
                            Text = _textManager.GetText(StringResources.KeyboardAcceptTitle),
                            CallbackData = QueryData.NewQueryString("Answer", "Accept", cardStateId.ToString())
                        },
                        new InlineKeyboardButton
                        {
                            Text = _textManager.GetText(StringResources.KeyboardRejectTitle),
                            CallbackData = QueryData.NewQueryString("Answer", "Reject", cardStateId.ToString())
                        },
                        new InlineKeyboardButton
                        {
                            Text = _textManager.GetText(StringResources.KeyboardAgainTitle),
                            CallbackData = QueryData.NewQueryString("Answer", "Again", cardStateId.ToString())
                        }
                    }
                };
            }

            result.Keyboards = TelegramHelper.GetInlineKeyboardMarkup(keyboards);

            return result;
        }
    }
}
