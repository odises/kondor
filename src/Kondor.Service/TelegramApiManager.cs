using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;

namespace Kondor.Service
{
    public class TelegramApiManager
    {
        private readonly string _apiKey;

        public TelegramApiManager(string apiKey)
        {
            _apiKey = apiKey;
        }

        public void AnswerCallbackQuery(string callbackQueryId, string text, bool showAlert)
        {
            try
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["callback_query_id"] = callbackQueryId;
                queryString["text"] = text;
                queryString["show_alert"] = showAlert.ToString();
                var nameValueCollection = new NameValueCollection
            {
                {"callback_query_id", callbackQueryId},
                {"text", text},
                {"show_alert", showAlert.ToString()}
            };
                var baseUri = $"https://api.telegram.org/{_apiKey}/answerCallbackQuery";

                var uri = baseUri + "?" + queryString.ToString();

                var webClient = new WebClient();
                var response = webClient.DownloadString(uri);
            }
            catch (WebException exception)
            {
                var errorResponse = (HttpWebResponse)exception.Response;
                var responseStream = errorResponse.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream);
                    var error = reader.ReadToEnd();
                    throw new WebException(error);
                }
            }
        }
    }
}
