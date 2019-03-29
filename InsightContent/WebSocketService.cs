using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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

        public static async Task ProcessWebSocket(HttpContext context, WebSocket webSocket)
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);

            var result = await ProcessReceive(webSocket);
            while (!result.Item1.CloseStatus.HasValue)
            {
                await ProcessSend(webSocket, result.Item2);
                result = await ProcessReceive(webSocket);
            }
            await webSocket.CloseAsync(result.Item1.CloseStatus.Value, result.Item1.CloseStatusDescription, CancellationToken.None);
        }

        private static async Task ProcessSend(WebSocket webSocket, string req)
        {
            var r = new Random(100);
            if (req == "time")
            {
                int i = 0;
                while(i < 60)
                {
                    byte[] array = Encoding.ASCII.GetBytes(i.ToString());
                    await webSocket.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, CancellationToken.None);
                    i++;
                    Thread.Sleep(1000);
                }
            }
            else
            {
                int i = 5;
                while (i > 0)
                {
                    byte[] array = Encoding.ASCII.GetBytes(r.Next(5).ToString());
                    await webSocket.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, CancellationToken.None);
                    i--;
                    Thread.Sleep(1000);
                }
            }            
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
