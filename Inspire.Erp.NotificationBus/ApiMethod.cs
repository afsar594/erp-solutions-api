using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.NotificationBus
{
    public class ApiMethod
    {
        public Http HttpType { get; set; }
        public string Url { get; set; }
        public object Body { get; set; }
        public string DocType { get; set; }

    }

    public enum Http
    {
        POST = 1,
        GET,
        PUT,
        DELETE

    }
}
