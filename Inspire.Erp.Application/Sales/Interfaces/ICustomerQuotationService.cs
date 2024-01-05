using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Sales.Interfaces
{
    public interface ICustomerQuotationService
    {
        //public PaymentVoucher InsertPaymentVoucher(PaymentVoucher paymentVoucher,
        //  List<PaymentVoucherDetails> paymentVoucherDetails);

        public CustomerQuotationModel InsertCustomerQuotation(CustomerQuotation customerQuotation);
        public CustomerQuotationModel UpdateCustomerQuotation(CustomerQuotation customerQuotation);
        public int DeleteCustomerQuotation(CustomerQuotation customerQuotation);
        public CustomerQuotationModel GetSavedCustQuotationDetails(string Qtno); 
        public IEnumerable<CustomerQuotation> GetCustomerQuotation();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);

        public VouchersNumbers GetVouchersNumbers(string Qtno);

    }
}
