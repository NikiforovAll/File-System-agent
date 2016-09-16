using System;
using System.Windows.Forms;

namespace salesforce_fileagent
{
    public partial class SalesforceNotifyForm: Form
    {
        public Server Server { get; private set; }
        public SalesforceNotifyForm()
        {
            InitializeComponent();
        }

        private void SalesforceNotifyForm_Load(object sender, EventArgs args)
        {
            //Server.Start();   
        }

    }
}
