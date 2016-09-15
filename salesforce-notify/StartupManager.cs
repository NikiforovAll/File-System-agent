using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace salesforce_notify
{
    public class StartupManager
    {
        public RegistryKey AppRegistryKey { get; private set; }
        public bool IsStartupEnabled
        {
            get { return AppRegistryKey?.GetValue(_appName) == null;}
            private set
            {
                
            }
        }

        private readonly string _appName;
        public StartupManager(string appName)
        {
            _appName = appName;
            AppRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        }

        public void Enable()
        {
            if(IsStartupEnabled) return;
            AppRegistryKey.SetValue(_appName, Application.ExecutablePath);
        }

        public void Disable()
        {
            if(!IsStartupEnabled) return;
            AppRegistryKey.DeleteValue(_appName);
        }
        

    }
}
