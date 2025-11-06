using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inspire.Erp.Application.Account.Services
{
    public class PaymentRequestService : IPaymentRequestService
    {
        private readonly IRepository<PaymentRequest> _paymentReqRepository;
        private readonly IRepository<PaymentRequestDetails> _paymentReqDetailsRepository;
        private readonly InspireErpDBContext context;
        private IRepository<Domain.Entities.ProgramSettings> _programsettingsRepository;

        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        public PaymentRequestService(
            IRepository<PaymentRequest> paymentRequestRepo,
            IRepository<Domain.Entities.ProgramSettings> programsettingsRepository,
            IRepository<PaymentRequestDetails> paymentRequestDetailsRepo,
            IRepository<VouchersNumbers> voucherNumbersRepository)
        {
            _paymentReqRepository = paymentRequestRepo;
            _paymentReqDetailsRepository = paymentRequestDetailsRepo;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbersRepository;
        }

        public PaymentRequest InsertPaymentRequest(
            PaymentRequest paymentRequest,
            List<PaymentRequestDetails> paymentRequestDetails)
        {
            try
            {
                string paymentVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.Payment, Prefix.PRequest_Prefix, _programsettingsRepository, _voucherNumbersRepository); ;
                paymentRequest.PaymentRequestVoucherNo = paymentVoucherNumber;
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _paymentReqRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PaymentRequestSno) + 1);
                _paymentReqRepository.Insert(paymentRequest);

                
                _paymentReqRepository.TransactionCommit();
                return this.GetSavedPaymentRequestDetails(paymentRequest.PaymentRequestSno);
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting Payment Request", ex);
            }


        }

        public PaymentRequest UpdatePaymentRequest(
            PaymentRequest paymentRequest,
            List<PaymentRequestDetails> paymentRequestDetails)
        {
            try
            {
                _paymentReqRepository.Update(paymentRequest);
                return this.GetSavedPaymentRequestDetails(paymentRequest.PaymentRequestSno); ;
            }
            catch (Exception ex) {
                throw ex;
            }

        }

        public int DeletePaymentRequest(
            PaymentRequest paymentRequest,
            List<PaymentRequestDetails> paymentRequestDetails)
        {
            int i = 0;
            try
            {
                _paymentReqRepository.BeginTransaction();
                paymentRequest.PaymentRequestDelStatus = true;
                paymentRequestDetails = paymentRequestDetails.Select((k) =>
                {
                    k.PaymentVocherDetailsDelStatus = true;
                    return k;
                }).OrderBy(x => x.PaymentRequestDetailsVoucherNo).AsEnumerable().ToList();
                _paymentReqRepository.Update(paymentRequest);
                _paymentReqDetailsRepository.UpdateList(paymentRequestDetails);
                _paymentReqRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _paymentReqRepository.TransactionRollback();
                i = 0;
                throw ex;
            }
            return i;
        }

        public PaymentRequest GetSavedPaymentRequestDetails(int prNo)
        {
            PaymentRequest voucherdata = new PaymentRequest();

            voucherdata = _paymentReqRepository.GetAsQueryable().Where(k => k.PaymentRequestSno == prNo).FirstOrDefault();
            if (voucherdata != null)
            {
                voucherdata.PaymentRequestDetails = _paymentReqDetailsRepository.GetAll().Where(x => x.PaymentRequestDetailsVoucherNo == voucherdata.PaymentRequestVoucherNo && x.PaymentVocherDetailsDelStatus == false).ToList();
                return voucherdata;
            }
            return null;

        }

        public IEnumerable<PaymentRequest> GetPaymentRequests()
        {
            // Return only active (non-deleted) payment requests
            return _paymentReqRepository.GetAsQueryable()
                .Where(x => x.PaymentRequestDelStatus != true)
                .OrderByDescending(x => x.PaymentRequestVoucherNo);
        }

    }
}
