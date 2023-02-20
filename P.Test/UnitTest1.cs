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
			//����
			var result = new PHttp().SetUrl("https://www.baidu.com/")
				.SetMethod(HttpMethod.Get)
				.Send()
				.ToString2();
			Console.WriteLine(result);

			//����
			result = new PHttp().SetUrl("https://www.baidu.com/")
				.SetMethod(HttpMethod.Get)
				.SetTimeOutSecond(50)
				.Send()
				.ToString2();
			Console.WriteLine(result);

			//�쳣  TimeOut ��Ķ���HtttpClientʵ��  �����Ҫ������ Scopedģʽ
			result = new PHttp().SetUrl("https://www.baidu.com/")
				.SetMethod(HttpMethod.Get)
				.SetTimeOutSecond(50)
				.Send()
				.ToString2();
			Console.WriteLine(result);

			//����  Scoped�ᴴ���µ�HtttpClient
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

			weChatApi.PushFile(Encoding.Default.GetBytes("���"), "123.txt", "media");
		}

		[Test]
		public void HttpN21Test()
		{
			PNHttp npg = new PNHttp();

			npg.SetUrl("http://192.168.1.131:5043/api/Test/TestPost2")
			  .SetMethod(PNHttpMethod.Post)
			  .SetData("asdsad", "asfsafsaf")
			  .SetData("xzcxzc", "12321342131")
			  .Send();
		}
	}
}