using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Kondor.Data.ApiModels;
using Kondor.Data.TelegramTypes;
using Newtonsoft.Json;

namespace Kondor.Service
{
    public class TelegramApiManager
    {
        private readonly string _apiKey;

        public TelegramApiManager(string apiKey)
        {
            _apiKey = apiKey;
        }

        public List<Update> GetUpdates(int? lastUpdateId = null)
        {
            var webClient = new WebClient();

            string response;

            if (lastUpdateId != null)
            {
                response = webClient.DownloadString(
                    $"https://api.telegram.org/{_apiKey}/getupdates?offset={lastUpdateId}");
            }
            else
            {
                response = webClient.DownloadString(
                    $"https://api.telegram.org/{_apiKey}/getupdates");
            }

            var responseModel = JsonConvert.DeserializeObject<GetUpdatesResult>(response);

            if (responseModel.Ok)
            {
                return responseModel.Result;
            }

            return new List<Update>();
        }

        public void SendMessage(int chatId, string text, string replyMarkup = null)
        {
            if (string.IsNullOrEmpty(replyMarkup))
            {
                var webClient = new WebClient();
                var response = webClient.DownloadString(
                    $"https://api.telegram.org/{_apiKey}/sendMessage?chat_id={chatId}&text={text}&parse_mode=Markdown");
            }
            else
            {
                var webClient = new WebClient();
                var response = webClient.DownloadString(
                    $"https://api.telegram.org/{_apiKey}/sendMessage?chat_id={chatId}&text={text}&parse_mode=Markdown&reply_markup={replyMarkup}");
            }
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
