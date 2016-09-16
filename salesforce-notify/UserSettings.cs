using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesforce_notify
{
    class UserSettings: ApplicationSettingsBase
    {
        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("8125")]
        public string Port
        {
            get { return (string)(this["Port"]); }
            set { this["Port"] = value; }
        }

    }
}
