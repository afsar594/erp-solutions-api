using System;
using System.Collections.Generic;

namespace Inspire.Erp.Domain.Entities
{
    public partial class GeneralSettings
    {
        public int Id { get; set; }
        public string KeyValue { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ValueType { get; set; }
        public bool? BoolValue { get; set; }
        public double? NumValue { get; set; }
        public string TextValue { get; set; }
        public string TableName { get; set; }
         public bool? DelStatus { get; set; }
    }
}
