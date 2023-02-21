using System.Text;
using Newtonsoft.Json;
using P.Core.Http;

namespace P.Core.Ip
{
	public static class PIp
	{
		/// <summary>
		/// 失败返回null
		/// </summary>
		/// <returns></returns>
		public static string? GetCurrentIp()
		{
			PHttp pHttpHelper = new();

			var rsp = pHttpHelper.SetUrl("https://api.ipify.org/?format=json")
				.SetMethod(HttpMethod.Get)
				.SetScopedMode()
				.Send();

			using (var steam = new StreamReader(rsp.Content.ReadAsStream(), Encoding.UTF8))
			{
				var rspStr = steam.ReadToEnd();

				var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(rspStr);

				if (dic.ContainsKey("ip"))
				{
					return dic["ip"];
				}
				else
				{
					return null;
				}
			}
		}
	}
}