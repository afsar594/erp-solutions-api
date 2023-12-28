using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Logging;
using Inspire.Erp.Application.MODULE;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Models.Sales.DeliveryNoteReport;
using Inspire.Erp.Domain.Modals.Common;
using SendGrid.Helpers.Mail;
using Inspire.Erp.Domain.Models.Procurement.PurchaseOrderTracking;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Inspire.Erp.Application.Sales.Implementations
{
    public class DeliveryNoteServices : IDeliveryNoteServices
    {
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<CustomerDeliveryNote> _deliveryNoteRepository;
        private IRepository<CustomerDeliveryNoteDetails> _deliveryNoteDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;

        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;

        private IRepository<CustomerMaster> _customerMasterRepository;
        private IRepository<CustomerMaster> _salesManMasterRepository;
        private IRepository<DepartmentMaster> _departmentMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepo;



        //private IRepository<ReportSalesVoucher> _reportSalesVoucherRepository;
        public DeliveryNoteServices(
            //IRepository<ReportSalesVoucher> reportSalesVoucherRepository,
            IRepository<CustomerMaster> customerMasterRepository,
            IRepository<CustomerMaster> salesManMasterRepository,
            IRepository<DepartmentMaster> departmentMasterRepository,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,

            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,

            IRepository<CustomerDeliveryNote> deliveryNoteRepository, IRepository<CustomerDeliveryNoteDetails> deliveryNoteDetailsRepository
            ,IRepository<LocationMaster> locationMasterRepo)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._deliveryNoteRepository = deliveryNoteRepository;
            this._deliveryNoteDetailsRepository = deliveryNoteDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;


            _itemMasterRepository = itemMasterRepository;
            _unitMasterRepository = unitMasterRepository;
            _suppliersMasterRepository = suppliersMasterRepository;
            _locationMasterRepository = locationMasterRepository;

            _customerMasterRepository = customerMasterRepository;

            _salesManMasterRepository = salesManMasterRepository;

            _departmentMasterRepository = departmentMasterRepository;
            _locationMasterRepo = locationMasterRepo;

            //_reportSalesVoucherRepository = reportSalesVoucherRepository;
        }

        //public IEnumerable<ReportSalesVoucher> SalesVoucher_GetReportSalesVoucher()
        //{
        //    return _reportSalesVoucherRepository.GetAll();
        //}



        ////////public IEnumerable<LocationMaster> SalesVoucher_GetAllLocationMaster()
        ////////{
        ////////    return _locationMasterRepository.GetAll();
        ////////}



        ////////public IEnumerable<SuppliersMaster> SalesVoucher_GetAllSuppliersMaster()
        ////////{
        ////////    return _suppliersMasterRepository.GetAll();
        ////////}


        ////////public IEnumerable<DepartmentMaster> SalesVoucher_GetAllDepartmentMaster()
        ////////{
        ////////    return _departmentMasterRepository.GetAll();
        ////////}




        ////////public IEnumerable<CustomerMaster> SalesVoucher_GetAllCustomerMaster()
        ////////{
        ////////    return _customerMasterRepository.GetAll();
        ////////}



        ////////public IEnumerable<CustomerMaster> SalesVoucher_GetAllSalesManMaster()
        ////////{
        ////////    return _salesManMasterRepository.GetAll();
        ////////}


        ////////public IEnumerable<UnitMaster> SalesVoucher_GetAllUnitMaster()
        ////////{
        ////////    return _unitMasterRepository.GetAll();
        ////////}
        ////////public IEnumerable<ItemMaster> SalesVoucher_GetAllItemMaster()
        ////////{
        ////////    return _itemMasterRepository.GetAll();
        ////////}

        public DeliveryNoteModel UpdateDeliveryNote(CustomerDeliveryNote deliveryNote,List<CustomerDeliveryNoteDetails> deliveryNoteDetails
            )
        {
            try
            {
                _deliveryNoteRepository.BeginTransaction();

                //////=================================
                ////clsCommonFunctions.Delete_OldEntry_StockRegister(deliveryNote.CustomerDeliveryNoteDeliveryId, VoucherType.SalesVoucher_TYPE, _stockRegisterRepository);
                ////clsCommonFunctions.Delete_OldEntry_AccountsTransactions(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _accountTransactionRepository);
                //////=================================
                deliveryNote.CustomerDeliveryNoteDetails = deliveryNoteDetails.Select((k) =>
                {
                    //if (k.SalesVoucherDetailsId == 0)
                    //{
                    k.CustomerDeliveryNoteDetailsDetId = deliveryNote.CustomerDeliveryNoteDeliveryId;
                  //  k.SalesVoucherDetailsNo = salesVoucher.SalesVoucherNo;
                    //k.SalesVoucherDetailsId = 0;
                    //}

                    return k;
                }).ToList();

                _deliveryNoteRepository.Update(deliveryNote);


               //// //_salesVoucherRepository.Update(salesVoucher);
               ////// accountsTransactions = accountsTransactions.Select((k) =>
               //// {
               ////     if (k.AccountsTransactionsTransSno == 0)
               ////     {
               ////         k.AccountsTransactionsTransDate = salesVoucher.SalesVoucherDate;
               ////         k.AccountsTransactionsVoucherNo = salesVoucher.SalesVoucherNo;
               ////         k.AccountsTransactionsVoucherType = VoucherType.SalesVoucher_TYPE;
               ////         k.AccountsTransactionsStatus = AccountStatus.Approved;
               ////     }

               ////     return k;
               //// }).ToList();
               //// _accountTransactionRepository.UpdateList(accountsTransactions);


               //// stockRegister = stockRegister.Select((k) =>
               //// {
               ////     if (k.StockRegisterStoreID == 0)
               ////     {
               ////         k.StockRegisterVoucherDate = salesVoucher.SalesVoucherDate;
               ////         k.StockRegisterRefVoucherNo = salesVoucher.SalesVoucherNo;
               ////         k.StockRegisterTransType = VoucherType.SalesVoucher_TYPE;

               ////         k.StockRegisterStatus = AccountStatus.Approved;
               ////     }

               ////     return k;
               //// }).ToList();
               //// _stockRegisterRepository.UpdateList(stockRegister);


                _deliveryNoteRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _deliveryNoteRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedDeliveryNoteDetails(deliveryNote.CustomerDeliveryNoteDeliveryId);
        }

        public int DeleteDeliveryNote(CustomerDeliveryNote deliveryNote,List<CustomerDeliveryNoteDetails> deliveryNoteDetails
            )
        {
            int i = 0;
            try
            {
                _deliveryNoteRepository.BeginTransaction();

                ////=================================
                //clsCommonFunctions.Delete_OldEntry_StockRegister(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _stockRegisterRepository);
                //clsCommonFunctions.Delete_OldEntry_AccountsTransactions(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _accountTransactionRepository);
                ////=================================


                deliveryNote.CustomerDeliveryNoteDelStatus = true;

                deliveryNoteDetails = deliveryNoteDetails.Select((k) =>
                {
                    k.CustomerDeliveryNoteDetailsDelStatus = true;
                    return k;
                }).ToList();

                //_salesVoucherDetailsRepository.UpdateList(salesVoucherDetails);

                ////accountsTransactions = accountsTransactions.Select((k) =>
                ////{
                ////    k.AccountstransactionsDelStatus = true;
                ////    return k;
                ////}).ToList();
                ////_accountTransactionRepository.UpdateList(accountsTransactions);

                ////stockRegister = stockRegister.Select((k) =>
                ////{
                ////    k.StockRegisterDelStatus = true;
                ////    return k;
                ////}).ToList();
                ////_stockRegisterRepository.UpdateList(stockRegister);

                deliveryNote.CustomerDeliveryNoteDetails = deliveryNoteDetails;

                _deliveryNoteRepository.Update(deliveryNote);

                ////var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == deliveryNote.CustomerDeliveryNoteDeliveryId).FirstOrDefault();

                ////_voucherNumbersRepository.Update(vchrnumer);

                _deliveryNoteRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _deliveryNoteRepository.TransactionRollback();
                i = 0;
                throw ex;
            }

            return i;
        }
        public DeliveryNoteModel InsertDeliveryNote(CustomerDeliveryNote deliveryNote, List<CustomerDeliveryNoteDetails> deliveryNoteDetails)
        {
            try
            {
                _deliveryNoteRepository.BeginTransaction();
                ////string salesVoucherNumber = this.GenerateVoucherNo(deliveryNote.CustomerDeliveryNoteDeliveryDate).VouchersNumbersVNo;
                // deliveryNote.CustomerDeliveryNoteDeliveryId = salesVoucherNumber;
    
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _deliveryNoteRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.CustomerDeliveryNoteDeliveryId) + 1);

                deliveryNote.CustomerDeliveryNoteDeliveryId = maxcount;

                deliveryNoteDetails = deliveryNoteDetails.Select((x) =>
                {
                    x.CustomerDeliveryNoteDetailsDeliveryDetailId = maxcount;
                    //x.SalesVoucherDetailsNo = salesVoucherNumber;
                    return x;
                }).ToList();

                //_salesVoucherDetailsRepository.InsertList(salesVoucherDetails);

                ////accountsTransactions = accountsTransactions.Select((k) =>
                ////{
                ////    //k.AccountsTransactionsTransDate = salesVoucher.SalesVoucherDate;
                ////    k.AccountsTransactionsVoucherNo = salesVoucherNumber;
                ////    k.AccountsTransactionsVoucherType = VoucherType.SalesVoucher_TYPE;
                ////    k.AccountsTransactionsStatus = AccountStatus.Approved;
                ////    return k;
                ////}).ToList();
                ////_accountTransactionRepository.InsertList(accountsTransactions);



                ////stockRegister = stockRegister.Select((k) =>
                ////{
                ////    k.StockRegisterVoucherDate = salesVoucher.SalesVoucherDate;
                ////    k.StockRegisterRefVoucherNo = salesVoucherNumber;
                ////    k.StockRegisterTransType = VoucherType.SalesVoucher_TYPE;
                ////    k.StockRegisterStatus = AccountStatus.Approved;
                ////    return k;
                ////}).ToList();
                ////_stockRegisterRepository.InsertList(stockRegister);


                _deliveryNoteRepository.Insert(deliveryNote);
                _deliveryNoteRepository.TransactionCommit();

                return this.GetSavedDeliveryNoteDetails(deliveryNote.CustomerDeliveryNoteDeliveryId);

                // return a;
            }
            catch (Exception ex)
            {
                _deliveryNoteRepository.TransactionRollback();
                throw ex;
            }

        }
        ////public IEnumerable<AccountsTransactions> GetAllTransaction()
        ////{
        ////    return _accountTransactionRepository.GetAll();
        ////}
        ////public IEnumerable<SalesVoucher> GetSalesVoucher()
        ////{
        ////    IEnumerable<SalesVoucher> salesVoucher_ALL = _salesVoucherRepository.GetAll().Where(k => k.SalesVoucherDelStatus == false || k.SalesVoucherDelStatus == null);
        ////    return salesVoucher_ALL;
        ////}
        public DeliveryNoteModel GetSavedDeliveryNoteDetails(long pvno)
        {
            DeliveryNoteModel deliveryNoteModel = new DeliveryNoteModel();
            //salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == pvno && k.SalesVoucherDelStatus == false).SingleOrDefault();

            deliveryNoteModel.deliveryNote = _deliveryNoteRepository.GetAsQueryable().Where(k => k.CustomerDeliveryNoteDeliveryId == pvno).SingleOrDefault();


          //  salesVoucherModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.SalesVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            deliveryNoteModel.deliveryNoteDetails = _deliveryNoteDetailsRepository.GetAsQueryable().Where(x => x.CustomerDeliveryNoteDetailsDetId == pvno && (x.CustomerDeliveryNoteDetailsDelStatus == false || x.CustomerDeliveryNoteDetailsDelStatus == null)).ToList();
            return deliveryNoteModel;
        }

        public IEnumerable<CustomerDeliveryNote> GetDeliveryNote()
        {
            var allCustomerDeliveryNotes = _deliveryNoteRepository.GetAsQueryable();
            return allCustomerDeliveryNotes;
          //  throw new NotImplementedException();
        }

        public async  Task<Response<List<DNReportDetails>>> DeliveryNoteReportDetails(DeliveryNoteReportFilter filter)
        {
            List<DNReportDetails> reportDetails = new List<DNReportDetails>();
            string filteredValue = " && 1 == 1";

            try
            {

                if (filter.LocationId > 0)
                {
                    filteredValue += $" && CustomerDeliveryNoteLocationID == {filter.LocationId} ";
                }
                if(filter.CustomerId > 0)
                {
                    filteredValue += $" && CustomerDeliveryNoteCustomerCode == {filter.CustomerId} "; 
                }
                ////if (!string.IsNullOrEmpty(filter.fromDate) && !string.IsNullOrEmpty(filter.toDate))
                ////{
                ////    filteredValue += $" && CustomerDeliveryNoteDeliveryDate >= {Convert.ToDateTime(filter.fromDate)} && CustomerDeliveryNoteDeliveryDate <= {Convert.ToDateTime(filter.toDate)} ";
                ////}
                //if(!string.IsNullOrEmpty(filter.InvioceStatus))
                //{
                //    filteredValue += $" && CustomerDeliveryNoteDeliveryStatus == {filter.InvioceStatus} ";
                //}
                reportDetails = await (from dn in _deliveryNoteRepository.GetAsQueryable().Where($"1 == 1  { filteredValue }")
                                    join lm in _locationMasterRepo.GetAsQueryable() on dn.CustomerDeliveryNoteLocationId equals (long)lm.LocationMasterLocationId into lmGroup
                                    from lm in lmGroup.DefaultIfEmpty()
                                    join dnd in _deliveryNoteDetailsRepository.GetAsQueryable() on dn.CustomerDeliveryNoteDeliveryId equals (long)dnd.CustomerDeliveryNoteDetailsDeliveryDetailId into dndGroup
                                    from dnd in dndGroup.DefaultIfEmpty()
                                    select new DNReportDetails()
                                    {
                                        DaliveryDate = dn.CustomerDeliveryNoteCpoDate,
                                        CustomerName = dn.CustomerDeliveryNoteCustomerName,
                                        DeliverySatus = dn.CustomerDeliveryNoteDeliveryStatus,
                                        DelId = dn.CustomerDeliveryNoteDeliveryId,
                                        Location = lm.LocationMasterLocationName,
                                        ItemList = new List<DNItemList>
                                        {
                                            new DNItemList
                                            {
                                              Description   = dnd.CustomerDeliveryNoteDetailsDescription,
                                              Qty  = dnd.CustomerDeliveryNoteDetailsQty,
                                              MaterialId = dnd.CustomerDeliveryNoteDetailsItemId

                                            }
                                        },


                                    }).OrderBy(x => x.DaliveryDate).ToListAsync();

        
                return new Response<List<DNReportDetails>>
                {
                    Valid = true,
                    Result = reportDetails,
                    Message = "Delivery Note Data Fonud"
                };
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        ////public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        ////{
        ////    try
        ////    {

        ////        var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.SalesVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
        ////        int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
        ////                                                .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.SalesVoucher_TYPE)
        ////                                                .DefaultIfEmpty()
        ////                                                .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


        ////        //var prefix = "CN";
        ////        //int vnoMaxVal = 1;

        ////        VouchersNumbers vouchersNumbers = new VouchersNumbers
        ////        {
        ////            VouchersNumbersVNo = prefix + vnoMaxVal,
        ////            VouchersNumbersVNoNu = vnoMaxVal,
        ////            VouchersNumbersVType = VoucherType.SalesVoucher_TYPE,
        ////            VouchersNumbersFsno = 1,
        ////            VouchersNumbersStatus = AccountStatus.Pending,
        ////            VouchersNumbersVoucherDate = VoucherGenDate

        ////        };
        ////        _voucherNumbersRepository.Insert(vouchersNumbers);
        ////        return vouchersNumbers;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }

        ////}

        //using Microsoft.Extensions.Logging;
        //private readonly ILogger<PaymentVoucherService> _logger;
        //IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,
        ////public VouchersNumbers GetVouchersNumbers(string pvno)
        ////{
        ////    try
        ////    {
        ////        VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == pvno).SingleOrDefault();
        ////        return vouchersNumbers;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        _logger.LogError(ex.Message);
        ////        throw ex;
        ////    }

        ////}

    }
}
