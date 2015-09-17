using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace authorize_server
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new ApiHelper("http://localhost:60847/".TrimEnd('/'));

            var login = api.Post("/token", new StringContent(string.Format("username={0}&password={1}&grant_type=password", "oclockvn@gmail.com", "123456")));

            var jss = new JavaScriptSerializer();
            var json = jss.Deserialize<dynamic>(login.ToString());

            var token = json["access_token"];

            var values = api.Get("/api/values", token);
        }
    }

    class ApiHelper
    {
        private HttpClient _client;

        public ApiHelper(string baseURL)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseURL);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));           
        }

        public List<object> Get(string request, string token="")
        {
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var resp = _client.GetAsync(request).Result;

            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }

            try
            {
                return resp.Content.ReadAsAsync<List<object>>().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object Post(string request, object data, string token="")
        {
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var resp = _client.PostAsync(request, data as HttpContent).Result;
            // var resp = _client.PostAsJsonAsync(request, data).Result;

            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }

            try
            {
                return resp.Content.ReadAsAsync<object>().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
