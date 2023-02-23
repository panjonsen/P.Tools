using P.Core.Config;

namespace T.Console
{
	internal class Program
	{

		private static void Main(string[] args)
		{



			Thread workerThread = new Thread(Taska);
			workerThread.Start();

			System.Console.ReadKey();
		}

		public static void Taska()
		{
			PConfig pConfigv = new($@"miliao1.config");
			pConfigv.Add("你好", "阿萨德撒");

			pConfigv.Add("你1好", "zxczc");
			pConfigv.Add("你2好", "zxczc");


			pConfigv.Update("你1好", "123");

			pConfigv.Delete("你2好");


			while (true)
			{
				System.Console.WriteLine(pConfigv.Get<bool>("Debug"));

				Thread.Sleep(1000);
			}
		}
	}
}