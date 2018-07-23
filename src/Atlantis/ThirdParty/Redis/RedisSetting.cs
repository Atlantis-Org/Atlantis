using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.Atlantis.ThirdParty.Redis
{
	public class RedisSetting
	{
		public string Host { get; set; }

		public int Port { get; set; }

		public string Password { get; set; }

		public bool IsAllowAdmin { get; set; }
	}
}
