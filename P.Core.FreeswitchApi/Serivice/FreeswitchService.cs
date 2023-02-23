using System.Text;
using Newtonsoft.Json;
using P.Core.FreeswitchApi.DtoModel;
using P.Core.Http;

namespace P.Core.FreeswitchApi.Serivice
{
	public class FreeswitchOption
	{
		public string Url { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string AuthBase64 { get; set; }
	}

	public class FreeswitchService
	{
		private FreeswitchOption _FreeswitchOption = new FreeswitchOption();

		public FreeswitchService(string url, string username, string password)
		{
			_FreeswitchOption.Url = url;
			_FreeswitchOption.Username = username;
			_FreeswitchOption.Password = password;
			_FreeswitchOption.AuthBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($@"{_FreeswitchOption.Username}:{_FreeswitchOption.Password}"));
		}

		private string FuncCall(string funcName, string funcParameter)
		{
			funcParameter = Uri.EscapeDataString(funcParameter);

			PHttp pHttp = new PHttp();

			return pHttp.SetScopedMode()
					.SetUrl(@$"{_FreeswitchOption.Url}/webapi/{funcName}?{funcParameter}")
					.SetMethod(HttpMethod.Get)
					.SetHeaders("Authorization", @$"Basic  {_FreeswitchOption.AuthBase64}")
					.Send()
					.ToString2();
		}

		/// <summary>
		/// 返回json字符串
		/// </summary>
		/// <param name="rsp"></param>
		/// <returns></returns>
		private string GetJsonObjectStr(string rsp)
		{
			var subStr = "cellspacing=4 border=1>";

			var strat_index = rsp.IndexOf(subStr);
			if (strat_index > -1)
			{
				rsp = rsp.Substring(strat_index + subStr.Length, rsp.Length - strat_index - subStr.Length);
			}

			return rsp.Trim();
		}

		/// <summary>
		/// 通话
		/// </summary>
		/// <returns></returns>
		public string ShowCalls()
		{
			return GetJsonObjectStr(FuncCall("show", "calls as json"));
		}

		/// <summary>
		/// 通话通道
		/// </summary>
		/// <returns></returns>
		public string ShowChannels()
		{
			return GetJsonObjectStr(FuncCall("show", "channels as json"));
		}

		/// <summary>
		/// 发起呼叫
		/// </summary>
		/// <param name="gateway">线路网关</param>
		/// <param name="callNumber">线路主叫号码</param>
		/// <param name="descNumber">接听号码</param>
		/// <param name="answerExtension">转接坐席</param>
		/// <returns></returns>
		public string Call(string gateway, string callNumber, string descNumber, string answerExtension)
		{
			var rsp = FuncCall("originate", @$"{{origination_caller_id_number={callNumber},hangup_after_bridge=true,ignore_early_media=true}}sofia/gateway/{gateway}/{descNumber} &bridge(sofia/internal/{answerExtension})");

			return GetJsonObjectStr(rsp);
		}

		/// <summary>
		/// 发起呼叫
		/// </summary>
		/// <param name="gateway">线路网关</param>
		/// <param name="callNumber">线路主叫号码</param>
		/// <param name="descNumber">接听号码</param>
		/// <returns></returns>
		public string Call(string gateway, string callNumber, string descNumber)
		{
			var rsp = FuncCall("originate", @$"{{origination_caller_id_number={callNumber},hangup_after_bridge=true,ignore_early_media=true}}sofia/gateway/{gateway}/{descNumber} &echo)");

			return GetJsonObjectStr(rsp);
		}

		/// <summary>
		/// 发起呼叫 内线
		/// </summary>
		/// <param name="callExtension">主叫坐席  需完整 xxxxxxxxxxx@huihu.zjfantian.cn  不需要端口补充</param>
		/// <param name="descExtension">被叫坐席   1001</param>
		/// <returns></returns>
		public string Call(string callExtension, string descExtension)
		{
			var rsp = FuncCall("originate", @$"{{origination_caller_id_number=HelloWord}}user/{callExtension} {descExtension}");

			return GetJsonObjectStr(rsp);
		}

		/// <summary>
		/// 发起呼叫并播放指定语音
		/// </summary>
		/// <param name="gateway">线路网关</param>
		/// <param name="callNumber">线路主叫号码</param>
		/// <param name="descNumber">接听号码</param>
		/// <param name="mp3Path">本地绝对路径或可以公网访问的地址</param>
		/// <param name="look">true=循环播放 false=单次播放</param>
		/// <returns></returns>
		public string CallByPlayBack(string gateway, string callNumber, string descNumber, string mp3Path, bool look)
		{
			string playType = "playback";

			if (look)
			{
				playType = "endless_playback";
			}

			var rsp = FuncCall("originate", @$"{{origination_caller_id_number={callNumber},hangup_after_bridge=true,ignore_early_media=true}}sofia/gateway/{gateway}/{descNumber} &{playType}({mp3Path})");

			return GetJsonObjectStr(rsp);
		}

		/// <summary>
		/// 发起呼叫并播放指定语音
		/// </summary>
		/// <param name="gateway">线路网关</param>
		/// <param name="callNumber">线路主叫号码</param>
		/// <param name="descNumber">接听号码</param>
		/// <param name="mp3Path">本地绝对路径或可以公网访问的地址</param>
		/// <param name="look">true=循环播放 false=单次播放</param>
		/// <returns></returns>
		public string CallByPlayBack(string gateway, string callNumber, string descNumber, string mp3Path, int playCount)
		{
			string mp3Paths = "";
			for (int i = 0; i < playCount; i++)
			{
				if (mp3Paths == "")
				{
					mp3Paths = mp3Path;
				}
				else
				{
					mp3Paths += "!" + mp3Path;
				}
			}
			mp3Paths = "file_string://" + mp3Paths;

			var rsp = FuncCall("originate", @$"{{origination_caller_id_number={callNumber},hangup_after_bridge=true,ignore_early_media=true}}sofia/gateway/{gateway}/{descNumber} &playback({mp3Paths})");

			return GetJsonObjectStr(rsp);
		}

		/// <summary>
		/// 获取指定号码正在通话中的集合
		/// </summary>
		/// <returns></returns>
		public List<DtoChannelsData> GetActiveCallObjectsByNumber(string mobile)
		{
			string channels = ShowChannels();

			var channelsStr = GetJsonObjectStr(channels);

			var apiResult = JsonConvert.DeserializeObject<DtoChannels>(channelsStr);

			var list = apiResult.rows
					.Where(m => m.direction == "outbound")

					.Where(m => m.cid_num == mobile)
					.ToList();

			return list;
		}

		/// <summary>
		/// 监听通话  监听人无法讲话
		/// </summary>
		/// <param name="account">监听坐席  用来听某个通话的坐席  xxxxxxx@huihu.zjfantian.cn:56001</param>
		/// <param name="callId">要监听的通话Id  </param>
		public string Eavesdrop(string account, string callId)
		{
			var rsp = FuncCall(funcName: "originate", @$"sofia/internal/{account} &eavesdrop({callId})");

			return GetJsonObjectStr(rsp);
		}

		/// <summary>
		/// 挂断所有通话  hupall命令
		/// </summary>
		public string Hupall()
		{
			var rsp = FuncCall("hupall", "");

			return GetJsonObjectStr(rsp);
		}

		/// <summary>
		/// 挂断指定通话 uuid_kill命令
		/// </summary>
		public string HupallByCallId(string callId)
		{
			var rsp = FuncCall("uuid_kill", callId);

			return GetJsonObjectStr(rsp);
		}
	}
}