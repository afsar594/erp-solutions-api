using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Sales.Interfaces
{
    public interface IPosSalesVoucherService
    {
        public POS_Sales_Voucher InsertPosSalesVoucher(POS_Sales_Voucher salesVoucher, List<POS_Sales_Voucher_Details> PosSalesVoucherDetails);
        public POS_Sales_Voucher UpdatePosSalesVoucher(POS_Sales_Voucher salesVoucher, List<POS_Sales_Voucher_Details> PosSalesVoucherDetails);
        public POS_Sales_Voucher GetSavedSalesVoucherDetails(string pvno);
        public IEnumerable<POS_Sales_Voucher> GetPosSalesVoucher();
    }
}
