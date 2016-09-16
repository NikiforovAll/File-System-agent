using System;
using System.Windows.Forms;
using salesforce_notify.Properties;
using System.Configuration;
using System.Drawing;
using System.Net;
using System.Web;
using System.IO;

namespace salesforce_notify
{
    public class SalesforceFileAgent : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly Server _server;
        public SalesforceFileAgent()
        {
            const int TRAY_WIDTH = 120;
            const int TRAY_HEIGHT = 88; 
            var contextMenuStrip = new ContextMenuStrip
            {
                Items =
                {
                    new ToolStripMenuItem("Start", Resources.play, StartServer)
                    {
                        Width = TRAY_WIDTH,
                        AutoSize = false
                    },
                    new ToolStripMenuItem("Stop", Resources.stop, StopServer)
                    {
                        Width = TRAY_WIDTH,
                        AutoSize = false
                    },
                    new ToolStripMenuItem("Startup", Resources.cogwheel)
                        {
                            DropDownItems =
                            {
                                new ToolStripMenuItem("Enable", null, EnableAppStartup),
                                new ToolStripMenuItem("Disable", null, DisableAppStartup)
                            },
                            AutoSize = false,
                            Width = TRAY_WIDTH
                        },
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Exit", Resources.exit, Exit)
                    {
                        Width = TRAY_WIDTH,
                        AutoSize = false
                    },
                },
                AutoSize = false,
                BackColor = Color.Beige,
                Width = TRAY_WIDTH,
                Height = TRAY_HEIGHT
            };
            //contextMenuStrip.PerformLayout();
            _trayIcon = new NotifyIcon
            {
                Icon = Resources.AppIcon,
                Visible = true,
                ContextMenuStrip = contextMenuStrip
            };
//            _startupManager = new StartupManager(ConfigurationManager.AppSettings["salesforce-fileagent"]);
            _trayIcon.MouseClick += ToggleServerStatus;
            _server = new Server(ConfigurationManager.AppSettings["listner-prefix"]);
            _server.Start(listnerContext =>
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
                    var operationResult = OpenFileSystemItem(path);
                    path = operationResult.Item1;
                    pathOrigin = operationResult.Item2;
                }
                NotifyUserBalloon(_trayIcon, $"{pathOrigin} {path}");
            });
        }

        private void ToggleServerStatus(object sender, MouseEventArgs e)
        {
            if(e.Button != MouseButtons.Left)
                return;
            if (_server.IsRunning)
            {
                StopServer(sender, e);
            }
            else
            {
                StartServer(sender, e);
            }
        }

        private void NotifyUserBalloon(NotifyIcon icon, string text)
        {
            icon.BalloonTipText = text;
            icon.BalloonTipIcon = ToolTipIcon.Info;
            icon.ShowBalloonTip(500);
        }

        private Tuple<string, string> OpenFileSystemItem(string path)
        {
            string pathOrigin;
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
            return new Tuple<string, string>(path, pathOrigin);
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
            _trayIcon.Visible = false;
            Application.Exit();
        }

        private void StartServer(object sender, EventArgs args)
        {
            if (!_server.IsRunning)
                _trayIcon.Icon = Resources.AppIcon;
            _server.Start();
        }
        private void StopServer(object sender, EventArgs args)
        {
            _trayIcon.Icon = Resources.logo_r;
            _server.Stop();
        }

        private void EnableAppStartup(object sender, EventArgs args)
        {
            if (StartUpManager.IsUserAdministrator())
            {
                StartUpManager.AddApplicationToAllUserStartup();
            }
            else
            {
                StartUpManager.AddApplicationToCurrentUserStartup();
            }
            NotifyUserBalloon(_trayIcon, "Startup enabled");
            Console.WriteLine("Startup enabled");
        }
        private void DisableAppStartup(object sender, EventArgs args)
        {
            if (StartUpManager.IsUserAdministrator())
            {
                StartUpManager.RemoveApplicationFromAllUserStartup();
            }
            else
            {
                StartUpManager.RemoveApplicationFromCurrentUserStartup();
            }
            NotifyUserBalloon(_trayIcon, "Startup disabled");
            Console.WriteLine("Startup disabled");
        }
    }
}
