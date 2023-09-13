using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Sales.Interfaces
{
    public interface ICustomerEnquiryService
    {
        //public PaymentVoucher InsertPaymentVoucher(PaymentVoucher paymentVoucher,
        //  List<PaymentVoucherDetails> paymentVoucherDetails);

        public CustomerEnquiryModel InsertCustomerEnquiry(EnquiryMaster EnquiryMaster, List<EnquiryDetails> customerEnquiryDetails);
        public CustomerEnquiryModel UpdateCustomerEnquiry(EnquiryMaster EnquiryMaster, List<EnquiryDetails> customerEnquiryDetails);
        public int DeleteCustomerEnquiry(EnquiryMaster EnquiryMaster, List<EnquiryDetails> customerEnquiryDetails);
        public CustomerEnquiryModel GetSavedCustEnquiryDetails(string Qtno);
        public IEnumerable<EnquiryMaster> GetCustomerEnquiry();
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate);
         
        public VouchersNumbers GetVouchersNumbers(string Qtno);

    }
}



