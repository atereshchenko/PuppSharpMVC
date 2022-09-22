using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PuppSharpMVC.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;

namespace PuppSharpMVC.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> IndexAsync(IFormFile file)
		{
			string url = Startup.ApiUrl;
			string message = "";
			using (var httpClient = new HttpClient())
			{
				using (var form = new MultipartFormDataContent())
				{
					using (var fs = file.OpenReadStream())
					{
						using (var streamContent = new StreamContent(fs))
						{
							using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
							{
								fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
								form.Add(fileContent, "file", file.FileName);
								HttpResponseMessage response = await httpClient.PutAsync(url, form);
								if (response.IsSuccessStatusCode)
								{
									HttpContent content = response.Content;
									var contentStream = await content.ReadAsStreamAsync();
									return File(contentStream, "application/pdf", "converted.pdf");
								}
								else
								{
									message = response.StatusCode.ToString();
								}
							}
						}
					}
				}
			}
			return View(message);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
