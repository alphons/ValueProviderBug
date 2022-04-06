
using Microsoft.AspNetCore.Mvc;

namespace CoreBasic.Web.LogicControllers
{
	public class ComplexController : ControllerBase
	{

		public class ApiModel
		{
			public string Name { get; set; }
			public List<List<Usert>> Users { get; set; }
		}

		[HttpPost]
		[Route("~/api/ComplexTest2/{SomeParameter2}")]
		public async Task<IActionResult> ComplexTest2(
			[FromHeader(Name ="Referer")] string SomeParameter1,
			[FromRoute] string SomeParameter2,
			[FromQuery] string SomeParameter3,
			[FromBody] ApiModel SomeParameter4,
			[FromBody] string SomeParameter5)
		{
			await Task.Yield();

			return Ok(new
			{

			});
		}


		[HttpPost]
		[Route("~/api/ComplexString")]
		public async Task<IActionResult> ComplexString(string Name)
		{
			await Task.Yield();

			return Ok(new
			{
				Name
			});
		}

		[HttpPost]
		[Route("~/api/ComplexDouble")]
		public async Task<IActionResult> ComplexDouble(double? F)
		{
			await Task.Yield();

			return Ok(new
			{
				F
			});
		}

		[HttpPost]
		[Route("~/api/ComplexStringInt")]
		public async Task<IActionResult> ComplexStringInt(string Name, int A)
		{
			await Task.Yield();

			return Ok(new
			{
				Name,
				A
			});
		}

		[HttpPost]
		[Route("~/api/ComplexListOfStrings")]
		public async Task<IActionResult> ComplexListOfStrings(List<string> ListOfStrings)
		{
			await Task.Yield();

			return Ok(new
			{
				ListOfStrings
			});
		}

		[HttpPost]
		[Route("~/api/ComplexListOfInts")]
		public async Task<IActionResult> ComplexListOfInts(List<int?> ListOfInts)
		{
			await Task.Yield();

			return Ok(new
			{
				ListOfInts
			});
		}

		[HttpPost]
		[Route("~/api/ComplexListNullableDouble")]
		public async Task<IActionResult> ComplexListNullableDouble(List<double?> list)
		{
			await Task.Yield();

			return Ok(new
			{
				list
			});
		}


		[HttpPost]
		[Route("~/api/ComplexListObjecs")]
		public async Task<IActionResult> ComplexListObjecs(List<string> list)
		{
			await Task.Yield();

			return Ok(new
			{
				list
			});
		}

		[HttpPost]
		[Route("~/api/ComplexStringList")]
		public async Task<IActionResult> ComplexStringList(string Name, List<string> list)
		{
			await Task.Yield();

			return Ok(new
			{
				Name,
				list
			});
		}

		public class ObjectB
		{
			public List<ObjectA> List { get; set; }

			public string Name { get;set;}
		}

		public class ObjectA
		{
			public string a { get; set; }
			public string b { get; set; }
		}

		[HttpPost]
		[Route("~/api/ComplexSingleObject")]
		public async Task<IActionResult> ComplexSingleObject(ObjectA AA)
		{
			await Task.Yield();

			return Ok(new
			{
				AA
			});
		}


		[HttpPost]
		[Route("~/api/ComplexArray")]
		public async Task<IActionResult> ComplexArray(ObjectA[] list)
		{
			await Task.Yield();

			return Ok(new
			{
				list
			});
		}

		[HttpPost]
		[Route("~/api/ComplexObjectArray")]
		public async Task<IActionResult> ComplexObjectArray(ObjectB objB)
		{
			await Task.Yield();

			return Ok(new
			{
				objB
			});
		}

		public class ObjectC
		{
			public string Name { get; set; }
			public List<List<string>> Users { get; set; }
		}

		[HttpPost]
		[Route("~/api/ComplexArrayArray")]
		public async Task<IActionResult> ComplexArrayArray(string Group, ObjectC List)
		{
			await Task.Yield();

			return Ok(new
			{
				Group,
				List
			});
		}

		public class Usert
		{
			public string Name { get; set; }

			public List<string> Alias { get; set; }
		}

		public class ObjectD
		{
			public string Name { get; set; }
			public List<List<Usert>> Users { get; set; }
		}

		[HttpPost]
		[Route("~/api/ComplexArrayArrayClass")]
		public async Task<IActionResult> ComplexArrayArrayClass(bool Testing, bool Relaxed, string Group, ObjectD GroupInfo)
		{
			await Task.Yield();

			return Ok(new
			{
				Testing,
				Relaxed,
				Group,
				GroupInfo
			});
		}


		/// <summary>
		/// Uploads have default a maximum of 30MByte presenting upload example of 2.5GB
		///
		/// For IIS Limit maxAllowedContentLength in Web.config (in the root of the app, not in wwwroot content folder!)
		/// 
		/// </summary>
		/// <param name="formFile"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("~/api/Upload")]
		[RequestSizeLimit(2_500_000_000)]
		[RequestFormLimits(MultipartBodyLengthLimit = 2_500_000_000)]
		public async Task<IActionResult> Upload(IFormFile file, string Form1)
		{
			if (file.Length > 0)
			{
				using var ms = new MemoryStream();
				await file.CopyToAsync(ms); // some dummy operation
				System.Diagnostics.Debug.WriteLine("yes " + ms.Length);
			}
			return Ok(new 
			{ 
				file.Length,
				Form1
			});
		}



	}
}
