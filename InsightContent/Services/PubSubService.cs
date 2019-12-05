using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tagdataaccessor;

namespace InsightContent.Services
{
    public class PubSubService : TagDataAccessor.TagDataAccessorBase, IPubSubService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _VTQserverIP = "localhost";
        private readonly int _VTQserverPort = 50051;
        private Server VTQserver;
        private IDictionary<string, List<(WebSocket, Guid)>> subscriptionList = new Dictionary<string, List<(WebSocket, Guid)>>();

        public PubSubService(ILogger<PubSubService> logger)
        {
            _logger = logger;
            this.StartAsync(new CancellationToken());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            VTQserver = new Server
            {
                Services = { TagDataAccessor.BindService(this) },
                Ports = { new ServerPort(this._VTQserverIP, this._VTQserverPort, ServerCredentials.Insecure) }
            };
            VTQserver.Start();
            _logger.LogInformation($"Now listening on: grpc://{this._VTQserverIP}:{this._VTQserverPort}");

            return Task.CompletedTask;
        }

        public override Task<VTQReply> SendVTQ(VTQ request, ServerCallContext context)
        {
            this.Publish(request.Name, request.Value);
            return Task.FromResult(new VTQReply { Message = $"Tag:{request.Name} Value:{request.Value} Time:{request.Time} Quality:{request.Quality}" });
        }

        private void Publish(string tagName, string tagValue)
        {
            if (!this.subscriptionList.ContainsKey(tagName))
            {
                return;
            }

            var item = this.subscriptionList[tagName];
            item.RemoveAll(x => x.Item1.CloseStatus.HasValue);
            if (item.Count == 0)
            {
                this.subscriptionList.Remove(tagName);
            }
            else
            {
                var dt = new DataTable();
                dt.Columns.Add(new DataColumn("tagName"));
                dt.Columns.Add(new DataColumn("value"));
                var dr = dt.NewRow();
                dr[0] = tagName;
                dr[1] = tagValue;
                dt.Rows.Add(dr);
                var result = JsonConvert.SerializeObject(dt);

                foreach (var ws in item)
                {
                    byte[] array = Encoding.ASCII.GetBytes(result);
                    ws.Item1.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tag server is stopping.");
            VTQserver.ShutdownAsync().Wait();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public void Subscribe(string topic, WebSocket ws, Guid wsId)
        {
            if (this.subscriptionList.ContainsKey(topic))
            {
                if (this.subscriptionList[topic].FindIndex(x => x.Item2 == wsId) == -1)
                {
                    this.subscriptionList[topic].Add((ws, wsId));
                }
            }
            else
            {
                this.subscriptionList.Add(topic, new List<(WebSocket, Guid)>() { (ws, wsId) });
            }
        }

        public void Unsubscribe(string topic, Guid wsId)
        {
            if (this.subscriptionList.ContainsKey(topic))
            {
                this.subscriptionList[topic].RemoveAll(x => x.Item2 == wsId);
                if (this.subscriptionList[topic].Count == 0)
                {
                    this.subscriptionList.Remove(topic);
                }
            }
        }
    }
}
