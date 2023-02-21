using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace P.Netstandard21.Http
{
	public enum PNHttpParament
	{
		None = 0,
		JsonStr = 1,
		Multipart = 2
	}

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

		public Dictionary<string, string> DicQuery { get; set; } = new Dictionary<string, string>();

		public Dictionary<string, Dictionary<string, object>> DicFiles = new Dictionary<string, Dictionary<string, object>>();

		public string? DataByJsonStr { get; set; }

		/// <summary>
		/// 请求类型 默认application/x-www-form-urlencoded =none
		/// </summary>
		public PNHttpParament ParamentType { get; set; }

		/// <summary>
		/// 表单提交可以自定义用
		/// </summary>
		public string Boundary { get; set; }
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
		public PNHttp SetQuery(string key, string value)
		{
			if (_PNHttpOption.DicQuery.ContainsKey(key))
			{
				_PNHttpOption.DicQuery[key] = value;
			}
			else
			{
				_PNHttpOption.DicQuery.Add(key, value);
			}

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

		/// <summary>
		/// 直接json字符串提交
		/// </summary>
		/// <param name="jsonStr"></param>
		/// <returns></returns>
		public PNHttp SetDataByJsonStr(string jsonStr)
		{
			_PNHttpOption.DataByJsonStr = jsonStr;
			_PNHttpOption.ParamentType = PNHttpParament.JsonStr;
			SetHeader("Content-Type", "application/json");
			return this;
		}

		/// <summary>
		/// 后面覆盖前面
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 同名fieldName 后面覆盖前面
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="fileDatas"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public PNHttp SetFile(string fileName, byte[] fileDatas, string fieldName)
		{
			if (_PNHttpOption.DicFiles.ContainsKey(fieldName))
			{
				_PNHttpOption.DicFiles[fieldName] = new Dictionary<string, object>() { { "FileName", fileName }, { "FileData", fileDatas } };
			}
			else
			{
				_PNHttpOption.DicFiles.Add(fieldName, new Dictionary<string, object>() { { "FileName", fileName }, { "FileData", fileDatas } });
			}
			_PNHttpOption.ParamentType = PNHttpParament.Multipart;

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
			if (_PNHttpOption.ParamentType == PNHttpParament.None)
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
			}

			if (_PNHttpOption.ParamentType == PNHttpParament.JsonStr)
			{
				byte[] data = Encoding.UTF8.GetBytes(_PNHttpOption.DataByJsonStr);
				using (Stream stream = request.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}
			}
			if (_PNHttpOption.ParamentType == PNHttpParament.Multipart)
			{
				//设置 Boundary
				string boundary = _PNHttpOption.Boundary;
				if (string.IsNullOrEmpty(boundary))
				{
					boundary = "---------------------------" + Guid.NewGuid().ToString("N");
				}
				SetHeader("Content-Type", @$"multipart/form-data; boundary={boundary}");

				using (Stream stream = request.GetRequestStream())
				{
					// 写入表单数据
					foreach (var item in _PNHttpOption.DicData)
					{
						var data = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
						stream.Write(data, 0, data.Length);
						data = Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"{item.Key}\"\r\n\r\n");
						stream.Write(data, 0, data.Length);
						data = Encoding.UTF8.GetBytes($"{item.Value}\r\n");
						stream.Write(data, 0, data.Length);
					}

					//文件处理
					foreach (var item in _PNHttpOption.DicFiles)
					{
						var data = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
						stream.Write(data, 0, data.Length);
						data = Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"{item.Key}\"; filename=\"{item.Value["FileName"]}\"\r\n");
						stream.Write(data, 0, data.Length);
						data = Encoding.UTF8.GetBytes("Content-Type: application/octet-stream\r\n\r\n");
						stream.Write(data, 0, data.Length);
						stream.Write((byte[])item.Value["FileData"], 0, ((byte[])item.Value["FileData"]).Length);
						data = Encoding.UTF8.GetBytes("\r\n");
						stream.Write(data, 0, data.Length);
					}

					// 写入结束标志
					var endData = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
					stream.Write(endData, 0, endData.Length);
				}
			}
			return request;
		}

		private string GetDataAdd()
		{
			var postData = "";
			foreach (var item in _PNHttpOption.DicQuery)
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
			if (postData != "")
			{
				if (_PNHttpOption.Url[_PNHttpOption.Url.Length - 1] != '?')
				{
					_PNHttpOption.Url += "?";
				}
			}

			return postData;
		}

		private HttpWebRequest GetData()
		{
			var query = GetDataAdd();
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_PNHttpOption.Url + query);
			Console.WriteLine($@"  request.ContentType:{request.ContentType}");
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

					request = PostDataAdd(request);
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

			return request;
		}
	}
}