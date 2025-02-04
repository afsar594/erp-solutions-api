using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
    public class ComplaintMaster
    {
        [Key]
        public int Id { get; set; }

      
        public int BuildingId { get; set; }
        public int FlatId { get; set; }
        public int CompanyId { get; set; }
        public string FullName { get; set; }

        public string ApartmentNo { get; set; }
        public string BuildingName { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }
        public string ComplaintType { get; set; }

        public string Comments { get; set; }
        public string PhotoPath { get; set; }
        public string Status { get; set; }

        public DateTime ComplaintDate { get; set; }
    }
}
