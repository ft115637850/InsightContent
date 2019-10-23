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

namespace InsightContent.Services
{
    public class PubSubService : IPubSubService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private IDictionary<string, List<(WebSocket, Guid)>> subscriptionList = new Dictionary<string, List<(WebSocket, Guid)>>();
        private Random seed = new Random(100);
        private int _suspension = 10000;
        private int _chroma = 200;
        private int _lotion = 60;
        private int _phosphorus = 600;
        private int _pH = 14;
        private int _COD = 6000;
        private int _SS = 1500;

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
                    dr[1] = DateTime.Now.Second % 10 > 5 ? "ON" : "OFF";
                    dr[2] = "";
                    dr[3] = "";
                }
                else if (item.Key == "Fan")
                {
                    dr[1] = DateTime.Now.Second % 10 < 5 ? "ON" : "OFF";
                    dr[2] = "";
                    dr[3] = "";
                }
                else if (item.Key == "changingString")
                {
                    dr[1] = DateTime.Now.Second % 10 < 5 ? "DateTime" : "DateTime.Now.Second";
                    dr[2] = "";
                    dr[3] = "";
                }
                else if (item.Key.Contains("SysTimeSec"))
                {
                    dr[1] = DateTime.Now.Second;
                    dr[2] = 59;
                    dr[3] = 0;
                }
                else if (item.Key.Contains("Millisecond"))
                {
                    dr[1] = DateTime.Now.Millisecond;
                    dr[2] = 999;
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
                else if (item.Key.Contains("suspension"))
                {
                    _suspension = _suspension - 50 > 50 ? _suspension - 50 : 10000;
                    dr[1] = _suspension;
                    dr[2] = 10000;
                    dr[3] = 50;
                }
                else if (item.Key.Contains("chroma"))
                {
                    _chroma = _chroma - 20 > 60 ? _chroma - 20 : 200;
                    dr[1] = _chroma;
                    dr[2] = 200;
                    dr[3] = 60;
                }
                else if (item.Key.Contains("lotion"))
                {
                    _lotion = _lotion - 5 > 20 ? _lotion - 5 : 60;
                    dr[1] = _lotion;
                    dr[2] = 60;
                    dr[3] = 20;
                }
                else if (item.Key.Contains("phosphorus"))
                {
                    _phosphorus = _phosphorus - 50 > 2 ? _phosphorus - 50 : 600;
                    dr[1] = _phosphorus;
                    dr[2] = 600;
                    dr[3] = 2;
                }
                else if (item.Key.Contains("pH"))
                {
                    _pH = _pH - 1 > 0 ? _pH - 1 : 14;
                    dr[1] = _pH;
                    dr[2] = 14;
                    dr[3] = 0;
                }
                else if (item.Key.Contains("COD"))
                {
                    _COD = _COD - 100 > 500 ? _COD - 100 : 6000;
                    dr[1] = _COD;
                    dr[2] = 6000;
                    dr[3] = 500;
                }
                else if (item.Key.Contains("SS"))
                {
                    _SS = _SS - 50 > 200 ? _SS - 50 : 1500;
                    dr[1] = _SS;
                    dr[2] = 1500;
                    dr[3] = 200;
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
