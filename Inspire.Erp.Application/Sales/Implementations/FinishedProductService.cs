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
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models.Sales.DeliveryNoteReport;
using System.Security.Cryptography;
using Inspire.Erp.Domain.Modals.Stock;
using Microsoft.AspNetCore.Mvc;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Entities.POS;

namespace Inspire.Erp.Application.Sales.Implementations
{
    public class FinishedProductService : IFinishedProduct
    {
        private IRepository<FinishedProductMaster> _FinishedProductMasterRepository;
        private IRepository<FinishedProductDetails> FinishedProductDetailsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<Domain.Entities.ProgramSettings> _programsettingsRepository;
        private IRepository<UserTracking> _userTrackingRepository;
        private readonly ILogger<CustomerQuotationService> _logger;
        public readonly InspireErpDBContext _Context;
        private readonly IRepository<PermissionApproval> _permissionApprovalRepo;
        private readonly IRepository<Approval> _approvalRepo;
        private readonly IRepository<ApprovalDetail> _approvalDetailRepo;
        private IRepository<ItemMaster> _itemMasterRepository;

        public FinishedProductService(ILogger<CustomerQuotationService> logger, IRepository<PermissionApproval> permissionApprovalRepo,
            IRepository<Approval> approvalRepo, IRepository<ApprovalDetail> approvalDetailRepo,
            IRepository<FinishedProductMaster> FinishedProductMasterRepository, IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<Domain.Entities.ProgramSettings> programsettingsRepository,
            IRepository<FinishedProductDetails> FinishedProductDetailsRepository,
            IRepository<UserTracking> userTrackingRepository,
            InspireErpDBContext Context)
        {

            _FinishedProductMasterRepository = FinishedProductMasterRepository;
            FinishedProductDetailsRepository = FinishedProductDetailsRepository;
            this._voucherNumbersRepository = voucherNumbersRepository;
            this._programsettingsRepository = programsettingsRepository;
            _logger = logger;
            this._userTrackingRepository = userTrackingRepository;
            _approvalDetailRepo = approvalDetailRepo;
            _approvalRepo = approvalRepo;
            _permissionApprovalRepo = permissionApprovalRepo;


            _Context = Context;
        }

        //public BillofQtyMaster UpdateBillofQty(BillofQtyMaster BillofQty, List<BillOfQTyDetails> BillOfQTyDetails)
        //{

        //    try
        //    {
        //        _BillofQtyMasterRepository.BeginTransaction();
        //        BillofQty.BillOfQTyDetails.Clear();

        //        _BillofQtyMasterRepository.Update(BillofQty);

        //        decimal maxcount1 = 0;
        //        maxcount1 = _BillOfQTyDetailsRepository.GetAsQueryable()
        //            .DefaultIfEmpty()
        //            .Max(o => o == null ? 0 : o.Id) + 1;


        //        foreach (var x in BillOfQTyDetails)
        //        {
        //            x.BillQtyId = BillofQty.Id;

        //            if (x.Id == 0)
        //            {
        //                x.Id = maxcount1;
        //                x.BillQtyId = BillofQty.Id;
        //                _BillOfQTyDetailsRepository.Insert(x);
        //                maxcount1++;
        //            }
        //            else
        //            {
        //                _BillOfQTyDetailsRepository.Update(x);
        //            }
        //        }
        //        _BillofQtyMasterRepository.TransactionCommit();
        //    }
        //    catch (Exception ex)
        //    {
        //        _BillofQtyMasterRepository.TransactionRollback();
        //        throw ex;
        //    }

        //    return this.GetSavedBillofQty(BillofQty.Id);
        //}
        //public int DeleteBillofQty(decimal id)
        //{
        //    int i = 0;
        //    try
        //    {
        //        _BillofQtyMasterRepository.BeginTransaction();

        //        var BillofQty = _BillofQtyMasterRepository.GetAsQueryable().Where(x => x.Id == id).FirstOrDefault();                

        //        var BillofQtydt = _BillOfQTyDetailsRepository.GetAsQueryable().Where(x => x.BillQtyId == id).ToList();
        //        _BillOfQTyDetailsRepository.DeleteList(BillofQtydt);

