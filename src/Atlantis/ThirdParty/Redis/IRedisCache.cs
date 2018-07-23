using System;
using System.Threading.Tasks;

namespace Followme.Atlantis.ThirdParty.Redis
{
	public interface IRedisCache
    {
		Task<T> TryGetAsync<T>(string key);

		/// <summary>
		/// Get the cache value and delete the value in redis
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		Task<T> TryTakeAsync<T>(string key);


		Task<bool> TryRemoveAsync(string key);

		/// <summary>
		///  Never expire
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		Task<bool> TrySetAsync<T>(string key, T value);

		Task<bool> TrySetExpireAsync<T>(string key, T value, DateTime absoluteExpiredTime);

		Task<bool> TrySetExpireAsync<T>(string key, T value, long silidingTime);

		Task<bool> TrySetExpireAsync<T>(string key, T value, TimeSpan? releativeTimeSpan = default(TimeSpan?));

		Task ClearCacheAsync();


	}

}
