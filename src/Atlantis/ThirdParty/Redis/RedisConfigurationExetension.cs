using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Configurations;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Utilities;
using Followme.Atlantis.ThirdParty.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
	public static class RedisConfigurationExtension
	{
		private static RedisSetting _redisSetting;

		public static Configuration UseRedisCache(this Configuration configuration, string redisConfigName = "Redis")
		{
			ObjectContainer.Register<IRedisCache, RedisCache>();
			configuration.Setting.SetRedisSetting(configuration.GetSetting<RedisSetting>(redisConfigName)?? throw new ArgumentNullException("The redis setting cannot be null!"));

			return configuration;
		}
		
	}

	public static class RedisConfigurationSettingExtension
	{
		private static RedisSetting _redisSetting;

		public static RedisSetting GetRedisSetting(this ConfigurationSetting setting)
		{
			return _redisSetting;
		}

		public static ConfigurationSetting SetRedisSetting(this ConfigurationSetting configurationSetting, RedisSetting redisSetting)
		{
			_redisSetting = redisSetting;
			return configurationSetting;
		}

	}
}
