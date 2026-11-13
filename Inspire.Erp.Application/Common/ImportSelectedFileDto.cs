using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Common
{
    public class ImportSelectedFileDto
    {
        public int PrId { get; set; }
        public int PoId { get; set; }
        public List<int> PrFileIds { get; set; } = new List<int>();
    }
}
