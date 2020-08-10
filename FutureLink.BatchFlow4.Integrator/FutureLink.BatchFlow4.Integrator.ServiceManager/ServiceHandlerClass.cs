using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using System.Xml.Serialization;
using FutureLink.BatchFlow4;

namespace FutureLink.BatchFlow4.Integrator.ServiceManager
{
    public class ServiceHandlerClass
    {
        private Timer _executeTimer = new Timer();
        protected Info.LoggingClass _logger = new Info.LoggingClass("FutureLink.BatchFlow4.Integrator.ServiceManager");


        private Info.ConfigurationInfo loadConfiguration()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Info.ConfigurationInfo));
                string fileName = getConfigurationFile();
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    Info.ConfigurationInfo config = (Info.ConfigurationInfo)serializer.Deserialize(stream);
                    return config;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "IntegratorClass", "loadConfiguration", ex);
                return null;

            }
        }

        private string getConfigurationFile()
        {
            string curPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(curPath, "configuration.xml");
        }

        async void _executeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _executeTimer.Stop();
                BLL.IntegratorClass ec = new BLL.IntegratorClass();
                Info.ConfigurationInfo config = loadConfiguration();                
                await ec.DoRun(config);
                _executeTimer.Interval = config.ExecutionInterval > 9 ? config.ExecutionInterval * 1000 : 5000;
                _executeTimer.Start();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.Message, "ServiceHandlerClass", "_executeTimer_Elapsed", ex);
                _executeTimer.Start();
            }
        }

        public void Start()
        {
            _logger.Trace("Service started", "ServiceHandlerClass", "Start");
            _executeTimer.Elapsed += _executeTimer_Elapsed;
            _executeTimer.Interval = (5000);
            _executeTimer.Start();
        }

        public void Stop()
        {
            _logger.Trace("Service stopped", "ServiceHandlerClass", "Stop");
            _executeTimer.Stop();
            _executeTimer.Elapsed -= _executeTimer_Elapsed;
        }

        public void Pause()
        {
            _executeTimer.Stop();
        }

        public void Continue()
        {
            _executeTimer.Start();
        }
    }


}
