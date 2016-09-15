using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace salesforce_notify
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
