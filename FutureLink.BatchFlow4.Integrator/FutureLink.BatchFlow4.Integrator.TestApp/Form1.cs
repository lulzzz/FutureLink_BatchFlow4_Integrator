using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureLink.BatchFlow4.Integrator.TestApp
{
    public partial class Form1 : Form
    {
        protected Info.LoggingClass _logger = new Info.LoggingClass("Futurelink.BatchFlow4.Home6.TestApp");
        ServiceManager.ServiceHandlerClass _serviceHandler = new ServiceManager.ServiceHandlerClass();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            _serviceHandler.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _serviceHandler.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
