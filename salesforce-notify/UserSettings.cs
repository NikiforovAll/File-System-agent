using System.Configuration;

namespace salesforce_fileagent
{
    class UserSettings: ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue("8125")]
        public string Port
        {
            get { return (string)(this["Port"]); }
            set { this["Port"] = value; }
        }
        [UserScopedSetting()]
        [DefaultSettingValue("SFFM")]
        public string CertName
        {
            get { return (string)(this["CertName"]); }
            set { this["CertName"] = value; }
        }
    }
}
