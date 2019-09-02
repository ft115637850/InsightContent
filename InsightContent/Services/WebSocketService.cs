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

namespace InsightContent.Services
{
    public class WebSocketService
    {
        public static async Task ProcessWebSocket(HttpContext context, WebSocket webSocket, IPubSubService broker)
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
            var socketId = Guid.NewGuid();

            var result = await ProcessReceive(webSocket);
            while (!result.Item1.CloseStatus.HasValue)
            {
                if (result.Item2 != string.Empty)
                {
                    var dt = JsonConvert.DeserializeObject<DataTable>(result.Item2);
                    foreach (DataRow dr in dt.Rows)
                    {
                        var action = Convert.ToString(dr[0]);
                        var topic = Convert.ToString(dr[1]);
                        if (action == "subscribe")
                        {
                            broker.Subscribe(topic, webSocket, socketId);
                        }
                        else
                        {
                            broker.Unsubscribe(topic, socketId);
                        }
                    }
                }
                
                result = await ProcessReceive(webSocket);
                // TO DO: check time elapse
                // Thread.Sleep(1000);
            }
            await webSocket.CloseAsync(result.Item1.CloseStatus.Value, result.Item1.CloseStatusDescription, CancellationToken.None);
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
    }
}
