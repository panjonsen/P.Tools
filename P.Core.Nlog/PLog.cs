using NLog;

namespace P.Core.Nlog
{
	/// <summary>
	/// 如果运行目录下没有配置文件将生成默认的  否则使用当前已存在的
	/// </summary>
	public class PLog
	{
		//默认加载当前目录下的nlog.config
		private Logger logger;

		public PLog() 
		{
			if (!File.Exists(Environment.CurrentDirectory + "/" + "nlog.config"))
			{
				File.WriteAllText(Environment.CurrentDirectory + "/" + "nlog.config", Resource1.nlog);
			}

			logger = LogManager.GetCurrentClassLogger();
		}

		public PLog(string path)
		{
			if (!File.Exists(path))
			{
				File.WriteAllText(path, Resource1.nlog);
			}

			logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();

			//logger = NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();
		}

		/// <summary>
		/// 改变路径加载配置
		/// </summary>
		/// <param name="path"></param>
		public void N_SetPath(string path)
		{
			LogManager.LoadConfiguration(path);
		}

		//返回日记对象
		public Logger N_GetLogger()
		{
			return logger;
		}

		public void Debug(string msg)
		{
			logger.Debug(msg);
		}

		public void Error(string msg)
		{
			logger.Error(msg);
		}

		public void Info(string msg)
		{
			logger.Info(msg);
		}

		public void Trace(string msg)
		{
			logger.Trace(msg);
		}

		public void Warn(string msg)
		{
			logger.Warn(msg);
		}

		public void Fatal(string msg)
		{
			logger.Fatal(msg);
		}
	}
}