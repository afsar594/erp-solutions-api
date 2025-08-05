////using System;
////using System.Collections.Generic;
////using System.Text;

////namespace Inspire.Erp.Application.Master.Implementations
////{
////    class NewCustomerService
////    {
////    }
////}

using Inspire.Erp.Application.Master.Interfaces;
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

namespace Inspire.Erp.Application.Account.Implementations
{
    public class NewCustomerService : INewCustomerService
    {
        private IRepository<CustomerMaster> _customerMasterRepository;
        private IRepository<CustomerContacts> _customerContactsRepository;
        private IRepository<CustomerDepartments> _customerDepartmentsRepository;
        private readonly IRepository<MasterAccountsTable> _masterAccountTable;
        ////private IRepository<ProgramSettings> _programsettingsRepository;
        ////private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<NewCustomerService> _logger;

        public NewCustomerService(IRepository<CustomerMaster> customerMasterRepository, IRepository<CustomerContacts> customerContactsRepository,
            IRepository<CustomerDepartments> customerDepartmentsRepository, IRepository<MasterAccountsTable> masterAccountTable, ILogger<NewCustomerService> logger)
        {
            this._customerMasterRepository = customerMasterRepository;
            this._masterAccountTable = masterAccountTable;
            this._customerContactsRepository = customerContactsRepository;
            this._customerDepartmentsRepository = customerDepartmentsRepository;
            _logger = logger;

        }

        public CustomerViewModel UpdateNewCustomer(CustomerMaster customerMaster, List<CustomerContacts> customerContacts, List<CustomerDepartments> customerDepartments)
        {

            try
            {
                if (customerMaster.IsCreateAccount.HasValue && (bool)customerMaster.IsCreateAccount)
                {
                    var MaSno = Convert.ToInt32(this._masterAccountTable.GetAsQueryable()
                                      .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                      .DefaultIfEmpty()
                                      .Max(o => o == null ? 0 : o.MaSno)) + 1;

                    var debtorAcc = this._masterAccountTable.GetAsQueryable()
                                    .FirstOrDefault(x => x.MaAccNo == customerMaster.CustomerMasterCustomerGroupAccNo);


                    var custMasterAccNo = this._masterAccountTable.GetAsQueryable()
                        .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == debtorAcc.MaAccNo)
                        .DefaultIfEmpty()
                        .Max(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, o.MaAccNo.Length))) + 1;

                    var custMasterAcc = new MasterAccountsTable
                    {
                        MaSno = MaSno,
                        MaRelativeNo = customerMaster.CustomerMasterCustomerGroupAccNo,
                        MaAccNo = customerMaster.CustomerMasterCustomerGroupAccNo + String.Format("{0:000}", custMasterAccNo),
                        MaAccName = customerMaster.CustomerMasterCustomerName,
                        MaAccType = AccountMasterStatus.Account,
                        MaMainHead = debtorAcc.MaMainHead,
                        MaSubHead = debtorAcc.MaSubHead,
                        MaFsno = 1,
                        MaStatus = AccountStatus.AccStatus,
                        MaUserId = 0,
                        MaDateCreated = DateTime.Now,
                        MaOpenBalance = 0,//need to add this 
                        MaTotalDebit = 0,//need to add this
                        MaTotalCredit = 0, //need to add this 
                        MaAssetDate = DateTime.Now
                    };

                    this._masterAccountTable.Insert(custMasterAcc);
                    customerMaster.CustomerMasterCustomerReffAcNo = custMasterAcc.MaAccNo;
                }
                _customerMasterRepository.BeginTransaction();

