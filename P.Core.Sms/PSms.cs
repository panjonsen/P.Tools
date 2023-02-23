using System.Xml;
using P.Core.Http;

namespace P.Core.Sms
{
	/*
	 doc["returnsms"]["returnstatus"].InnerText   "Success"
	 判断成功与否
	 */

	public class PSms : ISms

	{
		private string BaseUrl = "http://124.71.157.163:8888/sms.aspx";

		private string Dj_UserId = "30253";
		private string Dj_Account = "dengjie";
		private string Dj_Password = "123456";

		/// <summary>
		/// 内置会对content Url的编码 其他的外部自行处理
		/// </summary>
		/// <param name="mobile"></param>
		/// <param name="content"></param>
		public bool Send(string mobile, string content)
		{
			//	content = WebUtility.UrlEncode(content);

			P.Core.Http.PHttp pHttp = new P.Core.Http.PHttp();

			var result = pHttp.SetUrl(BaseUrl)
				.SetMethod(HttpMethod.Post)
					.SetData("action", "send")
					.SetData("userid", Dj_UserId)
					.SetData("account", Dj_Account)
					.SetData("password", Dj_Password)
					.SetData("mobile", mobile)
					.SetData("content", content)
					.Send()
					.ToString2();

			var doc = new XmlDocument();
			doc.LoadXml(result);

			if (doc["returnsms"]?["returnstatus"]?.InnerText == "Success")
			{
				return true;
			}
			return false;
		}

		public bool CheckMsgContent(string content)
		{
			P.Core.Http.PHttp pHttp = new P.Core.Http.PHttp();
			var result = pHttp.SetUrl(BaseUrl)
			.SetMethod(HttpMethod.Post)
				.SetData("action", "checkkeyword")
				.SetData("userid", Dj_UserId)
				.SetData("account", Dj_Account)
				.SetData("password", Dj_Password)

				.SetData("content", content)
				.Send()
				.ToString2();

			var doc = new XmlDocument();
			doc.LoadXml(result);

			if (doc["returnsms"]?["returnstatus"]?.InnerText == "Success")
			{
				return true;
			}
			return false;
		}
	}
}