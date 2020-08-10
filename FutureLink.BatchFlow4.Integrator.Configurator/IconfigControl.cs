using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutureLink.BatchFlow4.Integrator.Configurator
{
    interface IconfigControl
    {
        Info.IntegrationConfigDetail TaskInfo { get; set; }
        bool NewTask { get; set; }
        bool HasChanges { get; }
    }
}
