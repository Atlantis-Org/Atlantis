using System;
using System.Collections.Generic;
using System.Text;
using Followme.AspNet.Core.FastCommon.ThirdParty.DingDingTalk.Notification;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Configurations;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Serializing;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.DingDingTalk
{
    public class DingTalkClient : IDingTalkClient
    {
        private const int Error_DingTalk_Exception = 500;
        private readonly DingTalkSettings DingTalkSettings;
        private readonly ILogger _logger;
        private readonly IJsonSerializer _jsonSerialzer;

        public DingTalkClient(ILoggerFactory loggerFactory,
                              IJsonSerializer jsonSerialzer)
        {
            DingTalkSettings = ObjectContainer.Resolve<Configuration>().Setting.GetDingTalkSetting();
            _logger = loggerFactory.Create<DingTalkClient>();
            _jsonSerialzer = jsonSerialzer;
        }

        public DingTalkResult SentNotification(IDingTalkNotification notification)
        {
            try
            {
                var uri = $"{DingTalkSettings.ServerUrl}/send?access_token={DingTalkSettings.ReceiverToken}";
                var jsonStr = JsonConvert.SerializeObject(notification);
                var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                using (var _httpClient = new HttpClient())
                {
                    var response = _httpClient.PostAsync(uri, content).Result;
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    var result = _jsonSerialzer.Deserialize<DingTalkResult>(jsonResult);
                    if (!result.IsSucceed())
                    {
                        _logger.Error($"发送钉钉消息失败，原因:{result.ErrMsg}");
                    }
                    else
                    {
                        _logger.Info($"发送钉钉消息成功.消息内容:{jsonStr}");
                    }

                    return result;
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"发送钉钉消息异常，原因：{ex.ToString()}");
                return new DingTalkResult(Error_DingTalk_Exception, ex.ToString());
            }
        }

        public async Task<DingTalkResult> SentNotificationAsync(IDingTalkNotification notification)
        {
            return await Task.Run(() => SentNotification(notification));
        }
    }
}
