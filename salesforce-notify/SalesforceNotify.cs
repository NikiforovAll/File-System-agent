using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using salesforce_notify.Properties;
using System.Configuration;
using System.Net;
using System.Web;
using System.IO;
namespace salesforce_notify
{
    public class SalesforceNotify : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly Server _server;

        public SalesforceNotify()
        {
            var contextMenuStrip = new ContextMenuStrip()
            {
                Items = { new ToolStripMenuItem("Exit", null, new EventHandler(Exit)) }
            };
            _trayIcon = new NotifyIcon()
            {
                Icon = Resources.AppIcon,
                Visible = true,
                ContextMenuStrip = contextMenuStrip
            };

            _server = new Server(ConfigurationManager.AppSettings["listner-prefix"]);
            _server.Start((listnerContext) =>
            {
                HttpListenerRequest request = listnerContext.Request;
                var urlKeyValueParameters = request.Url.ParseQueryString();
                string path, pathOrigin = null;
                if (!urlKeyValueParameters.TryGetValue("path", out path))
                {
                    path = "url is invalid";
                }
                else
                {
                    path = HttpUtility.UrlDecode(path);
                    Console.WriteLine(path);
                    if (IsItemExists(out pathOrigin, path))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = path,
                            UseShellExecute = true,
                            Verb = "open"
                        });
                    }
                    else
                    {
                        pathOrigin = "Item not found";
                    }
                }
                NotifyUserBalloon(_trayIcon, $"{pathOrigin} : {path}");

            });
        }

        private void NotifyUserBalloon(NotifyIcon icon, string text)
        {
            icon.BalloonTipText = text;
            icon.BalloonTipIcon = ToolTipIcon.Info;
            icon.ShowBalloonTip(500);
        }

        private bool IsItemExists(out string type, string path)
        {
            path = path
                .Replace("\"", "")
                .Replace("/", "");
            type = File.Exists(path)
                         ? "File"
                         : Directory.Exists(path)
                             ? "Directory"
                             : "";
            return !type.Equals(String.Empty);
        }
        private void Exit(object sender, EventArgs args)
        {
            _server.Stop();
            _trayIcon.Visible = false;
            Application.Exit();
        }

    }
}
