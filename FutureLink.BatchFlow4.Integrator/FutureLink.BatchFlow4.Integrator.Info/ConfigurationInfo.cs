using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FutureLink.BatchFlow4.Integrator.Info
{
    [XmlRoot("ConfigurationInfo")]
    public class ConfigurationInfo
    {
        public ConfigurationInfo()
        {
            Items = new List<IntegrationConfigDetail>();
            ApiUrl = @"https://api.batchflow.dk";
        }

        [XmlElement("ApiUrl")]
        public string ApiUrl { get; set; }

        [XmlElement("ExecutionInterval")]
        public int ExecutionInterval { get; set; }
        [XmlElement("Item")]
        public List<IntegrationConfigDetail> Items { get; set; }
    }
}
