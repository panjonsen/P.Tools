using System.Net;
using System.Text;
using Newtonsoft.Json;
using P.Core.Http;
using P.Netstandard21.Http;

namespace P.Core.WeChatApi
{
	public class DtoPushMsg
	{
		public int errcode { get; set; }

		public string errmsg { get; set; }

		public string media_id { get; set; }
		public string type { get; set; }
	}

	public class WeChatApi
	{
		private string ApiBaseAddress = "https://qyapi.weixin.qq.com";
		private string ApiKey;

		public WeChatApi(string apiKey)
		{
			ApiKey = apiKey;
		}

		public bool PushMsg(string msg)
		{
			string httpBody = @$"
					{{
						""msgtype"": ""markdown"",
						""markdown"": {{
							""content"": ""{msg}""
						}}
					}}
					";

			var rsp = new PHttp().SetUrl(ApiBaseAddress + "/cgi-bin/webhook/send?key=" + ApiKey)
				   .SetMethod(HttpMethod.Post)
				   .SetDataByJsonStr(httpBody)
				   .SetScopedMode()
				   .Send()
				   .ToString2();

			var resultDto = JsonConvert.DeserializeObject<DtoPushMsg>(rsp);

			if (resultDto.errcode == 0)
			{
				return true;
			}
			return false;
		}

		public bool PushFile(byte[] fileDatas, string fileName, string fieldName)
		{
			PNHttp pNHttp = new PNHttp();
			var rsp = pNHttp.SetUrl(ApiBaseAddress + @$"/cgi-bin/webhook/upload_media?key={ApiKey}&type=file")
					.SetMethod(PNHttpMethod.Post)
					.SetFile(fileName, fileDatas, "media")
					.Send();

			//  var rsp = HttpUploadFile(fileDatas, fieldName, fileName);
			var resultDto = JsonConvert.DeserializeObject<DtoPushMsg>(rsp);

			if (resultDto == null)
			{
				return false;
			}

			if (resultDto.errcode != 0)
			{
				return false;
			}

			var httpBody = @$"{{
			    ""msgtype"": ""file"",
			    ""file"": {{
			         ""media_id"": ""{resultDto.media_id}""
			    }}
			}}";

			rsp = new PHttp().SetUrl(ApiBaseAddress + "/cgi-bin/webhook/send?key=" + ApiKey)
				  .SetMethod(HttpMethod.Post)
				  .SetDataByJsonStr(httpBody)
				  .SetScopedMode()
				  .Send()
				  .ToString2();

			resultDto = JsonConvert.DeserializeObject<DtoPushMsg>(rsp);

			if (resultDto.errcode == 0)
			{
				return true;
			}
			return false;
		}

	
	}
}