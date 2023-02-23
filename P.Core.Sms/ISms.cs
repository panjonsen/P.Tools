namespace P.Core.Sms
{
	internal interface ISms
	{
		public  bool Send(string mobile, string content);

		public   bool CheckMsgContent(string content);
	}
}