using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.DTO.Customer_Enquiry
{
    public class EnquiryNoDto
    {
        public string EnquiryNOId { get; set; }
        public string EnquiryNO { get; set; }
    }
    public class EnquiryToBoqDto
    {
        public int EnquiryDetailId { get; set; }
        public string EnquiryId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemQty { get; set; }
        public decimal ItemRate { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
    }   
}
