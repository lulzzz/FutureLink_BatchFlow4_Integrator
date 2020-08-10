using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace FutureLink.BatchFlow4.Integrator.Info
{
    public class SqlParamInfo
    {
        public string ParamName { get; set; }
        public DbType ParamType { get; set; }
        public ParameterDirection ParamDirection { get; set; }
        public object DefaultValue { get; set; }
    }
}
