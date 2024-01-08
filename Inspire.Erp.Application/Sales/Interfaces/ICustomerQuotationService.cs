using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public Task<Response<List<GetCustQuotationForSaleOrderResponse>>> GetCustomerQuotationForSaleOrder();

    }
}
