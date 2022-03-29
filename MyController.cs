
using Microsoft.AspNetCore.Mvc;

namespace TestWeb
{
	public class MyController : ControllerBase
	{
		[HttpGet]
		[Route("~/")]
		public async Task<IActionResult> Index(List<string> list)
		{
			await Task.Yield();

			// Returned list "a", b", "b",  "c"
			// Expected list "a", b", null, "c"

			return Content("<pre>returns "+string.Join(",", list)+ " expected a,b,,c</pre>", "text/html");

		}
	}
}
