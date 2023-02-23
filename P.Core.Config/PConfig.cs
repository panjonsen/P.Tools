using System.Xml;

namespace P.Core.Config
{
	public class PConfig
	{

		private XmlDocument xmlDoc = new XmlDocument();
		private string _FilePath = "";
		private static object looks = new object();

		/// <summary>
		/// 文件不存在会自动创建
		/// </summary>
		/// <param name="filePath">绝对路径 例如  D:\csharp\linkcorenet6\appxxx.config</param>
		public PConfig(string filePath)
		{
			_FilePath = filePath;

			xmlDoc.Load(_FilePath);
		}

		public void Update(string key, string value)
		{
			lock (looks)
			{
				xmlDoc.Load(_FilePath);
				if (xmlDoc["Configuration"] == null)
				{
					throw new Exception(@$"配置存在错误,请修改后再试");
				}

				if (xmlDoc["Configuration"][key] == null)
				{
					throw new Exception(@$"配置存在错误,请修改后再试");
				}

				string data = xmlDoc["Configuration"][key].Attributes["value"].Value = value;
				xmlDoc.Save(_FilePath);
			}
		}

		public bool Add(string key, string value)
		{
			lock (looks)
			{
				xmlDoc.Load(_FilePath);
				if (IsExist(key))
				{
					return false;
				}
				XmlElement newNode = xmlDoc.CreateElement(key);
				newNode.SetAttribute("value", value);

				xmlDoc["Configuration"].AppendChild(newNode);
				xmlDoc.Save(_FilePath);

				return true;
			}
		}

		public void Delete(string key)
		{
			lock (looks)
			{
				xmlDoc.Load(_FilePath);
				if (xmlDoc["Configuration"] == null)
				{
					throw new Exception(@$"配置存在错误,请修改后再试");
				}

				if (xmlDoc["Configuration"][key] == null)
				{
					throw new Exception(@$"配置存在错误,请修改后再试");
				}

				xmlDoc["Configuration"].RemoveChild(xmlDoc["Configuration"][key]);
				xmlDoc.Save(_FilePath);
			}
		}

		/// <summary>
		/// 不确定存在的键  请先通过IsExist检查
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public T Get<T>(string key)
		{
			lock (looks)
			{
				xmlDoc.Load(_FilePath);
				if (xmlDoc["Configuration"] == null)
				{
					throw new Exception(@$"配置存在错误,请修改后再试");
				}

				if (xmlDoc["Configuration"][key] == null)
				{
					throw new Exception(@$"配置存在错误,请修改后再试");
				}

				string data = xmlDoc["Configuration"][key].Attributes["value"].Value;

				try
				{
					T value = (T)Convert.ChangeType(data, typeof(T));
					return value;
				}
				catch (System.Exception ex)
				{
					throw ex;
				}
			}
		}

		public bool IsExist(string key)
		{
			lock (looks)
			{
				xmlDoc.Load(_FilePath);
				if (xmlDoc["Configuration"] == null)
				{
					throw new Exception(@$"配置存在错误,请修改后再试");
				}

				if (xmlDoc["Configuration"][key] == null)
				{
					return false;
				}
				return true;
			}
		}
	}
}