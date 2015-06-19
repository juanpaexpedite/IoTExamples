using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.AppService;
using Windows.System.Threading;
using Windows.Networking.Sockets;
using System.IO;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

namespace IoT.Servers
{
    public class RestServer : IDisposable
    {
        string RestString = "{0}";
        private const uint BufferSize = 8192;
        private int port = 8000;
        private readonly StreamSocketListener listener;
        private AppServiceConnection appServiceConnection;

        public RestServer(int serverPort, string serviceConnection)
        {
            listener = new StreamSocketListener();
            port = serverPort;
            appServiceConnection = new AppServiceConnection() { AppServiceName = serviceConnection };
            listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
        }


        public RestServer(int serverPort, AppServiceConnection connection)
        {
            listener = new StreamSocketListener();
            port = serverPort;
            appServiceConnection = connection;
            listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
        }

        public void StartServer()
        {
#pragma warning disable CS4014
            listener.BindServiceNameAsync(port.ToString());
#pragma warning restore CS4014
        }

        public void Dispose()
        {
            listener.Dispose();
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            var requestbuilder = await ReadInputStream(socket.InputStream);
            var requestlines = requestbuilder.ToString().Split('\n');

            string requestMethod = requestlines[0];
            string data = requestlines.Last().TrimEnd('\0');
            string[] requestParts = requestMethod.Split(' ');
            string method = requestParts[1].TrimStart('/');
            method = method.Substring(0, method.IndexOf('.')); //If you want the inline ? parameters just reedit this line

            using (IOutputStream output = socket.OutputStream)
            {
                if (requestParts[0] == "GET")
                    await WriteResponseAsync(output,method);
                else if(requestParts[0] == "POST")
                    await WriteResponseAsync(output,method,data);
            }
        }

        private async Task<StringBuilder> ReadInputStream(IInputStream InputStream)
        {
            StringBuilder requestbuilder = new StringBuilder();
            using (IInputStream input = InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    requestbuilder.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }
            return requestbuilder;
        }

        public Func<String,String,String> GetContentRequestData { get; set; }

        private async Task WriteResponseAsync(IOutputStream os, string method, string data = null)
        {
            string html = String.Format(RestString, GetContentRequestData == null ? String.Empty : GetContentRequestData(method,data));
            using (Stream resp = os.AsStreamForWrite())
            {
                byte[] bodyArray = Encoding.UTF8.GetBytes(html);
                MemoryStream stream = new MemoryStream(bodyArray);
                string header = String.Format("HTTP/1.1 200 OK\r\n" + "Content-Length: {0}\r\n" + "Connection: close\r\n\r\n", stream.Length);
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
            }
        }
    }
}
