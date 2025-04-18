using Inspire.Erp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspire.Erp.Domain.Entities
{
	public class OrderApprovalHierarchy
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public decimal OrderId { get; set; }
		[Required]
		public EnumOrderForms OrderType { get; set; }
		[Required]
		public long ApproverId { get; set; }
		[Required]
		public EnumApprovalStatus Status { get; set; }

		[Required]
        public string Remarks { get; set; }

        [ForeignKey("OrderId")]
		public virtual PurchaseOrder PurchaseOrders { get; set; }
		[ForeignKey("OrderId")]
		public virtual PurchaseRequisition PurchaseRequisitions { get; set; }
	}
}
