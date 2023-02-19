using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace P.Http
{
    public enum ParameterMode
    {
        Normal,
        JsonStr,
        Json,
        Multipart
    }

    public class HttpOption
    {
        /// <summary>
        /// json字符串直接请求体
        /// </summary>
        public string DataByJsonStr;

        /// <summary>
        /// post 提交参数
        /// </summary>
        public Dictionary<string, string> DicDatas = new Dictionary<string, string>();

        /// <summary>
        /// 文件路径形式
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> DicFilePaths = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 文件字节形式
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> DicFiles = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// 请求协议头
        /// </summary>
        public Dictionary<string, string> DicHeaders = new Dictionary<string, string>();

        /// <summary>
        /// get的请求参数
        /// </summary>
        public Dictionary<string, object> DicQuerys = new Dictionary<string, object>();

        /// <summary>
        /// 请求方式
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// 0=普通post 1=jsonStr 2=json 3=表单
        /// </summary>
        public ParameterMode ParameterType { get; set; }

        public PjmHttpProxy? PjmHttpProxy { get; set; } = null;

        public bool ScopedMode { get; set; }

        public int TimeOutSecond { get; set; } = -1;

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }
    }

    public class PHttp : IDisposable

    {
        private static HttpClient SingletonHttpClient = new HttpClient();

        private HttpOption _HttpOption = new HttpOption();

        public void Dispose()
        {
        }

        /// <summary>
        /// 重置单例HtttpClient实例
        /// 一定要注意当有其他地方在使用时不能轻易重置否则容易出异常
        /// 建议用途:当你后续的操作不改变超时时间和代理的时候可以提前重置一次。 否则都推荐Scoped
        /// </summary>
        /// <returns></returns>
        public PHttp ResetSingleton()
        {
            SingletonHttpClient = new HttpClient();
            return this;
        }

        public HttpResponseMessage Send()
        {
            HttpClient localHttpClient = SingletonHttpClient;

            if (_HttpOption.ScopedMode)
            {
                localHttpClient = new HttpClient();
            }

            if (_HttpOption.PjmHttpProxy != null)
            {
                HttpClientHandler? handler = null;
                string proxyUrl = _HttpOption.PjmHttpProxy.Ip;

                if (!string.IsNullOrEmpty(_HttpOption.PjmHttpProxy.Port))
                {
                    proxyUrl += ":" + _HttpOption.PjmHttpProxy.Port;
                }

                if (string.IsNullOrEmpty(_HttpOption.PjmHttpProxy.Username))
                {
                    handler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(proxyUrl),
                        UseProxy = true,
                        Credentials = new NetworkCredential(_HttpOption.PjmHttpProxy.Username, _HttpOption.PjmHttpProxy.Password)
                    };
                }
                else
                {
                    handler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(proxyUrl),
                        UseProxy = true,
                    };
                }
                try
                {
                    localHttpClient = new HttpClient(handler);
                }
                catch (System.InvalidOperationException ex)
                {
                    throw new Exception("此实例已经启动了请求,无法在后续请求继续修改。建议改用Scoped模式");
                }
            }
            if (_HttpOption.TimeOutSecond != -1)
            {
                try
                {
                    localHttpClient.Timeout = TimeSpan.FromSeconds(_HttpOption.TimeOutSecond);
                }
                catch (System.InvalidOperationException ex)
                {
                    throw new Exception("此实例已经启动了请求,无法在后续请求继续修改。建议改用Scoped模式");
                }
            }

            HttpRequestMessage? data = GetData();
            try
            {
                return localHttpClient.Send(data);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public PHttp SetData(string fieldName, string fieldValue)
        {
            _HttpOption.DicDatas.Add(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        /// 添加文件 默认采用表单
        /// 内置调用 SetParameterMode
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName">为空的话 则是文件的绝对路径+名字</param>
        /// <returns></returns>
        public PHttp SetFile(string fieldName, string filePath, string fileName = "")
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("文件不存在");
            }

            _HttpOption.DicFilePaths.Add(fieldName, new Dictionary<string, string>() { { "FileName", fileName }, { "FilePath", filePath } });
            SetParameterMode(ParameterMode.Multipart);
            return this;
        }

        /// <summary>
        /// 添加文件 默认采用表单
        /// 内置调用 SetParameterMode
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public PHttp SetFileBytes(string fieldName, byte[] fileBytes, string fileName)
        {
            _HttpOption.DicFiles.Add(fieldName, new Dictionary<string, object>() { { "FileName", fileName }, { "FileBytes", fileBytes } });
            SetParameterMode(ParameterMode.Multipart);
            return this;
        }

        /// <summary>
        /// 添加数据 需要编码的地方自行编码  默认 application/x-www-form-urlencoded
        /// 支持AdddFile之类的方法共用。
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        /// <summary>
        /// 添加协议头
        /// Content-Type Content-Length 忽略
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public PHttp SetHeaders(string keyName, string keyValue)
        {
            //无效头  处理

            switch (keyName)
            {
                case "Content-Type":
                case "Content-Length":
                    return this;

                default:
                    break;
            }

            _HttpOption.DicHeaders.Add(keyName, keyValue);

            return this;
        }

        public PHttp SetMethod(HttpMethod httpMethod)
        {
            _HttpOption.Method = httpMethod;
            return this;
        }

        public PHttp SetParameterMode(ParameterMode mode)
        {
            _HttpOption.ParameterType = mode;
            return this;
        }

        /// <summary>
        /// 设置代理
        ///
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public PHttp SetProxy(PjmHttpProxy proxy)
        {
            if (proxy == null)
            {
                throw new Exception("不需要设置请不要使用此方法并且不能传null");
            }
            _HttpOption.PjmHttpProxy = proxy;
            return this;
        }

        /// <summary>
        /// 添加Get参数 请保证Url后面自行添加  ?  例如 https://example.com/api/foo? 没有问号则异常
        /// 为什么这样做考虑后续有自行的需求 不在代码上做自动补充
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public PHttp SetQuery(string fieldName, string fieldValue)
        {
            _HttpOption.DicQuerys.Add(fieldName, fieldValue);
            return this;
        }

        public PHttp SetScopedMode()
        {
            _HttpOption.ScopedMode = true;
            return this;
        }

        public PHttp SetTimeOutSecond(int s)
        {
            _HttpOption.TimeOutSecond = s;
            return this;
        }

        public PHttp SetUrl(string url)
        {
            _HttpOption.Url = url;
            return this;
        }

        /// <summary>
        /// 内部处理数据
        /// </summary>
        /// <returns></returns>
        private HttpRequestMessage GetData()
        {
            if (string.IsNullOrEmpty(_HttpOption.Url))
            {
                throw new Exception("请先设置请求url");
            }
            if (_HttpOption.Method == null)
            {
                throw new Exception("请先设置Method");
            }

            var request = new HttpRequestMessage(_HttpOption.Method, _HttpOption.Url);
            if (_HttpOption.Method == HttpMethod.Get)
            {
                //get的处理

                string queryStr = "";
                foreach (var dicQuery in _HttpOption.DicQuerys)
                {
                    if (queryStr == "")
                    {
                        queryStr = dicQuery.Key + "=" + dicQuery.Value;
                    }
                    else
                    {
                        queryStr += "&" + dicQuery.Key + "=" + dicQuery.Value;
                    }
                }

                _HttpOption.Url += queryStr;
            }
            else
            {
                if (_HttpOption.ParameterType == ParameterMode.JsonStr)
                {
                    request.Content = new StringContent(_HttpOption.DataByJsonStr, Encoding.UTF8, "application/json");
                }
                else if (_HttpOption.ParameterType == ParameterMode.Multipart)
                {
                    var formData = new MultipartFormDataContent();
                    foreach (var dicFilePath in _HttpOption.DicFilePaths)
                    {
                        var fileStream = File.OpenRead(dicFilePath.Value["FilePath"]);
                        var fileContent = new StreamContent(fileStream);

                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = dicFilePath.Key,
                            FileName = dicFilePath.Value["FileName"] == "" ? Path.GetFileName(dicFilePath.Value["FilePath"]) : dicFilePath.Value["FileName"]
                        };
                        //如果文件名称没给出的 则取文件绝对路径含名称
                        formData.Add(fileContent);
                    }
                    foreach (var dicFile in _HttpOption.DicFiles)
                    {
                        MemoryStream memoryStream = new MemoryStream((byte[])dicFile.Value["FileBytes"]);

                        //尝试直接用 流
                        var fileContent = new StreamContent(memoryStream);

                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = dicFile.Key,
                            FileName = (string)dicFile.Value["FileName"]
                        };
                        //如果文件名称没给出的 则取文件绝对路径含名称
                        formData.Add(fileContent);
                    }

                    //如果还有普通字段  这里补充
                    foreach (var dicData in _HttpOption.DicDatas)
                    {
                        formData.Add(new StringContent(dicData.Value), dicData.Key);
                    }

                    request.Content = formData;
                }
                else
                {
                    request.Content = new FormUrlEncodedContent(_HttpOption.DicDatas);
                }
            }

            foreach (var dicHeader in _HttpOption.DicHeaders)
            {
                request.Headers.Add(dicHeader.Key, dicHeader.Value);
            }

            return request;
        }
    }

    public class PjmHttpProxy
    {
        public string Ip { get; set; }

        public string Password { get; set; }
        public string Port { get; set; }

        public string Username { get; set; }
    }
}