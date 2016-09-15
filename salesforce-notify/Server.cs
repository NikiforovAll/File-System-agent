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
        private bool _isFirstRun = true;
        private readonly HttpListener _httpListener;
        public string Prefixes { get; }
        private Action<HttpListenerContext> _mainAction; 
        public bool IsRunning { get; private set;}
        public Server(string prefixes)
        {
            this.Prefixes = prefixes;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(Prefixes);
        }
        public void Start(Action<HttpListenerContext> maiAction)
        {

            _mainAction = maiAction;
            if (_isFirstRun)
            {
                _isFirstRun = false;
            }
            _httpListener.Start();
            IsRunning = true;
            Console.WriteLine($@"App started...  {Prefixes}");
            Listen(maiAction);
        }

        public void Start()
        {
            if (!IsRunning )
            {
                this.Start(_mainAction);
            }
            else
            {
                Console.WriteLine("Already started...");
            }
        }

        public void Stop()
        {
            IsRunning = false;
            Console.WriteLine(@"App stopped...");
            _httpListener.Stop();
        }

        private void Listen(Action<HttpListenerContext> func)
        {
            _httpListener.BeginGetContext(ListenerCallback, func);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (!_httpListener.IsListening) return;
            var func = result.AsyncState as Action<HttpListenerContext>;
            func?.Invoke(_httpListener.EndGetContext(result));
            Listen(func);
        }

    }
}
