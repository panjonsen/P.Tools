using System.Text;
using P.Core.Http;
using P.Core.WeChatApi;
using P.Netstandard21.Http;

namespace P.Test
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void HttpTest()
		{
			//正常
			var result = new PHttp().SetUrl("https://www.baidu.com/")
				.SetMethod(HttpMethod.Get)
				.Send()
				.ToString2();
			Console.WriteLine(result);

			//正常
			result = new PHttp().SetUrl("https://www.baidu.com/")
				.SetMethod(HttpMethod.Get)
				.SetTimeOutSecond(50)
				.Send()
				.ToString2();
			Console.WriteLine(result);

			//异常  TimeOut 会改动到HtttpClient实例  如果需要请设置 Scoped模式
			result = new PHttp().SetUrl("https://www.baidu.com/")
				.SetMethod(HttpMethod.Get)
				.SetTimeOutSecond(50)
				.Send()
				.ToString2();
			Console.WriteLine(result);

			//正常  Scoped会创建新的HtttpClient
			result = new PHttp().SetUrl("https://www.baidu.com/")
			.SetMethod(HttpMethod.Get)
			.SetTimeOutSecond(50)
			.SetScopedMode()
			.Send()
			.ToString2();
			Console.WriteLine(result);
		}

		[Test]
		public void WeChatApiTestAsync()
		{
			WeChatApi weChatApi = new WeChatApi("b00d1b8d-03b3-49d7-b5e9-1ffa194ebfd8");

			//	weChatApi.PushMsg("asdasdasd");

			weChatApi.PushFile(Encoding.Default.GetBytes("你好"), "123.txt", "media");
		}

		[Test]
		public void HttpN21Test()
		{
			PNHttp npg = new PNHttp();

			npg.SetUrl("http://localhost:5119/api/HttpService/TestPost")
			  .SetMethod(PNHttpMethod.Post)
			  .SetFile("asdsad.txt", Encoding.UTF8.GetBytes("你好"), "media")
			  .SetFile("fffff.txt", Encoding.UTF8.GetBytes("bbbbbbb"), "aaaaa")
			  .SetData("asdasdd", "asdaf")
			  .Send();

			npg.SetUrl("https://qyapi.weixin.qq.com/cgi-bin/webhook/upload_media?key=b00d1b8d-03b3-49d7-b5e9-1ffa194ebfd8&type=file")
			  .SetMethod(PNHttpMethod.Post)
			  .SetFile("asdsad.txt", Encoding.UTF8.GetBytes("你好"), "media")

			  .Send();

			//
		}
	}
}