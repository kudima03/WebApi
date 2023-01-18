using CommonEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace WebAPIUserApp
{
    public class ConnectionModule
    {
        protected static HttpClient client;

        static ConnectionModule()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("ServerUrl"));
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        public static async Task<List<BookCard>> GetAllBookCardsAsync()
        {
            var response = await client.GetAsync("books");
            List<BookCard> bookCards = null;
            if (response.IsSuccessStatusCode)
            {
                bookCards = JsonConvert.DeserializeObject<List<BookCard>>(await response.Content.ReadAsStringAsync());
            }
            return bookCards;
        }

        public static async Task<HttpStatusCode> PostBookCardAsync(BookCard bookCard)
        {
            var response = await client.PostAsync("books", new StringContent(JsonConvert.SerializeObject(bookCard), Encoding.UTF8, "application/json"));
            return response.StatusCode;
        }

        public static async Task<HttpStatusCode> PutBookCard(BookCard bookCard)
        {
            var response = await client.PutAsync("books", new StringContent(JsonConvert.SerializeObject(bookCard), Encoding.UTF8, "application/json"));
            return response.StatusCode;
        }

        public static async Task<HttpStatusCode> DeleteBookCard(int[] idArray)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(idArray), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(client.BaseAddress + "books")
            };
            var response = await client.SendAsync(request);
            return response.StatusCode;
        }
    }
}

