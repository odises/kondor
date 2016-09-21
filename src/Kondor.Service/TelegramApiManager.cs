using System;
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
    public class MessageSentEventArgs : EventArgs
    {
        public string MessageId { get; set; }
        public int ChatId { get; set; }
    }

    public class TelegramApiManager
    {
        private readonly string _apiKey;

        public event EventHandler<MessageSentEventArgs> MessageSent;

        protected virtual void OnMessageSent(MessageSentEventArgs e)
        {
            MessageSent?.Invoke(this, e);
        }

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

            var responseModel = JsonConvert.DeserializeObject<TelegramApiResponseModel>(response);

            if (responseModel.Ok)
            {
                return JsonConvert.DeserializeObject<List<Update>>(responseModel.Result.ToString());
            }

            throw new InvalidDataException();
        }

        public Message SendMessage(int chatId, string text, string replyMarkup = null)
        {
            try
            {
                string response;
                if (string.IsNullOrEmpty(replyMarkup))
                {
                    var webClient = new WebClient();
                    response = webClient.DownloadString(
                        $"https://api.telegram.org/{_apiKey}/sendMessage?chat_id={chatId}&text={text}&parse_mode=Markdown");
                }
                else
                {
                    var webClient = new WebClient();
                    response = webClient.DownloadString(
                        $"https://api.telegram.org/{_apiKey}/sendMessage?chat_id={chatId}&text={text}&parse_mode=Markdown&reply_markup={replyMarkup}");
                }

                var parsedResponse = JsonConvert.DeserializeObject<TelegramApiResponseModel>(response);

                if (parsedResponse.Ok)
                {
                    var message = JsonConvert.DeserializeObject<Message>(parsedResponse.Result.ToString());
                    OnMessageSent(new MessageSentEventArgs
                    {
                        MessageId = message.MessageId,
                        ChatId = message.Chat.Id
                    });

                    return message;
                }
                else
                {
                    throw new InvalidDataException();
                }
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

                throw;
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

                var baseUri = $"https://api.telegram.org/{_apiKey}/answerCallbackQuery";

                var query = Uri.EscapeUriString(HttpUtility.UrlDecode(queryString.ToString()));

                var uri = $"{baseUri}?{query}";

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

        public void EditMessageText(int chatId, int messageId, string text, string parseMode, bool disableWebPagePreview,
            string replyMarkup = null)
        {
            var nameValueCollection = new NameValueCollection
            {
                {"chat_id", chatId.ToString()},
                {"message_id", messageId.ToString()},
                {"text", text},
                {"parse_mode", parseMode},
                {"disable_web_page_preview", disableWebPagePreview.ToString()},
            };

            if (!string.IsNullOrEmpty(replyMarkup))
            {
                nameValueCollection.Add("reply_markup", replyMarkup);
            }


            EditMessageText(nameValueCollection);
        }

        public void EditMessageText(string inlineMessageId, string text, string parseMode, bool disableWebPagePreview,
            string replyMarkup = null)
        {
            var nameValueCollection = new NameValueCollection
            {
                {"inline_message_id", inlineMessageId},
                {"text", text},
                {"parse_mode", parseMode},
                {"disable_web_page_preview", disableWebPagePreview.ToString()}
            };

            if (!string.IsNullOrEmpty(replyMarkup))
            {
                nameValueCollection.Add("reply_markup", replyMarkup);
            }

            EditMessageText(nameValueCollection);
        }

        protected void EditMessageText(NameValueCollection nameValueCollection)
        {
            try
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                foreach (string key in nameValueCollection)
                {
                    var value = nameValueCollection[key];
                    queryString.Add(key, value);
                }

                var baseUri = $"https://api.telegram.org/{_apiKey}/editMessageText";

                var query = Uri.EscapeUriString(HttpUtility.UrlDecode(queryString.ToString()));

                var uri = $"{baseUri}?{query}";

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
