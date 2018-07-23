using Followme.AspNet.Core.FastCommon.Components;
using Followme.AspNet.Core.FastCommon.Configurations;
using Followme.AspNet.Core.FastCommon.Logging;
using Followme.AspNet.Core.FastCommon.Serializing;
using Followme.AspNet.Core.FastCommon.Utilities;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Followme.Atlantis.ThirdParty.Redis
{
	public class RedisCache : IRedisCache, IDisposable
	{
		private readonly RedisSetting _redisSetting;
		private readonly ILogger _logger;
		private readonly IJsonSerializer _jsonSerializer;
		private readonly Configuration _configuration;

		private IDatabase _cache;
		private ConnectionMultiplexer _connectionMultiplexer;
		private static readonly object _locker = new object();

		private readonly int _defaultExpireTime = 30;//单位：分钟
		private readonly int _redisExpireAddtionalTime = 30;//单位：分钟

		private IDatabase Cache
		{
			get
			{
				if (_cache == null) Init();
				return _cache;
			}
		}

		public RedisCache(ILoggerFactory loggerFactory, IJsonSerializer jsonSerializer)
		{
			_redisSetting = ObjectContainer.Resolve<Configuration>().Setting.GetRedisSetting();
			_logger = loggerFactory.Create<RedisCache>();
			_jsonSerializer = jsonSerializer;
		}

		

		private void Init()
		{
			if (_connectionMultiplexer == null)
			{
				lock (_locker)
				{
					if (_connectionMultiplexer == null || !_connectionMultiplexer.IsConnected)
					{
						_connectionMultiplexer = ConnectionMultiplexer.Connect(GetConfiguration());
						BindEvent(_connectionMultiplexer);

						_cache = _connectionMultiplexer.GetDatabase();
					}
				}
			}
		}

		private ConfigurationOptions GetConfiguration()
		{			
			return new ConfigurationOptions()
			{
				EndPoints =
				{
					{_redisSetting.Host,_redisSetting.Port}
				},
				AllowAdmin = _redisSetting.IsAllowAdmin,
				Password = _redisSetting.Password
			};
		}

		#region Event

		private ConnectionMultiplexer BindEvent(ConnectionMultiplexer connection)
		{
			connection.ConnectionFailed += MultiplexerConnectionFailed;
			connection.ConnectionRestored += MultiplexerConnectionRestored;
			connection.ErrorMessage += MultiplexerErrorMessage;
			connection.ConfigurationChanged += MultiplexerConfigurationChanged;
			connection.HashSlotMoved += MultiplexerHashSlotMoved;
			connection.InternalError += MultiplexerInternalError;

			return connection;
		}

		private void MultiplexerInternalError(object sender, InternalErrorEventArgs e)
		{
			_logger.Error("InternalError:Message" + e.Exception.Message);
		}

		private void MultiplexerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
		{
			_logger.Warn("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
		}

		private void MultiplexerConfigurationChanged(object sender, EndPointEventArgs e)
		{
			_logger.Warn("Configuration changed: " + e.EndPoint);
		}

		private void MultiplexerErrorMessage(object sender, RedisErrorEventArgs e)
		{
			_logger.Error("ErrorMessage: " + e.Message);
		}

		private void MultiplexerConnectionRestored(object sender, ConnectionFailedEventArgs e)
		{
			_logger.Error("ConnectionRestored: " + e.EndPoint);
		}

		private void MultiplexerConnectionFailed(object sender, ConnectionFailedEventArgs e)
		{
			_logger.Error("Connection restored failed ：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
		}
		#endregion



		public async Task<T> TryGetAsync<T>(string key)
		{
			Ensure.NotNullOrWhiteSpace(key, "key can't be null！");
			RedisValue redisValue = await Cache.StringGetAsync(key);
			if (redisValue.HasValue && redisValue != "{}")
			{
				var redisVal = default(T);
				CheckKeyAndValue(key, redisVal);
				return redisVal;
			}
			return _jsonSerializer.Deserialize<T>(redisValue);
		}

		public async Task<T> TryTakeAsync<T>(string key)
		{
			Ensure.NotNullOrWhiteSpace(key, "key can't be null！");
			var value = await TryGetAsync<T>(key);
			if (value == null) return default(T);

			await Cache.KeyDeleteAsync(key);
			return value;
		}

		public async Task<bool> TryRemoveAsync(string key)
		{
			return await Cache.KeyDeleteAsync(key);
		}

		public async Task ClearCacheAsync()
		{
			Init();
			EndPoint[] endPoints = _connectionMultiplexer.GetEndPoints();
			foreach (EndPoint point in endPoints)
			{
				IServer server = _connectionMultiplexer.GetServer(point);
				await server.FlushDatabaseAsync();
			}
		}

		public async Task<bool> TrySetAsync<T>(string key, T value)
		{
			string valueStr = CheckKeyAndValue(key, value);
			return await Cache.StringSetAsync(key, valueStr, null, When.NotExists, CommandFlags.FireAndForget);
		}

		public async Task<bool> TrySetExpireAsync<T>(string key, T value, DateTime absoluteExpiredTime)
		{
			var cacheOption = new CacheOptions
			{
				Expiry = absoluteExpiredTime,
				ExpireType = ExpireType.Relative
			};

			return await SetValueAsync(key, value, cacheOption);
		}

		public async Task<bool> TrySetExpireAsync<T>(string key, T value, long silidingTime)
		{
			var cacheOption = new CacheOptions
			{
				Expiry = DateTime.Now.AddMinutes(silidingTime),
				ExpireType = ExpireType.Silibing
			};

			return await SetValueAsync(key, value, cacheOption);
		}

		public async Task<bool> TrySetExpireAsync<T>(string key, T value, TimeSpan? releativeTimeSpan = default(TimeSpan?))
		{
			var cacheOption = new CacheOptions
			{
				Expiry = releativeTimeSpan.HasValue ? DateTime.Now.Add(releativeTimeSpan.Value) : DateTime.Now.AddMinutes(_defaultExpireTime),
				ExpireType = ExpireType.Relative
			};
			return await SetValueAsync(key, value, cacheOption);
		}

		private async Task<bool> SetValueAsync<T>(string key, T value, CacheOptions cacheOption)
		{
			bool result = false;
			try
			{
				var valueStr = CheckKeyAndValue(key, value);
				var redisExpireTime = cacheOption.Expiry.AddMinutes(_redisExpireAddtionalTime);

				// 存储协议：第一位（1 代表滑动过期） 第二位（逻辑过期时间 时间戳）第三位 数据位 第四位 逻辑过期标志（1代表过期）

				await Cache.KeyDeleteAsync(key);

				var times = cacheOption.Expiry.Ticks;
				if (cacheOption.ExpireType == ExpireType.Silibing)
					times = times - DateTime.UtcNow.Ticks;

				await Cache.ListRightPushAsync(key, new RedisValue[4] { (int)cacheOption.ExpireType, times, valueStr, 0 });

				result = await Cache.KeyExpireAsync(key, redisExpireTime);

			}
			catch (Exception ex)
			{
				_logger.Error($"Redis cache write operation error！key={key},value={_jsonSerializer.Serialize(value)}, expire type：{cacheOption.ExpireType}", ex);
			}
			return result;
		}

		private string CheckKeyAndValue<T>(string key, T value)
		{
			Ensure.NotNull(key, "key can't be null！");
			if (value == null) return null;
			if (value.GetType() == typeof(string)) return value.ToString();

			var valueStr = _jsonSerializer.Serialize(value);
			if (string.IsNullOrWhiteSpace(valueStr)) return null;

			return valueStr;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_connectionMultiplexer != null)
				{
					_connectionMultiplexer.Close();
					_connectionMultiplexer.Dispose();
				}

				_cache = null;
			}
		}

	}



}
