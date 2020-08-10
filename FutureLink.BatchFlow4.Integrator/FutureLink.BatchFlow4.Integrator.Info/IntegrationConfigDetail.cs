using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FutureLink.BatchFlow4.Integrator.Info
{
    public class IntegrationConfigDetail
    {
        [XmlElement("Type")]
        public string Type { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }
        [XmlElement("Destination")]
        public string Destination { get; set; }
        [XmlElement("Source")]
        public string Source { get; set; }
        [XmlElement("Succeeded")]
        public string Succeeded { get; set; }
        [XmlElement("Failed")]
        public string Failed { get; set; }
        [XmlElement("Format")]
        public string Format { get; set; }
        [XmlElement("Serial")]
        public string Serial { get; set; }
    }
}
