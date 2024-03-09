using Inspire.Erp.Application.Procurement.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Account.Implementations;
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
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Models.Procurement.PurchaseOrderTracking;
using System.Threading.Tasks;
using Inspire.Erp.Infrastructure.Database;

namespace Inspire.Erp.Application.Procurement.Implementation
{
    public class PurchaseRequisitionService : IPurchaseRequisitionService
    {
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PurchaseRequisition> _purchaseRequisitionRepository;
        private IRepository<PurchaseRequisitionDetails> _purchaseRequisitionDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;
        private readonly IRepository<PermissionApproval> _permissionApprovalRepo;
        private readonly IRepository<Approval> _approvalRepo;
        private readonly IRepository<ApprovalDetail> _approvalDetailRepo;
        public readonly InspireErpDBContext dbContext;

        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;

        ////      private IRepository<ReportPurchaseRequisition> _reportPurchaseRequisitionRepository;
        ////public PurchaseRequisitionService(IRepository<ReportPurchaseRequisition> reportPurchaseRequisitionRepository,
        public PurchaseRequisitionService(IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,
            IRepository<PermissionApproval> permissionApprovalRepo, IRepository<Approval> approvalRepo, IRepository<ApprovalDetail> approvalDetailRepo,
            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
             IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,

            IRepository<PurchaseRequisition> purchaseRequisitionRepository, IRepository<PurchaseRequisitionDetails> purchaseRequisitionDetailsRepository)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._purchaseRequisitionRepository = purchaseRequisitionRepository;
            this._purchaseRequisitionDetailsRepository = purchaseRequisitionDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _approvalDetailRepo = approvalDetailRepo;
            _permissionApprovalRepo = permissionApprovalRepo;
            _approvalRepo= approvalRepo;

            _itemMasterRepository = itemMasterRepository;
            _unitMasterRepository = unitMasterRepository;
            _suppliersMasterRepository = suppliersMasterRepository;
            _locationMasterRepository = locationMasterRepository;

            ////_reportPurchaseRequisitionRepository = reportPurchaseRequisitionRepository;
        }

        ////public IEnumerable<ReportPurchaseRequisition> PurchaseRequisition_GetReportPurchaseRequisition()
        ////{
        ////    return _reportPurchaseRequisitionRepository.GetAll();
        ////}


