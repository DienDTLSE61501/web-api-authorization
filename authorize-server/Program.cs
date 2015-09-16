using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace authorize_server
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new ApiHelper("http://localhost:60847/".TrimEnd('/'));

            var values = api.Get("/api/values");

            var login = api.Post("token", new
            {
                username = "oclockvn@gmail.com",
                password = "123456",
                grant_type = "password"
            });

            

            var login2 = api.Post("/token", new StringContent(string.Format("username={0}&password={1}&grant_type=password","oclockvn@gmail.com", "123456")));

            var login3 = api.Post("/token", new StringContent(string.Format("username={0}&password={1}&grant_type=password", "oclockvn@gmail.com", "123456")));
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

        public List<object> Get(string request)
        {
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

        public object Post(string request, object data)
        {
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
