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

namespace InsightContent
{
    public class PubSubService : IPubSubService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private IDictionary<string, List<(WebSocket, Guid)>> subscriptionList = new Dictionary<string, List<(WebSocket, Guid)>>();
        private Random seed = new Random(100);

        public PubSubService(ILogger<PubSubService> logger)
        {
            _logger = logger;
            this.StartAsync(new CancellationToken());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("tagName"));
            dt.Columns.Add(new DataColumn("value"));
            dt.Columns.Add(new DataColumn("max"));
            dt.Columns.Add(new DataColumn("min"));
            var removeList = new List<string>();
            foreach (var item in this.subscriptionList)
            {
                item.Value.RemoveAll(x => x.Item1.CloseStatus.HasValue);
                if (item.Value.Count == 0)
                {
                    removeList.Add(item.Key);
                    continue;
                }
                var dr = dt.NewRow();
                dr[0] = item.Key;
                if (item.Key == "isPumping")
                {
                    dr[1] = DateTime.Now.Second % 10 > 5;
                    dr[2] = true;
                    dr[3] = false;
                }
                else if (item.Key == "noPumping")
                {
                    dr[1] = DateTime.Now.Second % 10 < 5;
                    dr[2] = true;
                    dr[3] = false;
                }
                else if (item.Key.Contains("SysTimeSec"))
                {
                    dr[1] = DateTime.Now.Second;
                    dr[2] = 59;
                    dr[3] = 0;
                }
                else if (item.Key.Contains("Trend"))
                {
                    dr[1] = Math.Sin(Math.PI * 2 * DateTime.Now.Second / 60);
                    dr[2] = 1;
                    dr[3] = -1;
                }
                else if (item.Key.Contains("Pump"))
                {
                    dr[1] = DateTime.Now.Second % 10 > 5 ? 1 : 0;
                    dr[2] = 1;
                    dr[3] = -1;
                }
                else
                {
                    dr[1] = seed.Next(100);
                    dr[2] = 100;
                    dr[3] = 0;
                }
                dt.Rows.Add(dr);
                var result = JsonConvert.SerializeObject(dt);

                foreach (var ws in item.Value)
                {
                    byte[] array = Encoding.ASCII.GetBytes(result);
                    ws.Item1.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                dt.Clear();
            }

            foreach(var topic in removeList)
            {
                this.subscriptionList.Remove(topic);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
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
