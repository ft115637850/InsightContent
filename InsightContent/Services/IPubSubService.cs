using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace InsightContent.Services
{
    public interface IPubSubService
    {
        void Subscribe(string topic, WebSocket ws, Guid socketId);
        void Unsubscribe(string topic, Guid socketId);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
