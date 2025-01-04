using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
namespace Inspire.Erp.Web.ViewModels
{
    public partial class RFQViewModel1
    {

        public RfqEntry RfqEntry { get; set; }
        public List<RfqEntryDetails> RfqEntryDetails { get; set; }
    }
}