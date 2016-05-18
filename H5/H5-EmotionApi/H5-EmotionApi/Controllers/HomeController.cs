using H5_EmotionApi.Models;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace H5_EmotionApi.Controllers
{
    public class HomeController : Controller
    {
        //_apiKey: Replace this with your own Project Oxford Emotion API key, please do not use my key. I inlcude it here so you can get up and running quickly but you can get your own key for free at https://www.projectoxford.ai/emotion
        public const string _apiKey = "3d90d31ad43746679cb9f0cc7bb774ea";

        //_apiUrl: The base URL for the API. Find out what this is for other APIs via the API documentation
        public const string _apiUrl = "https://api.projectoxford.ai/emotion/v1.0/recognize";

        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/FileExample
        public ActionResult FileExample()
        {
            return View();
        }

        // POST: Home/FileExample
        [HttpPost]
        public async Task<ActionResult> FileExample(HttpPostedFileBase file)
        {
            using (var httpClient = new HttpClient())
            {
                //setup HttpClient
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

                //setup data object
                HttpContent content = new StreamContent(file.InputStream);
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

                //make request
                var response = await httpClient.PostAsync(_apiUrl, content);

                //read response and write to view
                var responseContent = await response.Content.ReadAsStringAsync();
                ViewData["Result"] = responseContent;
            }

            return View();
        }

        public async Task<ActionResult> URLExample()
        {
            using (var httpClient = new HttpClient())
            {
                //setup HttpClient
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //setup data object
                var dataObject = new URLData()
                {
                    url = "https://scontent.fotp3-1.fna.fbcdn.net/v/t1.0-9/10369182_632067816884944_2968610805758093992_n.jpg?oh=58ba0683a978c2a67deef500e1e23a86&oe=579EAB66"
                };

                //setup httpContent object
                var dataJson = JsonConvert.SerializeObject(dataObject);
                HttpContent content = new StringContent(dataJson);
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                //make request
                var response = await httpClient.PostAsync(_apiUrl, content);

                //read response and write to view
                var responseContent = await response.Content.ReadAsStringAsync();
                ViewData["Result"] = responseContent;
            }

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}