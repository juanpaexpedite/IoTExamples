using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Clients
{
    public class RestClient
    {
        public string URL = String.Empty;

        public RestClient(string uri)
        {
            URL = uri;
        }

        public async Task<T> Request<T>(string method)
        {
            return JsonConvert.DeserializeObject<T>(await Request(method));
        }

        public async Task<T> Request<T>(string method, T data)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(await Request(method));
            }
            catch
            {
                return default(T);
            }
        }

        public async Task<U> Request<T,U>(string method, T data)
        {
            return JsonConvert.DeserializeObject<U>(await Request(method));
        }

        public async Task<String> Request(string method, string data = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{URL}/{method}.json");
            request.Method = data == null ? "GET" : "POST";
            request.ContentType = "application/json";
            StreamWriter requestWriter = new StreamWriter(await request.GetRequestStreamAsync(), System.Text.Encoding.UTF8);
            if (data != null)
            {
                await requestWriter.WriteAsync(data);
                await requestWriter.FlushAsync();
            }

            try
            {
                WebResponse webResponse = await request.GetResponseAsync();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                return await responseReader.ReadToEndAsync();
            }
            catch (Exception e)
            {
               return null;
            }
        }


    }
}
