using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace P.Netstandard21.Http
{
	public enum PNHttpMethod
	{
		Get,
		Post,
		Put,
		Delete,
	}

	public class PNHttpProxy
	{
		public string Ip { get; set; }

		public string Pory { get; set; }
		public string Username { get; set; }

		public string Password { get; set; }
	}

	public class PNHttpOption
	{
		public string Url { get; set; }

		public PNHttpMethod Method { get; set; }

		public PNHttpProxy? Proxy { get; set; }

		public Dictionary<string, string> DicData { get; set; } = new Dictionary<string, string>();

		public Dictionary<string, string> DicHeaders { get; set; } = new Dictionary<string, string>();

		public string? DataByJsonStr { get; set; }
	}

	public class PNHttp
	{
		private PNHttpOption _PNHttpOption = new PNHttpOption();

		public PNHttp SetUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new System.Exception("url无效");
			}
			_PNHttpOption.Url = url;

			return this;
		}

		public PNHttp SetMethod(PNHttpMethod method)
		{
			_PNHttpOption.Method = method;

			return this;
		}

		/// <summary>
		/// 重复添加key后面覆盖前面的值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public PNHttp SetData(string key, string value)
		{
			if (_PNHttpOption.DicData.ContainsKey(key))
			{
				_PNHttpOption.DicData[key] = value;
			}
			else
			{
				_PNHttpOption.DicData.Add(key, value);
			}

			return this;
		}

		public PNHttp SetHeader(string key, string value)
		{
			if (_PNHttpOption.DicHeaders.ContainsKey(key))
			{
				_PNHttpOption.DicHeaders[key] = value;
			}
			else
			{
				_PNHttpOption.DicHeaders.Add(key, value);
			}
			return this;
		}

		public string Send()
		{
			var request = GetData();

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream);
			string responseText = reader.ReadToEnd();
			// 处理响应内容

			Console.WriteLine(responseText);

			return responseText;
		}

		private HttpWebRequest HeadersAdd(HttpWebRequest request)
		{
			foreach (var DicHeader in _PNHttpOption.DicHeaders)
			{
				switch (DicHeader.Key)
				{
					case "Content-Type":
						request.ContentType = DicHeader.Value;

						break;

					case "User-Agent":
						request.UserAgent = DicHeader.Value;

						break;

					case "Referer":
						request.Referer = DicHeader.Value;

						break;

					case "Host":
						request.Host = DicHeader.Value;

						break;

					default:
						if (request.Headers.AllKeys.Count(m => m == DicHeader.Key) > 0)
						{
							request.Headers.Remove(DicHeader.Key);
						}

						request.Headers.Add(DicHeader.Key, DicHeader.Value);
						break;
				}
			}

			return request;
		}

		private HttpWebRequest PostDataAdd(HttpWebRequest request)
		{
			using (var writer = new StreamWriter(request.GetRequestStream()))
			{
				var postData = "";
				foreach (var item in _PNHttpOption.DicData)
				{
					if (postData == "")
					{
						postData = $@"{item.Key}={item.Value}";
					}
					else
					{
						postData += $@"&{item.Key}={item.Value}";
					}
				}
				writer.Write(postData);
			}

			return request;
		}

		private HttpWebRequest GetData()
		{
			//处理url
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_PNHttpOption.Url);

			//默认类型
			request.ContentType = "application/x-www-form-urlencoded";
			//随机内置一个
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36 Core/1.47.277.400 QQBrowser/9.4.7658.400";
			//处理Method
			switch (_PNHttpOption.Method)
			{
				case PNHttpMethod.Get:
					request.Method = "GET";
					break;

				case PNHttpMethod.Post:
					request.Method = "POST";
					request = HeadersAdd(request);
					break;

				case PNHttpMethod.Put:
					request.Method = "PUT";
					request = HeadersAdd(request);
					break;

				case PNHttpMethod.Delete:
					request.Method = "DELETE";
					request = HeadersAdd(request);
					break;

				default:
					break;
			}
			request = PostDataAdd(request);
			return request;
		}
	}
}