                _customerMasterRepository.Update(customerMaster);
                _customerContactsRepository.UpdateList(customerContacts);
                _customerDepartmentsRepository.UpdateList(customerDepartments);
                _customerMasterRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _customerMasterRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedCustomers(customerMaster.CustomerMasterCustomerNo);
        }

        public int DeleteNewCustomers(CustomerMaster customerMaster, List<CustomerContacts> customerContacts, List<CustomerDepartments> customerDepartments)
        {
            int i = 0;
            try
            {
                _customerMasterRepository.BeginTransaction();

                customerMaster.CustomerMasterCustomerDeleteStatus = true;
                customerMaster.CustomerMasterCustomerDelStatus = true;

                customerContacts = customerContacts.Select((k) =>
                {
                    k.CustomerContactsDelStatus = true;
                    return k;
                }).OrderBy(x => x.CustomerContactsName).AsEnumerable().ToList();

                _customerContactsRepository.UpdateList(customerContacts);

                customerDepartments = customerDepartments.Select((k) =>
                {
                    k.CustomerDepartmentsDelStatus = true;
                    return k;
                }).OrderBy(x => x.CustomerDepartmentsDelStatus).AsEnumerable().ToList();
                _customerDepartmentsRepository.UpdateList(customerDepartments);

                var account = _masterAccountTable.GetAsQueryable().Where(a => a.MaAccNo == customerMaster.CustomerMasterCustomerReffAcNo).FirstOrDefault();
                if (account != null)
                {
                    _masterAccountTable.Delete(account);
                }

                _customerMasterRepository.Update(customerMaster);

                _customerMasterRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _customerMasterRepository.TransactionRollback();
                i = 0;
                throw ex;
            }


            return i;

        }
        public CustomerViewModel InsertNewCustomer(CustomerMaster customerMaster, List<CustomerContacts> customerContacts, List<CustomerDepartments> customerDepartments)
        {
            try
            {
                _customerMasterRepository.BeginTransaction();

                ////int OBtVoucherNumber = (openingVoucher.OpeningVoucherMasterId == null || openingVoucher.OpeningVoucherMasterId == 0) ?
                ////              this.GenerateVoucherNo(openingVoucher.OpeningVoucherMasterOpBDate.Value).VouchersNumbersVNo :  openingVoucher.OpeningVoucherMasterId;
                ////openingVoucher.OpeningVoucherMasterId = OBtVoucherNumber;


                int mxc = 0;
                mxc = _customerMasterRepository.GetAsQueryable().Where(k => k.CustomerMasterCustomerNo != null)
                            .DefaultIfEmpty()
                            .Max(o => o == null ? 0 : o.CustomerMasterCustomerNo) + 1;

                ////VouchersNumbers vouchersNumbers = new VouchersNumbers
                ////{
                ////    VouchersNumbersVNo = "OB" + Convert.ToString(mxc),
                ////    VouchersNumbersVNoNu = mxc,
                ////    VouchersNumbersVType = VoucherType.OBVoucher,
                ////    VouchersNumbersFsno = 1,
                ////    VouchersNumbersStatus = AccountStatus.Pending,
                ////    VouchersNumbersVoucherDate = openingVoucher.OpeningVoucherMasterOpBDate.Value

                ////};
                ////_voucherNumbersRepository.Insert(vouchersNumbers);


                customerMaster.CustomerMasterCustomerNo = mxc;

                ////string vNo = "";
                ///
                if (customerContacts != null)
                {
                    customerContacts = customerContacts.Select((x) =>
                    {
                        x.CustomerContactsCustomerId = mxc;
                        return x;
                    }).OrderBy(x => x.CustomerContactsName).AsEnumerable().ToList();
                    _customerContactsRepository.InsertList(customerContacts);

                };

                if (customerDepartments != null)
                {
                    customerDepartments = customerDepartments.Select((x) =>
                    {
                        x.CustomerDepartmentsCustomerId = mxc;
                        return x;
                    }).OrderBy(x => x.CustomerDepartmentsCustomerId).AsEnumerable().ToList();

                    _customerDepartmentsRepository.InsertList(customerDepartments);

                }

                if (customerMaster.IsCreateAccount.HasValue && (bool)customerMaster.IsCreateAccount)
                {
                    var MaSno = Convert.ToInt32(this._masterAccountTable.GetAsQueryable()
                                      .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                      .DefaultIfEmpty()
                                      .Max(o => o == null ? 0 : o.MaSno)) + 1;

                    var debtorAcc = this._masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == customerMaster.CustomerMasterCustomerGroupAccNo);
                    //.FirstOrDefault(x => x.MaAccName.ToUpper() == "Debtors".ToUpper());

                    var custMasterAccNo = this._masterAccountTable.GetAsQueryable()
                        .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == debtorAcc.MaAccNo)
                        .DefaultIfEmpty()
                        .Max(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, o.MaAccNo.Length))) + 1;

                    var custMasterAcc = new MasterAccountsTable
                    {
                        MaSno = MaSno,
                        //MaRelativeNo = debtorAcc.MaAccNo,
                        MaRelativeNo = customerMaster.CustomerMasterCustomerGroupAccNo,
                        //MaAccNo = debtorAcc.MaAccNo + String.Format("{0:000}", custMasterAccNo),
                        MaAccNo = customerMaster.CustomerMasterCustomerGroupAccNo + String.Format("{0:000}", custMasterAccNo),
                        MaAccName = customerMaster.CustomerMasterCustomerName,
                        MaAccType = AccountMasterStatus.Account,
                        MaMainHead = debtorAcc.MaMainHead,
                        MaSubHead = debtorAcc.MaSubHead,
                        MaFsno = 1,
                        MaStatus = AccountStatus.AccStatus,
                        MaUserId = 0,
                        MaDateCreated = DateTime.Now,
                        MaOpenBalance = 0,//need to add this 
                        MaTotalDebit = 0,//need to add this
                        MaTotalCredit = 0, //need to add this 
                        MaAssetDate = DateTime.Now
                    };

                    this._masterAccountTable.Insert(custMasterAcc);
                    customerMaster.CustomerMasterCustomerReffAcNo = custMasterAcc.MaAccNo;
                }

                _customerMasterRepository.Insert(customerMaster);
                _customerMasterRepository.TransactionCommit();

                return this.GetSavedCustomers(customerMaster.CustomerMasterCustomerNo);

            }
            catch (Exception ex)
            {
                _customerMasterRepository.TransactionRollback();
                throw ex;
            }

        }

        public IEnumerable<CustomerMaster> GetAllCustomers()
        {
            IEnumerable<CustomerMaster> customerMasters = _customerMasterRepository.GetAll().Where(k => k.CustomerMasterCustomerDelStatus == false || k.CustomerMasterCustomerDelStatus == null);
            customerMasters = customerMasters.OrderBy(o => o.CustomerMasterCustomerName).ToList();
            return customerMasters;
        }
        public CustomerViewModel GetSavedCustomers(int pvno)
        {

            CustomerViewModel customerView = new CustomerViewModel();

            customerView.customerMaster = _customerMasterRepository.GetAsQueryable().Where(K => K.CustomerMasterCustomerNo == pvno).SingleOrDefault();
            customerView.customercontacts = _customerContactsRepository.GetAsQueryable().Where(c => c.CustomerContactsCustomerId == pvno && (c.CustomerContactsDelStatus == false || c.CustomerContactsDelStatus == null)).ToList();
            customerView.customerdepartments = _customerDepartmentsRepository.GetAsQueryable().Where(x => x.CustomerDepartmentsCustomerId == pvno && (x.CustomerDepartmentsDelStatus == false || x.CustomerDepartmentsDelStatus == null)).ToList();


            return customerView;

        }
    }
}


