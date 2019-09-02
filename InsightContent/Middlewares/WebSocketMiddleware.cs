using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using InsightContent.Services;

namespace InsightContent.Middlewares
{
    public class WebSocketMiddleware
    {
        private RequestDelegate _nextDelegate;
        private IPubSubService _broker;

        public WebSocketMiddleware(RequestDelegate nextDelegate, IPubSubService broker)
        {
            this._nextDelegate = nextDelegate;
            this._broker = broker;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/ws")
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                    await WebSocketService.ProcessWebSocket(httpContext, webSocket, this._broker);
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                }
            }
            else
            {
                await _nextDelegate.Invoke(httpContext);
            }
        }
    }
}
