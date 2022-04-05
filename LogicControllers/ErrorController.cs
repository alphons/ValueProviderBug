
using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

namespace ValueProviderBug.LogicControllers
{
	public class ErrorController : ControllerBase
	{

		public ErrorController()
		{
		}

		public enum ErrorTypeEnum
		{
			Unknown = 0,
			Info = 1,
			Exception = 2
		}

		public class ErrorLogClass
		{
			public int Id { get; set; }

			//[Column(TypeName = "datetime2")]
			public DateTime DateTime { get; set; }
			public string Name { get; set; }
			public string UserAgent { get; set; }
			public string Ip { get; set; }
			public string Url { get; set; }

			// ClientNotify
			public string Referer { get; set; }
			public ErrorTypeEnum ErrorType { get; set; }

			// Exception
			public string Message { get; set; }
			public string ExceptionType { get; set; }
			public string StackTrace { get; set; }
		}

		public async Task<bool> ErrorLogAsync(ErrorLogClass errorlog)
		{
			await Task.Yield();
			return true;
		}

		[DebuggerNonUserCode]
		[Route("~/internalerror")]
		public async Task<IActionResult> LogInternalError()
		{
			await Task.Yield();

			return StatusCode(500, "LogInternalError");
		}

		[HttpPost]
		[Route("~/api/ErrorLog")]
		public async Task<IActionResult> ApiErrorLog(string Message, string ExceptionType, string Referer, string Url, string Status)
		{
			if (Status == "0")
				return Ok();

			await Task.Yield();

			return Ok();
		}

		[HttpPost]
		[Route("~/error/ErrorLog")]
		public async Task<IActionResult> ErrorLog(string Message, string ExceptionType, string Referer, string Url, string Status)
		{
			if (Status == "0")
				return Ok();

			await Task.Yield();

			return Ok();
		}


	}
}

