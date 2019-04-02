using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsightContent
{
    public class WebSocketService
    {
        public static async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public static async Task ProcessWebSocket(HttpContext context, WebSocket webSocket, IPubSubService broker)
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);

            var result = await ProcessReceive(webSocket);
            while (!result.Item1.CloseStatus.HasValue)
            {
                // await ProcessSend(webSocket, getData(result.Item2));
                broker.Subscribe(result.Item2, webSocket);
                result = await ProcessReceive(webSocket);
                // TO DO: check time elapse
                // Thread.Sleep(1000);
            }
            // TO DO: Unsubscribe all tags
            broker.Unsubscribe(result.Item2);
            await webSocket.CloseAsync(result.Item1.CloseStatus.Value, result.Item1.CloseStatusDescription, CancellationToken.None);
        }

        private static async Task ProcessSend(WebSocket webSocket, string req)
        {
            byte[] array = Encoding.ASCII.GetBytes(req.ToString());
            await webSocket.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        private static async Task<(WebSocketReceiveResult, string)> ProcessReceive(WebSocket webSocket)
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
            WebSocketReceiveResult result = null;
            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        string data = reader.ReadToEnd();
                        return (result, data);
                    }
                }
            }
            return (result, null);
        }

        static Random seed = new Random(100);
        private static string getData(string requestData)
        {
            if (requestData == string.Empty)
            {
                return "";
            }

            var dt = JsonConvert.DeserializeObject<DataTable>(requestData);
            foreach(DataRow dr in dt.Rows)
            {
                if (Convert.ToString(dr[0]).Contains("SysTimeSec"))
                {
                    dr[1] = DateTime.Now.Second;
                    dr[2] = 59;
                    dr[3] = 0;
                }
                else if (Convert.ToString(dr[0]).Contains("Trend"))
                {
                    dr[1] = Math.Sin(Math.PI * 2 * DateTime.Now.Second / 60);
                    dr[2] = 1;
                    dr[3] = -1;
                }
                else
                {
                    dr[1] = seed.Next(100);
                    dr[2] = 100;
                    dr[3] = 0;
                }
            }

            return JsonConvert.SerializeObject(dt);
        }
    }
}
