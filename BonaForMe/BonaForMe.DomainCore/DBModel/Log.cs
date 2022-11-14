using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonaForMe.DomainCore.DBModel
{
    public class Log : BaseEntity
    {
        public Guid LOG_FIELD_ID { get; set; }
        public Guid LOG_USERID { get; set; }
        public string LOG_FIELD_NAME { get; set; }
        public string LOG_PROCESS { get; set; }
        public byte LOG_TYPE { get; set; }
        public string LOG_FUNCTIONNAME { get; set; }
        public string LOG_ERROR { get; set; }
        public string LOG_URL { get; set; }
        public bool LOG_SYSTEM { get; set; }
    }
}
