using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;

namespace Studing_HttpClient_RetryPolicy.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly HttpClient client;

        public ValuesController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("Retry");
        }

        [HttpGet("[action]")]
        public object CallMock()
        {
            // sucess
            var sucess = client.GetAsync("https://run.mocky.io/v3/3c9a57ff-0b05-4267-8901-ed1b6dadecf8").Result;
            Console.WriteLine($"sucess StatusCode: {sucess.StatusCode}");

            // fail
            var fail = client.GetAsync("https://run.mocky.io/v3/b4498bd3-c223-4296-a467-bfdc2b740fc6").Result;
            Console.WriteLine($"fail StatusCode: {fail.StatusCode}");

            return Ok();
        }
    }
}
