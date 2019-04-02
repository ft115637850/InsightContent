using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace InsightContent
{
    public interface IPubSubService
    {
        void Subscribe(string requestData, WebSocket ws);
        void Unsubscribe(string requestData);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
