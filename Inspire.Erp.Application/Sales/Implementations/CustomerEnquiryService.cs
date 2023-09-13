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
    public class CustomerEnquiryService : ICustomerEnquiryService
    {
        private IRepository<EnquiryMaster> _customerEnquiryRepository;
        private IRepository<EnquiryDetails> _customerEnquiryDetailsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private readonly ILogger<CustomerEnquiryService> _logger;

        public CustomerEnquiryService(ILogger<CustomerEnquiryService> logger,
            IRepository<EnquiryMaster> customerEnquiryRepository, IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<EnquiryDetails> customerEnquiryDetailsRepository)
        {

            this._customerEnquiryRepository = customerEnquiryRepository;
            this._customerEnquiryDetailsRepository = customerEnquiryDetailsRepository;
            this._programsettingsRepository = programsettingsRepository;
            this._voucherNumbersRepository = voucherNumbersRepository;
            _logger = logger;

        }

        public CustomerEnquiryModel UpdateCustomerEnquiry(EnquiryMaster customerEnquiry, List<EnquiryDetails> customerEnquiryDetails)
        {

            try
            {
                _customerEnquiryRepository.BeginTransaction();

                _customerEnquiryRepository.Update(customerEnquiry);
                _customerEnquiryDetailsRepository.UpdateList(customerEnquiryDetails);
                _customerEnquiryRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _customerEnquiryRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedCustEnquiryDetails(customerEnquiry.EnquiryMasterEnquiryId);
        }

        public int DeleteCustomerEnquiry(EnquiryMaster customerEnquiry, List<EnquiryDetails> customerEnquiryDetails)
        {
            int i = 0;
            try
            {
                _customerEnquiryRepository.BeginTransaction();

                customerEnquiry.EnquiryMasterDelStatus = true;

                customerEnquiryDetails = customerEnquiryDetails.Select((k) =>
                {
                    k.EnquiryDetailsEnquiryDetailsDelStatus = true;
                    return k;
                }).ToList();

                _customerEnquiryDetailsRepository.UpdateList(customerEnquiryDetails);

                _customerEnquiryRepository.Update(customerEnquiry);



                _customerEnquiryRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _customerEnquiryRepository.TransactionRollback();
                i = 0;
                throw ex;
            }


            return i;

        }
        public CustomerEnquiryModel InsertCustomerEnquiry(EnquiryMaster customerEnquiry, List<EnquiryDetails> customerEnquiryDetails)
        {
            try
            {
                _customerEnquiryRepository.BeginTransaction();
                string customerEnquiryNumber = (customerEnquiry.EnquiryMasterEnquiryId == null || customerEnquiry.EnquiryMasterEnquiryId.Trim() == "") ?
                                              this.GenerateVoucherNo(customerEnquiry.EnquiryMasterEnquiryDate.Value).VouchersNumbersVNo : customerEnquiry.EnquiryMasterEnquiryId;
                customerEnquiry.EnquiryMasterEnquiryId = customerEnquiryNumber;

                customerEnquiryDetails = customerEnquiryDetails.Select((x) =>
                {
                    x.EnquiryDetailsEnquiryDetailsId = 0;
                    x.EnquiryDetailsEnquiryDetailsEquiryId = customerEnquiryNumber;
                    ////x.EnquiryDetailsEnquiryDetailsId = Convert.ToInt32(customerEnquiryNumber);
                    return x;
                }).ToList();
                _customerEnquiryDetailsRepository.InsertList(customerEnquiryDetails);

                _customerEnquiryRepository.Insert(customerEnquiry);
                _customerEnquiryRepository.TransactionCommit();
                return this.GetSavedCustEnquiryDetails(customerEnquiry.EnquiryMasterEnquiryId);

            }
            catch (Exception ex)
            {
                _customerEnquiryRepository.TransactionRollback();
                throw ex;
            }
        }

        public IEnumerable<EnquiryMaster> GetCustomerEnquiry()
        {
            IEnumerable<EnquiryMaster> customerEnquiry = _customerEnquiryRepository.GetAll().Where(k => k.EnquiryMasterDelStatus == false || k.EnquiryMasterDelStatus == null);
            return customerEnquiry;
        }
        public CustomerEnquiryModel GetSavedCustEnquiryDetails(string Qtno)
        {
            if (_voucherNumbersRepository.GetAsQueryable().Any(x => x.VouchersNumbersVNo == Qtno))
            {
                CustomerEnquiryModel voucherdata = _voucherNumbersRepository.GetAsQueryable().Where(x => x.VouchersNumbersVNo == Qtno)
                                                .Include(k => k.EnquiryDetails).AsEnumerable()
                                                .Select((k) => new CustomerEnquiryModel
                                                {
                                                    customerEnquiryDetails = k.EnquiryDetails.Where(x => x.EnquiryDetailsEnquiryDetailsDelStatus == false).ToList()
                                                })
                                                .SingleOrDefault();
                voucherdata.custEnquiry = _customerEnquiryRepository.GetAsQueryable().Where(k => k.EnquiryMasterEnquiryId == Qtno).FirstOrDefault();
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
                ////int mxc = 0;
                ////mxc = Convert.ToInt32(this._customerEnquiryRepository.GetAsQueryable().Where(k => k.EnquiryMasterEnquiryId != null).Select(x => x.EnquiryMasterEnquiryId).Max());
                ////if (mxc == null) { mxc = 1; } else { mxc = mxc + 1; }

                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.CE_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.Enquiry)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                VouchersNumbers vouchersNumbers = new VouchersNumbers


                ////VouchersNumbers vouchersNumbers = new VouchersNumbers
                {

                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    ////VouchersNumbersVNo = mxc.ToString(),
                    ////VouchersNumbersVNoNu = mxc,
                    VouchersNumbersVType = VoucherType.Enquiry,
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
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == Qtno && k.VouchersNumbersVType == VoucherType.Enquiry).SingleOrDefault();
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
