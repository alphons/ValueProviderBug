
using Microsoft.AspNetCore.Mvc;

#nullable enable

namespace ValueProviderBug
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

			var expected = new List<string?>() { "a", "b", null, "c" };

			return Content($"<pre>returns {string.Join(",", list)} expected {string.Join(",", expected)}</pre>", "text/html");

		}
	}
}
