using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Models.Approvals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.RFQ.Interface
{
    public interface IRfqService
    {
        public IEnumerable<RfqEntry> GetRfq();
        public RfqEntry GetRfqById(int? id);
        public Task<RfqEntry> InsertRfq(RfqEntry RfqEntry);
        public Task<RfqEntry> UpdatetRfq(RfqEntry RfqEntry);

        public IEnumerable<RfqEntry> DeleteRfq(string Id);
        
    }
}
