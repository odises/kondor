﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using Kondor.Data;
using Kondor.Data.ApiModels;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Extensions;
using Kondor.Service.Leitner;
using Newtonsoft.Json;

namespace Kondor.Service
{
    public class TelegramMessageHandler
    {
        private readonly LeitnerService _leitnerService;
        private readonly string _apiKey;
        private readonly string _directory;
        private readonly List<Tuple<int, Card>> _userActiveCard;
        private readonly string _cipherKey;
        private readonly string _baseUri;

        public TelegramMessageHandler(string apiKey, string directory, string cipherKey, string baseUri)
        {
            _userActiveCard = new List<Tuple<int, Card>>();
            _leitnerService = new LeitnerService(20, 15);
            _apiKey = apiKey;
            _directory = directory;
            _cipherKey = cipherKey;
            _baseUri = baseUri;
        }

        protected void SendMessage(int chatId, string text, string replyMarkup = null)
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
        public string GetRegistrationLink(int telegramUserId, string telegramUsername, string baseUri)
        {
            var encrypted = StringCipher.Encrypt($"{telegramUserId}:{telegramUsername}", _cipherKey);
            var base64Encoded = encrypted.GetBase64Encode();
            return $"{baseUri}/{base64Encoded}";
        }
        public int GetMessages()
        {
            var count = 0;

            using (var entities = new EntityContext())
            {
                var lastUpdateId = 0;
                var firstOrDefault = entities.Updates.OrderByDescending(p => p.UpdateId).FirstOrDefault();
                if (firstOrDefault != null)
                {
                    lastUpdateId = firstOrDefault.UpdateId;
                }

                var webClient = new WebClient();

                string response;

                if (lastUpdateId > 0)
                {
                    response = webClient.DownloadString(
                        $"https://api.telegram.org/{_apiKey}/getupdates?offset={lastUpdateId}");
                }
                else
                {
                    response = webClient.DownloadString(
                        $"https://api.telegram.org/{_apiKey}/getupdates");
                }

                try
                {
                    var responseModel = JsonConvert.DeserializeObject<GetUpdatesResult>(response);
                    count = responseModel.Result.Count;
                    foreach (var item in responseModel.Result)
                    {
                        UpdateType updateType;

                        if (item.Message != null)
                        {
                            updateType = UpdateType.Message;
                        }
                        else if (item.EditedMessage != null)
                        {
                            updateType = UpdateType.EditedMessage;
                        }
                        else if (item.InlineQuery != null)
                        {
                            updateType = UpdateType.InlineQuery;
                        }
                        else if (item.ChosenInlineResult != null)
                        {
                            updateType = UpdateType.ChosenInlineResult;
                        }
                        else if (item.CallbackQuery != null)
                        {
                            updateType = UpdateType.CallbackQuery;
                        }
                        else
                        {
                            updateType = UpdateType.Unclear;
                        }

                        if (!entities.Updates.Any(p => p.UpdateId == item.UpdateId))
                        {
                            entities.Updates.Add(new Data.DataModel.Update
                            {
                                UpdateId = item.UpdateId,
                                Status = UpdateStatus.Unprocessed,
                                CreationDatetime = DateTime.Now,
                                ModifiedDatetime = DateTime.Now,
                                UpdateType = updateType,
                                SerializedUpdate = JsonConvert.SerializeObject(item, Formatting.None, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                })
                            });
                        }
                    }
                }
                catch (JsonReaderException exception)
                {
                    Console.WriteLine("invalid json format");
                }

                entities.SaveChanges();

                return count;
            }
        }
        public string GenerateExamHtml(Card card)
        {
            return GenerateMemHtml(card.Mem);
        }
        public string GenerateMemHtml(Mem mem)
        {
            var result = "<b>This is just a test</b><i>Believe me</i>";
            return result;
        }
        public int ProcessMessages()
        {
            using (var entities = new EntityContext())
            {
                var unprocessed = entities.Updates.Where(p => p.Status == UpdateStatus.Unprocessed).ToList();

                foreach (var update in unprocessed)
                {
                    var model = JsonConvert.DeserializeObject<Data.TelegramTypes.Update>(update.SerializedUpdate);
                    try
                    {
                        if (model.Message != null)
                        {
                            MessageProcessor(model.Message);
                        }
                        else if (model.EditedMessage != null)
                        {
                            EditedMessageProcessor(model.Message);
                        }
                        else if (model.InlineQuery != null)
                        {
                            InlineQueryProcessor(model.InlineQuery);
                        }
                        else if (model.ChosenInlineResult != null)
                        {
                            ChosenInlineResultProcessor(model.ChosenInlineResult);
                        }
                        else if (model.CallbackQuery != null)
                        {
                            CallbackQueryProcessor(model.CallbackQuery);
                        }
                        else
                        {
                            throw new InvalidDataException();
                        }
                    }
                    catch (Exception exception)
                    {
                        
                    }
                    update.Status = UpdateStatus.Processed;
                }
                entities.SaveChanges();
            }


            return 0;

        }

        private void CallbackQueryProcessor(CallbackQuery callbackQuery)
        {
            throw new NotImplementedException();
        }

        private void ChosenInlineResultProcessor(ChosenInlineResult chosenInlineResult)
        {
            throw new NotImplementedException();
        }

        private void InlineQueryProcessor(InlineQuery inlineQuery)
        {
            throw new NotImplementedException();
        }

        private void EditedMessageProcessor(Message message)
        {
            throw new NotImplementedException();
        }

        public void MessageProcessor(Message message)
        {

        }
    }
}