        public PurchaseRequisitionModel UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions, 
            List<PurchaseRequisitionDetails> purchaseRequisitionDetails
            //, List<StockRegister> stockRegister
            )
        {

            try
            {
                _purchaseRequisitionRepository.BeginTransaction();

               this.deleteAccountTractionByVoucherId(purchaseRequisition.PurchaseRequisitionNo, VoucherType.PurchaseRequisition_TYPE);
                purchaseRequisition.PurchaseRequisitionDetails = purchaseRequisitionDetails.Select((k) =>
                {
                    //if (k.PurchaseRequisitionDetailsId == 0)
                    //{
                    k.PurchaseRequisitionId = purchaseRequisition.PurchaseRequisitionId;
                    k.PurchaseRequisitionDetailsNo = purchaseRequisition.PurchaseRequisitionNo;
                    //k.PurchaseRequisitionDetailsId = 0;
                    //}

                    return k;
                }).ToList();

                _purchaseRequisitionRepository.Update(purchaseRequisition);





                //_purchaseRequisitionRepository.Update(purchaseRequisition);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = purchaseRequisition.PurchaseRequisitionDate;
                        k.AccountsTransactionsVoucherNo = purchaseRequisition.PurchaseRequisitionNo;
                        k.AccountsTransactionsVoucherType = VoucherType.PurchaseRequisition_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }

                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);


                //stockRegister = stockRegister.Select((k) =>
                //{
                //    if (k.StockRegisterId == 0)
                //    {
                //        k.StockRegisterVoucherDate = purchaseRequisition.PurchaseRequisitionDate;
                //        k.StockRegisterVoucherNo = purchaseRequisition.PurchaseRequisitionNo;
                //        k.StockRegisterTransType = VoucherType.PurchaseRequisition_TYPE;

                //        k.StockRegisterStatus = AccountStatus.Approved;
                //    }

                //    return k;
                //}).ToList();
                //_stockRegisterRepository.UpdateList(stockRegister);






                _purchaseRequisitionRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _purchaseRequisitionRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedPurchaseRequisitionDetails(purchaseRequisition.PurchaseRequisitionNo);
        }

        public int DeletePurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions,
            List<PurchaseRequisitionDetails> purchaseRequisitionDetails
            //, List<StockRegister> stockRegister
            )
        {
            int i = 0;
            try
            {
                _purchaseRequisitionRepository.BeginTransaction();

                purchaseRequisition.PurchaseRequisitionDelStatus = true;

                purchaseRequisitionDetails = purchaseRequisitionDetails.Select((k) =>
                {
                    k.PurchaseRequisitionDetailsDelStatus = true;
                    return k;
                }).ToList();

                //_purchaseRequisitionDetailsRepository.UpdateList(purchaseRequisitionDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);




                //stockRegister = stockRegister.Select((k) =>
                //{
                //    k.StockRegisterDelStatus = true;
                //    return k;
                //}).ToList();
                //_stockRegisterRepository.UpdateList(stockRegister);




                purchaseRequisition.PurchaseRequisitionDetails = purchaseRequisitionDetails;

                _purchaseRequisitionRepository.Update(purchaseRequisition);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == purchaseRequisition.PurchaseRequisitionNo).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);

                _purchaseRequisitionRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _purchaseRequisitionRepository.TransactionRollback();
                i = 0;
                throw ex;
            }

            return i;

        }


       public void deleteAccountTractionByVoucherId(string AccountsTransactionsVoucherNo,string AccountsTransactionsVoucherType)
        {
            List<AccountsTransactions> accttrans = this._accountTransactionRepository.GetAsQueryable()
                 .Where(k => k.AccountsTransactionsVoucherNo == AccountsTransactionsVoucherNo &&   k.AccountsTransactionsVoucherType == AccountsTransactionsVoucherType && k.AccountstransactionsDelStatus != true).Select((l) => new AccountsTransactions
                 {
                      AccountsTransactionsTransSno = l.AccountsTransactionsTransSno,
                      AccountstransactionsDelStatus = true,

                     AccountsTransactionsAccNo = l.AccountsTransactionsAccNo,
                     AccountsTransactionsTransDate = l.AccountsTransactionsTransDate,
                     AccountsTransactionsParticulars = l.AccountsTransactionsParticulars,

                     AccountsTransactionsDebit = l.AccountsTransactionsDebit,
       

     AccountsTransactionsCredit = l.AccountsTransactionsCredit,
                     AccountsTransactionsFcDebit = l.AccountsTransactionsFcDebit,
                     AccountsTransactionsFcCredit = l.AccountsTransactionsFcCredit,

                     AccountsTransactionsVoucherType = l.AccountsTransactionsVoucherType,




       


    AccountsTransactionsVoucherNo = l.AccountsTransactionsVoucherNo,
                     AccountsTransactionsDescription = l.AccountsTransactionsDescription,
                     AccountsTransactionsUserId = l.AccountsTransactionsUserId,

                     AccountsTransactionsStatus = l.AccountsTransactionsStatus,


       




      AccountsTransactionsTstamp = l.AccountsTransactionsTstamp,
                     RefNo = l.RefNo,
                     AccountsTransactionsFsno = l.AccountsTransactionsFsno,

                     AccountsTransactionsAllocDebit = l.AccountsTransactionsAllocDebit,
      


    AccountsTransactionsAllocCredit = l.AccountsTransactionsAllocCredit,
                     AccountsTransactionsAllocBalance = l.AccountsTransactionsAllocBalance,
                     AccountsTransactionsFcAllocDebit = l.AccountsTransactionsFcAllocDebit,

                     AccountsTransactionsFcAllocCredit = l.AccountsTransactionsFcAllocCredit,

        


    AccountsTransactionsFcAllocBalance = l.AccountsTransactionsFcAllocBalance,
                     AccountsTransactionsLocation = l.AccountsTransactionsLocation,
                     AccountsTransactionsJobNo = l.AccountsTransactionsJobNo,

                     AccountsTransactionsCostCenterId = l.AccountsTransactionsCostCenterId,


        



     AccountsTransactionsApprovalDt = l.AccountsTransactionsApprovalDt,
                     AccountsTransactionsDepartment = l.AccountsTransactionsDepartment,
                     AccountsTransactionsFcRate = l.AccountsTransactionsFcRate,

                     AccountsTransactionsCompanyId = l.AccountsTransactionsCompanyId,
        
    AccountsTransactionsCurrencyId = l.AccountsTransactionsCurrencyId,
                     AccountsTransactionsDrGram = l.AccountsTransactionsDrGram,
                     AccountsTransactionsCrGram = l.AccountsTransactionsCrGram,

                     AccountsTransactionsCheqNo = l.AccountsTransactionsCheqNo,

       
    AccountsTransactionsLpoNo = l.AccountsTransactionsLpoNo,
                     AccountsTransactionsCheqDate = l.AccountsTransactionsCheqDate,
                     AccountsTransactionsOpposEntryDesc = l.AccountsTransactionsOpposEntryDesc,

                     AccountsTransactionsAllocUpdateBal = l.AccountsTransactionsAllocUpdateBal,



         


    AccountsTransactionsDeptId = l.AccountsTransactionsDeptId,
                     AccountsTransactionsVatno = l.AccountsTransactionsVatno,
                     AccountsTransactionsVatableAmount = l.AccountsTransactionsVatableAmount,

                     
         





    }).ToList() ;

            this._accountTransactionRepository.UpdateList(accttrans);
        }
        public PurchaseRequisitionModel InsertPurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions,
            List<PurchaseRequisitionDetails> purchaseRequisitionDetails
            //, List<StockRegister> stockRegister
            )
        {
            try
            {
                _purchaseRequisitionRepository.BeginTransaction();
                string purchaseRequisitionNumber = this.GenerateVoucherNo(purchaseRequisition.PurchaseRequisitionDate.Date).VouchersNumbersVNo;
                purchaseRequisition.PurchaseRequisitionNo = purchaseRequisitionNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _purchaseRequisitionRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseRequisitionId) + 1);

                purchaseRequisition.PurchaseRequisitionId = maxcount;






                purchaseRequisitionDetails = purchaseRequisitionDetails.Select((x) =>
                {
                    x.PurchaseRequisitionId = maxcount;
                    x.PurchaseRequisitionDetailsNo = purchaseRequisitionNumber;
                    return x;
                }).ToList();
                //_purchaseRequisitionDetailsRepository.InsertList(purchaseRequisitionDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    //k.AccountsTransactionsTransDate = purchaseRequisition.PurchaseRequisitionDate;
                    k.AccountsTransactionsVoucherNo = purchaseRequisitionNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseRequisition_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);



                //stockRegister = stockRegister.Select((k) =>
                //{
                //    k.StockRegisterVoucherDate = purchaseRequisition.PurchaseRequisitionDate;
                //    k.StockRegisterVoucherNo = purchaseRequisitionNumber;
                //    k.StockRegisterTransType = VoucherType.PurchaseRequisition_TYPE;
                //    k.StockRegisterStatus = AccountStatus.Approved;
                //    return k;
                //}).ToList();
                //_stockRegisterRepository.InsertList(stockRegister);





                _purchaseRequisitionRepository.Insert(purchaseRequisition);
                var permsionApproval = _permissionApprovalRepo.GetAsQueryable().Where(x => x.VoucherType == VoucherType.PurchaseRequisition_TYPE).OrderBy(x => x.LevelOrder).ToList();
                var approvals = new Approval()
                {
                    CreatedBy = 0,
                    LocationId = null,
                    Status = false,
                    VoucherType = "CustomerQuotation"
                };
                _approvalRepo.Insert(approvals);
                _approvalDetailRepo.InsertList(permsionApproval.Select(x => new ApprovalDetail()
                {
                    ApprovalId = approvals.Id,
                    Status = false,
                    UserId = x.UserId,
                    LevelOrder = x.LevelOrder,
                    ApprovedAt = DateTime.Now,
                }).ToList());

                _purchaseRequisitionRepository.TransactionCommit();
                return this.GetSavedPurchaseRequisitionDetails(purchaseRequisition.PurchaseRequisitionNo);

            }
            catch (Exception ex)
            {
                _purchaseRequisitionRepository.TransactionRollback();
                throw ex;
            }



        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<PurchaseRequisition> GetPurchaseRequisition()
        {
            IEnumerable<PurchaseRequisition> purchaseRequisition_ALL = _purchaseRequisitionRepository.GetAll().Where(k => k.PurchaseRequisitionDelStatus == false || k.PurchaseRequisitionDelStatus == null);
            return purchaseRequisition_ALL;
        }
        public PurchaseRequisitionModel GetSavedPurchaseRequisitionDetails(string pvno)
        {
            PurchaseRequisitionModel purchaseRequisitionModel = new PurchaseRequisitionModel();
            //purchaseRequisitionModel.purchaseRequisition = _purchaseRequisitionRepository.GetAsQueryable().Where(k => k.PurchaseRequisitionNo == pvno && k.PurchaseRequisitionDelStatus == false).SingleOrDefault();

            purchaseRequisitionModel.purchaseRequisition = _purchaseRequisitionRepository.GetAsQueryable().Where(k => k.PurchaseRequisitionNo == pvno).SingleOrDefault();



            purchaseRequisitionModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.PurchaseRequisition_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            purchaseRequisitionModel.purchaseRequisitionDetails = _purchaseRequisitionDetailsRepository.GetAsQueryable().Where(x => x.PurchaseRequisitionDetailsNo == pvno && (x.PurchaseRequisitionDetailsDelStatus == false || x.PurchaseRequisitionDetailsDelStatus == null)).ToList();
            return purchaseRequisitionModel;





        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {






                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PurchaseRequisition_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PurchaseRequisition_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                //var prefix = "CN";
                //int vnoMaxVal = 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PurchaseRequisition_TYPE,
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



        //using Microsoft.Extensions.Logging;
        //private readonly ILogger<PaymentVoucherService> _logger;
        //IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,
        public VouchersNumbers GetVouchersNumbers(string pvno)
        {
            try
            {
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == pvno).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }
        
       public async Task<Response<List<PurChaseRequisitionStatus>>> GetPurchaseRequisitionStatus(PurChaseRequisitionStatusFilterReport model)
        {
            List<PurChaseRequisitionStatus> data = new List<PurChaseRequisitionStatus>();
            try
            {
                //    string filteredValue = string.Empty;
                //    if (model.JobId > 0)
                //    {
                //        filteredValue += $" and pd.PurchaseRequisitionJobId = {model.JobId} ";
                //    }                
                //    if (model.ItemId > 0)
                //    {
                //        filteredValue += $" and pd.PurchaseRequisitionDetailsItemID = {model.ItemId} ";
                //    }
                //    if (!string.IsNullOrEmpty(model.PRNO) && model.PRNO.ToUpper() != "ALL".ToUpper())
                //    {
                //        filteredValue += $" and po.PurchaseRequisitionRefNo = {model.PRNO} ";
                //    }
                //    if (!string.IsNullOrEmpty(model.POStatus))
                //    {
                //        filteredValue += $" and po.PurchaseRequisitionStatus = {model.POStatus} ";
                //    }              
                //    if (!string.IsNullOrEmpty(model.fromDate) && !string.IsNullOrEmpty(model.toDate))
                //    {
                //        filteredValue += $" and (pm.PurchaseRequisitionDate >= {model.fromDate} and  pm.PurchaseRequisitionDate <= {model.toDate} )";
                //    }
                //    var results = (from pd in dbContext.PurchaseRequisitionDetails
                //                   join pm in dbContext.PurchaseRequisition on pd.PurchaseRequisitionId equals pm.PurchaseRequisitionId
                //                     join im in dbContext.ItemMaster on pd.PurchaseRequisitionDetailsItemID equals im.ItemMasterItemId into imJoin
                //                     from im in imJoin.DefaultIfEmpty()
                //                     join um in dbContext.UnitMaster on pd.Purchase_Requisition_Details_Unit_ID equals um.UnitMasterUnitId into umJoin
                //                     from um in umJoin.DefaultIfEmpty()
                //                     join podGroup in (
                //                     from pod in dbContext.PurchaseOrderDetails
                //                     join po in dbContext.GeneralPurchaseOrder on pod.PurchaseOrderDetailsPoId equals po.GeneralPurchaseOrderId
                //                     group new { pod, po } by new { pod.PurchaseOrderDetailsId, pod.PurchaseOrderDetailsPrdId, po.GeneralPurchaseOrderStatus } into grp
                //                     select new
                //                     {
                //                         grp.Key.PurchaseOrderDetailsId,
                //                         grp.Key.PurchaseOrderDetailsPrdId,
                //                         grp.Key.GeneralPurchaseOrderStatus,
                //                         Quantity = grp.Sum(x => x.pod.PurchaseOrderDetailsQuantity)
                //                     }) on new { pd.Purchase_Requisition_Details_Purchase_Reques_Detail_ID, pd.Purchase_Requisition_Details_Item_ID } equals new { podGroup.PurchaseOrderDetailsPrdId } into podJoin
                //                     from pod in podJoin.DefaultIfEmpty()
                //                     join vd in (
                //                     from d in dbContext.AvIssueVoucherDetails
                //                     join i in dbContext.AvIssueVoucher on d.AvIssueVoucherDetailsAvSvNo equals i.AvIssueVoucherAccNo
                //                     group d by d.AvIssueVoucherDetailsMaterialId into grp
                //                     select new
                //                     {
                //                         grp.Key,
                //                         issueQty = grp.Sum(x => x.AvIssueVoucherDetailsSoldQuantity),
                //                         stock = grp.Sum(x => x.AvIssueVoucherDetailsStock),
                //                         BalToIssue = grp.Sum(x => x.AvIssueVoucherDetailsStock) - grp.Sum(x => x.AvIssueVoucherDetailsSoldAmount)
                //                     }) on im.Item_ID equals vd.Key into vdJoin
                //                     from vd in vdJoin.DefaultIfEmpty()
                //                     join vda in (
                //                     from d in dbContext.AvIssueVoucherDetails
                //                     join i in dbContext.AvIssueVoucher on d.AvIssueVoucherDetailsAvSvNo equals i.AvIssueVoucherAccNo
                //                     join s in dbContext.StockRegister on d.AvIssueVoucherDetailsMaterialId equals s.StockRegisterMaterialID
                //                     group new { d, s } by d.AvIssueVoucherDetailsMaterialId into grp
                //                     select new
                //                     {
                //                         grp.Key,
                //                         issueQty = grp.Sum(x => x.d.AvIssueVoucherDetailsSoldQuantity),
                //                         stock = grp.Sum(x => x.s.StockRegisterSIN - x.s.StockRegisterSout),
                //                         BalToIssue = grp.Sum(x => (x.s.StockRegisterSIN - x.s.StockRegisterSout) - Convert.ToDecimal(x.d.AvIssueVoucherDetailsSoldQuantity))
                //                     }) on im.Item_ID equals vda.Key into vdaJoin
                //                     from vda in vdaJoin.DefaultIfEmpty()
                //                     where pd.Purchase_Requisition_Details_Required_Status == "OPEN" && $"1 == 1  {filteredValue}"
                //                     select new PurChaseRequisitionStatus
                //                     {
                //                         issueQty = vd.issueQty ?? 0,
                //                         BalToIssue = vd.BalToIssue ?? 0,
                //                         stock = vd.stock ?? 0,
                //                         Unit = um.Unit_Master_Unit_Short_Name + " :: " + um.Unit_Master_Unit_ID.ToString(),
                //                         PartNo = im.Part_No ?? "",
                //                         DelQty = pod.Quantity ?? 0,
                //                         PoStatus = pod.PoStatus ?? "OPEN",
                //                         RequisitionDate = pm.Purchase_Requisition_Requisition_Date

                //                     }).ToListAsync();
                return new Response<List<PurChaseRequisitionStatus>>()
                {
                    Valid = true,
                    Message = "Data Found",
                    Result = data
                };
            }
            catch (Exception ex)
            {
                return Response<List<PurChaseRequisitionStatus>>.Fail(new List<PurChaseRequisitionStatus>(), ex.Message);
            }
        }
    }
}