        //        _BillofQtyMasterRepository.Delete(BillofQty);
        //        _BillofQtyMasterRepository.TransactionCommit();
        //        i = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        _BillofQtyMasterRepository.TransactionRollback();
        //        i = 0;
        //        throw ex;
        //    }


        //    return i;

        //}
        public FinishedProductMaster InsertFinishProduct(FinishedProductMaster FinishedProductMaster, List<FinishedProductDetails> FinishedProductDetails)
        {
            try
            {
                _FinishedProductMasterRepository.BeginTransaction();

                // Generate unique voucher number
                string customerQuotationNumber = clsCommonFunctions.GenrateVoucherNo(
                    VoucherType.BOQ, Prefix.BOQ_Prefix,
                    _programsettingsRepository, _voucherNumbersRepository);

                FinishedProductMaster.Finish_Product_MasterVoucherNo = customerQuotationNumber;

                // Ensure FinishedProductDetails is initialized
                if (FinishedProductMaster.FinishedProductDetails == null)
                {
                    FinishedProductMaster.FinishedProductDetails = new List<FinishedProductDetails>();
                }

                // Get next ID for FinishedProductMaster
                decimal nextMasterId = _FinishedProductMasterRepository
                    .GetAsQueryable()
                    .DefaultIfEmpty()
                    .Max(o => o == null ? 0 : o.Finish_Product_MasterId) + 1;

                FinishedProductMaster.Finish_Product_MasterId = nextMasterId;

                // Assign IDs to details
                decimal nextDetailId = 1; // Start detail ID from 1
                foreach (var detail in FinishedProductDetails)
                {
                    detail.Finish_Product_MasterId = nextMasterId;
                    detail.Finish_Product_DetailId = nextDetailId;
                    FinishedProductMaster.FinishedProductDetails.Add(detail);
                    nextDetailId++;
                }

                // Insert into database
                _FinishedProductMasterRepository.Insert(FinishedProductMaster);
                _FinishedProductMasterRepository.TransactionCommit();

                return GetSavedBillofQty(FinishedProductMaster.Finish_Product_MasterId);
            }
            catch (Exception ex)
            {
                _FinishedProductMasterRepository.TransactionRollback();
                throw; // Preserves the stack trace
            }
        }

        // Retrieve all finished products
        public IEnumerable<FinishedProductMaster> GetIFinishedProduct()
        {
            return _FinishedProductMasterRepository.GetAll() ?? new List<FinishedProductMaster>();
        }

        // Retrieve a saved finished product by ID
        public FinishedProductMaster GetSavedBillofQty(decimal id)
        {
            var voucherData = _FinishedProductMasterRepository
                .GetAsQueryable()
                .FirstOrDefault(k => k.Finish_Product_MasterId == id);

            if (voucherData != null)
            {
                voucherData.FinishedProductDetails = FinishedProductDetailsRepository
                    .GetAsQueryable()
                    .Where(qd => qd.Finish_Product_MasterId == id)
                    .ToList();
            }

            return voucherData; // Return null if not found
        }

        //public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        //{
        //    try
        //    {
        //        var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.BOQ_Prefix).FirstOrDefault().ProgramSettingsTextValue;
        //        int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
        //                                                .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.BOQ)
        //                                                .DefaultIfEmpty()
        //                                                .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
        //        VouchersNumbers vouchersNumbers = new VouchersNumbers

        //        {
        //            VouchersNumbersVNo = prefix + vnoMaxVal,
        //            VouchersNumbersVNoNu = vnoMaxVal,
        //            VouchersNumbersVType = VoucherType.BOQ,
        //            VouchersNumbersFsno = 1,
        //            VouchersNumbersStatus = AccountStatus.Pending,
        //            VouchersNumbersVoucherDate = VoucherGenDate

        //        };
        //        _voucherNumbersRepository.Insert(vouchersNumbers);
        //        return vouchersNumbers;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
        //public VouchersNumbers GetVouchersNumbers(string Qtno)
        //{
        //    try
        //    {
        //        VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == Qtno && k.VouchersNumbersVType == VoucherType.BOQ).SingleOrDefault();
        //        return vouchersNumbers;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        throw ex;
        //    }

        //}
    }
}


