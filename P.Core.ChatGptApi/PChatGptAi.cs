using System.Text;
using Newtonsoft.Json;
using P.Core.Http;

namespace P.Core.ChatGptApi
{
	public class ChatGptOption
	{
		public string Url { get; set; }

		public string ApiKey { get; set; }
	}

	/*
		 prompt: 这是一个字符串字段，表示您希望向ChatGPT API提供的文本提示。ChatGPT将使用这个提示来生成回复。在这个例子中，我们将prompt设置为"Hello, ChatGPT!"，表示我们希望ChatGPT生成一句问候语。

		max_tokens: 这是一个整数字段，表示ChatGPT生成的文本中最多包含多少个标记（token）。标记是文本中的单词、标点符号和其他单个文本元素。在这个例子中，我们将max_tokens设置为50，表示我们希望生成的文本不超过50个标记。

		n: 这是一个整数字段，表示您希望ChatGPT生成多少个文本回复。在这个例子中，我们将n设置为1，表示我们只需要生成一个文本回复。

		temperature: 这是一个浮点数字段，表示生成文本的“温度”（temperature）。温度控制生成文本的多样性和不确定性。较高的温度会导致更随机和多样化的文本，而较低的温度则会导致更稳定和可预测的文本。在这个例子中，我们将temperature设置为0.5，表示我们希望生成相对稳定和可预测的文本。
	*/

	public class ChatGptRequestData
	{
		/// <summary>
		/// 问的内容
		/// </summary>
		public string Prompt { get; set; }

		/// <summary>
		/// 包含多少个标记
		/// </summary>
		public int MaxTokens { get; set; }

		/// <summary>
		/// 表示生成文本的“温度”（temperature）。温度控制生成文本的多样性和不确定性。
		/// </summary>
		public float Temperature { get; set; }

		/// <summary>
		/// 模型类型
		/// </summary>
		public string ModelType { get; set; } = "text-davinci-003";
	}

	public record class ChatGptApiResult
	{
		public string id { get; set; }

		public string @object { get; set; }
		public int created { get; set; }

		public string model { get; set; }

		public List<ChatGptApiResultChoices> choices { get; set; }

		public ChatGptApiResultUsage usage { get; set; }
	}

	public record class ChatGptApiResultChoices
	{
		public string text { get; set; }

		public string index { get; set; }

		public object logprobs { get; set; }

		public string finish_reason { get; set; }
	}

	public record class ChatGptApiResultUsage
	{
		public int prompt_tokens { get; set; }
		public int completion_tokens { get; set; }
		public int total_tokens { get; set; }
	}

	public class PChatGptAi
	{
		private readonly ChatGptOption _ChatGptOption;

		public PChatGptAi(ChatGptOption chatGptOption)
		{
			this._ChatGptOption = chatGptOption;
		}

		public string Send(ChatGptRequestData chatGptRequestData)
		{
			try
			{
				var requestDataJson = @$"{{""model"":""{chatGptRequestData.ModelType}"",""prompt"":""{chatGptRequestData.Prompt}"",""temperature"":{chatGptRequestData.Temperature},""max_tokens"":{chatGptRequestData.MaxTokens},""top_p"":1,""frequency_penalty"":0,""presence_penalty"":0}}";

				PHttp pHttpHelper = new();

				var rsp = pHttpHelper.SetUrl(_ChatGptOption.Url)
					.SetMethod(HttpMethod.Post)
					.SetHeaders("Authorization", "Bearer " + _ChatGptOption.ApiKey)
					.SetDataByJsonStr(requestDataJson)
					.SetTimeOutSecond(60)
					.SetHeaders("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6")
					.SetScopedMode()
					.Send();

				using (var reader = new StreamReader(rsp.Content.ReadAsStream(), Encoding.UTF8))
				{
					var result = reader.ReadToEnd();

					return result;
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		public ChatGptApiResult FormatStr(string str)
		{
			return JsonConvert.DeserializeObject<ChatGptApiResult>(str);
		}
	}
}