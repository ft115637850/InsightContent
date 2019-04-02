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
        private IDictionary<string, IList<WebSocket>> subscriptionList = new Dictionary<string, IList<WebSocket>>();
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
            foreach (var item in subscriptionList)
            {
                var dr = dt.NewRow();
                dr[0] = item.Key;
                if (item.Key.Contains("SysTimeSec"))
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
                    ws.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                dt.Clear();
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

        public void Subscribe(string requestData, WebSocket ws)
        {
            if (requestData == string.Empty)
            {
                return;
            }

            var dt = JsonConvert.DeserializeObject<DataTable>(requestData);
            foreach (DataRow dr in dt.Rows)
            {
                var topic = Convert.ToString(dr[0]);
                if (this.subscriptionList.ContainsKey(topic))
                {
                    this.subscriptionList[topic].Add(ws);
                }
                else
                {
                    this.subscriptionList.Add(topic, new List<WebSocket>() { ws });
                }
            }
        }

        public void Unsubscribe(string requestData)
        {
            if (requestData == string.Empty)
            {
                return;
            }
            /*
            var dt = JsonConvert.DeserializeObject<DataTable>(requestData);
            foreach (DataRow dr in dt.Rows)
            {
                var topic = Convert.ToString(dr[0]);
                if (this.subscriptionList.ContainsKey(topic))
                {
                    this.subscriptionList.Remove(topic);
                }
            }*/
        }
    }
}
