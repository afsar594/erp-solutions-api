using Inspire.Erp.Application.Procurement.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models.Procurement;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Inspire.Erp.Application.Procurement.Implimentation
{
    public class PurchaseRequisitionStatusReport : IPurchaseRequisitionStatusReport
    {


        private IRepository<PurchaseOrder> _pOrder;
        private IRepository<PurchaseOrderDetails> _poDetails;
        private IRepository<PurchaseRequisition> _pRequisition;
        private IRepository<PurchaseRequisitionDetails> _pRDetails;
        private IRepository<AvIssueVoucher> _avIssueVoucher;
        private IRepository<AvIssueVoucherDetails> _avIssueDetails;
        private IRepository<ItemMaster> _itemMaster;
        private IRepository<UnitMaster> _unitMaster;
        private IRepository<StockRegister> _stockRegister;

        public PurchaseRequisitionStatusReport(IRepository<PurchaseOrderDetails> poDetails, IRepository<PurchaseRequisition> pRequisition, IRepository<ItemMaster> itemMaster, IRepository<UnitMaster> unitMaster, IRepository<PurchaseOrder> pOrder, IRepository<AvIssueVoucherDetails> avIssueDetails, IRepository<AvIssueVoucher> avIssueVoucher, IRepository<StockRegister> stockRegister, IRepository<PurchaseRequisitionDetails> pRDetails)
        {
            _poDetails = poDetails;
            _pRequisition = pRequisition;
            _itemMaster = itemMaster;
            _unitMaster = unitMaster;
            _pOrder = pOrder;
            _avIssueDetails = avIssueDetails;
            _avIssueVoucher = avIssueVoucher;
            _stockRegister = stockRegister;
            _pRDetails = pRDetails;
        }

        public async Task<Response<List<PurchaseRequisitionStatus>>> GetPurchaseRequisitionStatusReport(ReportFilter model)
        {
            try
            {
                


            }
            catch (Exception ex)
            {
                return Response<List<PurchaseRequisitionStatus>>.Fail(new List<PurchaseRequisitionStatus>(), ex.Message);
            }
        }
    }
}
