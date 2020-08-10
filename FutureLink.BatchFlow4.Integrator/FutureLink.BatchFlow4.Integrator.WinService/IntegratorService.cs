using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace FutureLink.BatchFlow4.Integrator.WinService
{
    public partial class IntegratorService : ServiceBase
    {
        private ServiceManager.ServiceHandlerClass _serviceHandler = new ServiceManager.ServiceHandlerClass();

        public IntegratorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _serviceHandler.Start();
        }

        protected override void OnStop()
        {
            _serviceHandler.Stop();
        }

        protected override void OnPause()
        {
            _serviceHandler.Pause();
        }

        protected override void OnContinue()
        {
            _serviceHandler.Continue();
        }

        protected override void OnShutdown()
        {
            _serviceHandler.Stop();
        }

    }
}
