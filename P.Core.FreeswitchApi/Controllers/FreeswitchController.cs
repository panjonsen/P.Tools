using Microsoft.AspNetCore.Mvc;
using P.Core.FreeswitchApi.Serivice;

namespace P.Core.FreeswitchApi.Controllers
{
	
	/// <summary>
 /// 控制器注释
 /// </summary>
	[Route("api/[controller]/[action]")]
	[ApiController]
	
	public class FreeswitchController : ControllerBase
	{
		private readonly FreeswitchService _FreeswitchService;

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="freeswitchService"></param>
		public FreeswitchController(FreeswitchService freeswitchService)
		{
			this._FreeswitchService = freeswitchService;
		}

		/// <summary>
		/// 通话
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult ShowCalls()
		{
			var rsp = _FreeswitchService.ShowCalls();

			return Ok(rsp);
		}

		/// <summary>
		/// 发起呼叫
		/// </summary>
		/// <param name="gateway">网关id 9d66cbe1-5025-4d8d-8bb4-92209687b6ba</param>
		/// <param name="callNumber">网关主叫号码 17056036476</param>
		/// <param name="descNumber">接听号码  130001300000</param>
		/// <param name="answerExtension">接听坐席 xxxxxxxxxxx@huihu.zjfantian.cn:56001</param>
		/// <returns></returns>
		[HttpGet("{gateway}/{callNumber}/{descNumber}/{answerExtension}")]
		public IActionResult Call(string gateway, string callNumber, string descNumber, string answerExtension) {

			var rsp = _FreeswitchService.Call(gateway, callNumber, descNumber, answerExtension);

			return Ok(rsp);

		}
	}
}