using System;

namespace Followme.Atlantis.ThirdParty.Redis
{
	public class CacheOptions
	{
		public ExpireType ExpireType { get; set; }

		public DateTime Expiry { get; set; }
	}

	public enum ExpireType
	{
		/// <summary>
		/// 永不过期
		/// </summary>
		Nerver = -1,
		/// <summary>
		/// 相对
		/// </summary>
		Relative = 0,
		/// <summary>
		/// 滑动
		/// </summary>
		Silibing = 1
	}
}