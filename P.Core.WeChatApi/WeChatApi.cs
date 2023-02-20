using Newtonsoft.Json;
using P.Core.Http;
using System.Net;
using System.Text;

namespace P.Core.WeChatApi
{
    public class DtoPushMsg
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }

        public string media_id { get; set; }
        public string type { get; set; }
    }

    public class WeChatApi
    {
        private string ApiBaseAddress = "https://qyapi.weixin.qq.com";
        private string ApiKey;

        public WeChatApi(string apiKey)
        {
            ApiKey = apiKey;
        }

        public bool PushMsg(string msg)
        {
            string httpBody = @$"
					{{
						""msgtype"": ""markdown"",
						""markdown"": {{
							""content"": ""{msg}""
						}}
					}}
					";

            var rsp = new PHttp().SetUrl(ApiBaseAddress + "/cgi-bin/webhook/send?key=" + ApiKey)
                   .SetMethod(HttpMethod.Post)
                   .SetDataByJsonStr(httpBody)
                   .SetScopedMode()
                   .Send()
                   .ToString2();

            var resultDto = JsonConvert.DeserializeObject<DtoPushMsg>(rsp);

            if (resultDto.errcode == 0)
            {
                return true;
            }
            return false;
        }

        public bool PushFile(byte[] fileDatas, string fileName, string fieldName)
        {
            var rsp = HttpUploadFile(fileDatas, fieldName, fileName);
            var resultDto = JsonConvert.DeserializeObject<DtoPushMsg>(rsp);

            if (resultDto == null)
            {
                return false;
            }

            if (resultDto.errcode != 0)
            {
                return false;
            }

            var httpBody = @$"{{
			    ""msgtype"": ""file"",
			    ""file"": {{
			         ""media_id"": ""{resultDto.media_id}""
			    }}
			}}";

            rsp = new PHttp().SetUrl(ApiBaseAddress + "/cgi-bin/webhook/send?key=" + ApiKey)
                  .SetMethod(HttpMethod.Post)
                  .SetDataByJsonStr(httpBody)
                  .SetScopedMode()
                  .Send()
                  .ToString2();

            resultDto = JsonConvert.DeserializeObject<DtoPushMsg>(rsp);

            if (resultDto.errcode == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Http上传文件
        /// </summary>
        private string HttpUploadFile(byte[] fileData, string fieldName, string fileName)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(ApiBaseAddress + @$"/cgi-bin/webhook/upload_media?key={ApiKey}&type=file") as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            //请求头部信息
            StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"{0}\";filename=\"{1}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fieldName, fileName));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

            byte[] bArr = fileData;

            Stream postStream = request.GetRequestStream();
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(bArr, 0, bArr.Length);
            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            postStream.Close();

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream instream = response.GetResponseStream();
            StreamReader sr = new StreamReader(instream, Encoding.UTF8);
            //返回结果网页（html）代码
            string content = sr.ReadToEnd();
            return content;
        }
    }
}