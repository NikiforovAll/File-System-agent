using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace salesforce_notify
{
    public class Server
    {
        public string Prefixes { get; }
        private HttpListener _httpListener;
        private bool _isFirstRun = true;

        public Server(string prefixes)
        {
            this.Prefixes = prefixes;
        }
        public void Start(Action<HttpListenerContext> maiAction)
        {
            _httpListener = new HttpListener();
            if (_isFirstRun)
            {
                _isFirstRun = false;
                _httpListener.Prefixes.Add(Prefixes);
            }
            _httpListener.Start();
            Console.WriteLine($@"App started...  {Prefixes}");
            Listen(maiAction);
        }

        public void Stop()
        {
            Console.WriteLine(@"App stopped...");
            _httpListener.Stop();
        }

        private void Listen(Action<HttpListenerContext> func)
        {
            var result = _httpListener.BeginGetContext(ListenerCallback, func);
            result.AsyncWaitHandle.WaitOne();
        }

        private void ListenerCallback(IAsyncResult result)
        {
            var func = result.AsyncState as Action<HttpListenerContext>;
            func?.Invoke(_httpListener.EndGetContext(result));
            Listen(func);
        }

    }
}
