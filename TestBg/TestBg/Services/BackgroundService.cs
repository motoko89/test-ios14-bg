using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(TestBg.Services.BackgroundService))]
namespace TestBg.Services
{
    public class BackgroundService : IBackgroundService
    {
        public async Task PerformFetch()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://google.com");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode + ":" + (await response.Content.ReadAsStringAsync()));
                }

                string responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("responseString = " + responseString);
            }
        }
    }
}
