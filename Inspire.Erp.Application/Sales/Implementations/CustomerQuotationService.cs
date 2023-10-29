using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Inspire.Erp.Application.Sales.Implementations
{
    public class CustomerQuotationService : ICustomerQuotationService
    {
        private IRepository<CustomerQuotation> _customerQuotationRepository;
        private IRepository<CustomerQuotationDetails> _customerQuotationDetailsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;  
        private IRepository<UserTracking> _userTrackingRepository;
        private readonly ILogger<CustomerQuotationService> _logger;

        public CustomerQuotationService(ILogger<CustomerQuotationService> logger,
            IRepository<CustomerQuotation> customerQuotationRepository, IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<CustomerQuotationDetails> customerQuotationDetailsRepository,
            IRepository<UserTracking> userTrackingRepository)
        {

            this._customerQuotationRepository = customerQuotationRepository;
            this._customerQuotationDetailsRepository = customerQuotationDetailsRepository;
            this._voucherNumbersRepository = voucherNumbersRepository;
            this._programsettingsRepository = programsettingsRepository;
            _logger = logger;
            this._userTrackingRepository = userTrackingRepository;
            

        }

        public CustomerQuotationModel UpdateCustomerQuotation(CustomerQuotation customerQuotation, List<CustomerQuotationDetails> customerQuotationDetails)
        {

            try
            {
                _customerQuotationRepository.BeginTransaction();

                _customerQuotationRepository.Update(customerQuotation);
                _customerQuotationDetailsRepository.UpdateList(customerQuotationDetails);


                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = customerQuotation.CustomerQuotationQuotationId;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "CustomerQuotation";
                _userTrackingRepository.Insert(trackingData);
                _customerQuotationRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _customerQuotationRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedCustQuotationDetails(customerQuotation.CustomerQuotationQuotationId);
        }

        public int DeleteCustomerQuotation(CustomerQuotation customerQuotation, List<CustomerQuotationDetails> customerQuotationDetails)
        {
            int i = 0;
            try
            {
                _customerQuotationRepository.BeginTransaction();

                customerQuotation.CustomerQuotationDelStatus = true;

                customerQuotationDetails = customerQuotationDetails.Select((k) =>
                {
                    k.CustomerQuotationDetailsDelStatus = true;
                    return k;
                }).ToList();

                _customerQuotationDetailsRepository.UpdateList(customerQuotationDetails);

                _customerQuotationRepository.Update(customerQuotation);



                _customerQuotationRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _customerQuotationRepository.TransactionRollback();
                i = 0;
                throw ex;
            }


            return i;

        }
        public CustomerQuotationModel InsertCustomerQuotation(CustomerQuotation customerQuotation, List<CustomerQuotationDetails> customerQuotationDetails)
        {
            try
            {
                _customerQuotationRepository.BeginTransaction();
                string customerQuotationNumber = (customerQuotation.CustomerQuotationQuotationId == null || customerQuotation.CustomerQuotationQuotationId.Trim() == "") ?
                                              this.GenerateVoucherNo(customerQuotation.CustomerQuotationQuotationDate.Value).VouchersNumbersVNo : customerQuotation.CustomerQuotationQuotationId;
                customerQuotation.CustomerQuotationQuotationId = customerQuotationNumber;

                customerQuotationDetails = customerQuotationDetails.Select((x) =>
                {
                    x.CustomerQuotationDetailsId = 0;
                    x.CustomerQuotationDetailsQuotationId = customerQuotationNumber;
                    return x;
                }).ToList();
                _customerQuotationDetailsRepository.InsertList(customerQuotationDetails);

                _customerQuotationRepository.Insert(customerQuotation);
               
                
                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = customerQuotation.CustomerQuotationQuotationId;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "CustomerQuotation";
                _userTrackingRepository.Insert(trackingData);
                
                _customerQuotationRepository.TransactionCommit();
                return this.GetSavedCustQuotationDetails(customerQuotation.CustomerQuotationQuotationId);

            }
            catch (Exception ex)
            {
                _customerQuotationRepository.TransactionRollback();
                throw ex;
            }



        }

        public IEnumerable<CustomerQuotation> GetCustomerQuotation()
        {
            IEnumerable<CustomerQuotation> customerQuotation = _customerQuotationRepository.GetAll().Where(k => k.CustomerQuotationDelStatus == false || k.CustomerQuotationDelStatus == null);
            return customerQuotation;
        }
        public CustomerQuotationModel GetSavedCustQuotationDetails(string Qtno)
             {
            if (_voucherNumbersRepository.GetAsQueryable().Any(x => x.VouchersNumbersVNo == Qtno))
            {
                CustomerQuotationModel voucherdata = _voucherNumbersRepository.GetAsQueryable().Where(x => x.VouchersNumbersVNo == Qtno)
                                                .Include(k => k.CustomerQuotationDetails).AsEnumerable()
                                                .Select((k) => new CustomerQuotationModel
                                                {
                                                    customerQuotationDetails = k.CustomerQuotationDetails.Where(x => x.CustomerQuotationDetailsDelStatus == false).ToList()
                                                })
                                                .SingleOrDefault();
                voucherdata.custQuotation = _customerQuotationRepository.GetAsQueryable().Where(k => k.CustomerQuotationQuotationId == Qtno).FirstOrDefault();
                return voucherdata;

            }
            return null;

        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                ////var vno=  this._paymentVoucherRepository.GetAsQueryable().Where(k => k.PaymentVoucherVoucherNo == null).FirstOrDefault();
                ///
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.CQ_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.Quotation)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                VouchersNumbers vouchersNumbers = new VouchersNumbers

                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.Quotation,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public VouchersNumbers GetVouchersNumbers(string Qtno)
        {
            try
            {
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == Qtno && k.VouchersNumbersVType == VoucherType.Quotation).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }
    }
}


