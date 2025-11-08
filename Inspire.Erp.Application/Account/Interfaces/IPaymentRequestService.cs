using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IPaymentRequestService
    {
        public PaymentRequest InsertPaymentRequest(
           PaymentRequest paymentRequest,
           List<PaymentRequestDetails> paymentRequestDetails);

        public PaymentRequest UpdatePaymentRequest(
            PaymentRequest paymentRequest,
            List<PaymentRequestDetails> paymentRequestDetails);

        public int DeletePaymentRequest(
            PaymentRequest paymentRequest,
            List<PaymentRequestDetails> paymentRequestDetails);
        public IEnumerable<PaymentRequest> GetPaymentRequests();
        public PaymentRequest GetSavedPaymentRequestDetails(int prNo);

        


    }

}
