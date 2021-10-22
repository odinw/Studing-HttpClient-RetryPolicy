using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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
        public IActionResult CallMock()
        {
            HttpClient httpClient = new HttpClient();

            var result = httpClient.GetStringAsync("https://run.mocky.io/v3/3c9a57ff-0b05-4267-8901-ed1b6dadecf8").Result;

            return Ok(httpClient);

            // sucess
            var sucess = client.GetAsync("https://run.mocky.io/v3/3c9a57ff-0b05-4267-8901-ed1b6dadecf8").Result;
            Console.WriteLine($"sucess StatusCode: {sucess.StatusCode}");

            // fail
            var fail = client.GetAsync("https://run.mocky.io/v3/b4498bd3-c223-4296-a467-bfdc2b740fc6").Result;
            Console.WriteLine($"fail StatusCode: {fail.StatusCode}");

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> UserInfo()
        {
            var httpMessage = new HttpRequestMessage(HttpMethod.Post, "http://my/webapi");
            HttpContent content = new StringContent("MyParameter", Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpMessage.Content = content;

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.SendAsync(httpMessage);

            // 檢查 SendAsync 結果
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Error - {response.RequestMessage.RequestUri} {(int)response.StatusCode} {response.StatusCode}");

            // 檢查處理結果
            string ResultMessage = await response.Content.ReadAsStringAsync(); // 這是 json
            JObject jObject = JObject.Parse(ResultMessage);
            bool suss = jObject.SelectToken("Succ").Value<bool>();
            if (suss == false)
                throw new Exception($"Error - {response.RequestMessage.RequestUri} Succ: {suss}");

            return Ok(ResultMessage);
        }

        /// <summary>
        /// 新增一筆消費歷程
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PointRecord()
        {
            var httpMessage = new HttpRequestMessage(HttpMethod.Post, "http://my/webapi");

            string parameter = JsonConvert.SerializeObject(
                new 
                { 
                    
                });

            HttpContent content = new StringContent(parameter, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpMessage.Content = content;

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.SendAsync(httpMessage);

            return Ok();
        }
    }
}
