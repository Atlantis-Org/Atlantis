using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Configurations;
using Followme.AspNet.Core.FastCommon.ThirdParty.DingDingTalk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public static class DingTalkConfigurationExtension
    {
        public static Configuration RegisterDingDingTalk(this Configuration configuration, string dingTalkSettingConfigName="DingTalk")
        {
            ObjectContainer.Register<IDingTalkClient, DingTalkClient>();
            configuration.Setting.SetDingDingTalkSetting(configuration.GetSetting<DingTalkSettings>(dingTalkSettingConfigName) ?? throw new ArgumentNullException("Please setting DingDingTalk!"));
            return configuration;
        }
    }

    public static class DingDingTalkConfigurationSettingExtension
    {
        private static DingTalkSettings _setting;

        public static DingTalkSettings GetDingTalkSetting(this ConfigurationSetting setting)
        {
            return _setting;
        }

        public static ConfigurationSetting SetDingDingTalkSetting(this ConfigurationSetting configurationSetting, DingTalkSettings dingTalkSetting)
        {
            _setting = dingTalkSetting;
            return configurationSetting;
        }
    }
}
