using Inspire.Erp.Application.Store.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Store.Implementation
{
    public class StockTransferImp : IStockTransfer
    {
        private readonly IRepository<StockTransfer> _StockTransferRepo;
        private readonly IRepository<VouchersNumbers> _VoucherNumberRepo;
        private readonly IRepository<StockItemResponse> _StockItemResponse;
        private readonly IRepository<StockTransferResponseModel> _StockTransferResponse;
        private readonly IRepository<StockTransferDetails> _StockTransferDetailRepo;
        private readonly IRepository<StockRegister> _StockRegisterRepo;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private static string prefix;
        public StockTransferImp(IRepository<StockTransfer> StockTransferRepo,
            IRepository<StockTransferDetails> StockTransferDetailRepo,
            IRepository<StockItemResponse> stockItemResponse,
            IRepository<StockTransferResponseModel> StockTransferResponse,
            IRepository<VouchersNumbers> VoucherNumberRepo,
            IRepository<StockRegister> StockRegisterRepo,
            IRepository<ProgramSettings> programsettingsRepository)
        {
            _StockRegisterRepo = StockRegisterRepo;
            _VoucherNumberRepo = VoucherNumberRepo;
            _StockItemResponse = stockItemResponse;
            _StockTransferResponse = StockTransferResponse;
            _StockTransferDetailRepo = StockTransferDetailRepo;
            _StockTransferRepo = StockTransferRepo;
            _programsettingsRepository = programsettingsRepository;
            prefix = _programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == "HIDE_STOCKTRANSFER").FirstOrDefault().ProgramSettingsTextValue;
        }
        ////////   New Function   ///////////
        public IEnumerable<StockItemResponse> getStockItemResponse()
        {
            try
            {
                var x = _StockItemResponse.GetBySP($"EXEC getStockItemsList");
                return x;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> submitTransferReport(List<StockTransferRequestModel> obj)
        {
            try
            {
                List<StockTransferDetails> std = new List<StockTransferDetails>();
                List<StockTransfer> st = new List<StockTransfer>();
                List<VouchersNumbers> vn = new List<VouchersNumbers>();
                List<StockRegister> sr = new List<StockRegister>();
                for (int i = 0; i < obj.Count; i++)
                {
                    st[i].StockTransferStvno = prefix + obj[i].voucherNo;
                    st[i].StockTransferStDate = DateTime.Parse(obj[i].voucherDate);
                    st[i].StockTransferLocationIdFrom = int.Parse(obj[i].locationFrom);
                    st[i].StockTransferLocationIdTo = int.Parse(obj[i].locationTo);
                    st[i].StockTransferNarration = obj[i].narration;
                    st[i].StockTransferJobId = int.Parse(obj[i].selectedJob);
                    st[i].StockTransferDelStatus = false;
                    st[i].StockTransferApproved = false;

                    vn[i].VouchersNumbersVNo = prefix + obj[i].voucherNo;
                    vn[i].VouchersNumbersVNoNu = decimal.Parse(obj[i].voucherNo);
                    vn[i].VouchersNumbersVoucherDate = DateTime.Parse(obj[i].voucherDate);
                    vn[i].VouhersNumbersDelStatus = false;
                    vn[i].VouchersNumbersStatus = "P";
                    vn[i].VouchersNumbersLocationId = decimal.Parse(obj[i].locationTo);
                    _VoucherNumberRepo.InsertList(vn);

                    sr[i].StockRegisterRefVoucherNo = prefix + obj[i].voucherNo;
                    sr[i].StockRegisterAssignedDate = DateTime.Parse(obj[i].voucherDate);
                    sr[i].StockRegisterMaterialID = int.Parse(obj[i].itemId);
                    sr[i].StockRegisterQuantity = int.Parse(obj[i].quantity);
                    sr[i].StockRegisterAmount = int.Parse(obj[i].amount);
                    sr[i].StockRegisterFCAmount = int.Parse(obj[i].amount);
                    sr[i].StockRegisterStatus = "S";
                    sr[i].StockRegisterTransType = "Stock Transfer";
                    sr[i].StockRegisterRemarks = obj[i].narration;
                    sr[i].StockRegisterLocationID = int.Parse(obj[i].locationTo);
                    sr[i].StockRegisterUnitID = int.Parse(obj[i].unit);
                    sr[i].StockRegisterJobID = int.Parse(obj[i].selectedJob);
                    sr[i].StockRegisterDepID = int.Parse(obj[i].department);
                    sr[i].StockRegisterDelStatus = false;

                   
                    //std[i].VoucherNumber = int.Parse(obj[i].voucherNo);
                    std[i].StockTransferDetailsUnitId = int.Parse(obj[i].unit);
                    std[i].StockTransferDetailsMaterialId = int.Parse(obj[i].itemId);
                    std[i].StockTransferDetailsQty = double.Parse(obj[i].quantity);
                    std[i].StockTransferDetailsRate = double.Parse(obj[i].amount);
                    std[i].StockTransferDetailsRemarks = obj[i].narration;
                    std[i].StockTransferDetailsDelStatus = false;
                    
                }
                _StockRegisterRepo.InsertList(sr);
                _StockTransferDetailRepo.InsertList(std);
                _StockTransferRepo.InsertList(st);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }
        public IEnumerable<StockTransferResponseModel> getStockTransferReport()
        {
            try
            {
                return _StockTransferResponse.GetBySP($"EXEC getStockTransferReport");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> deleteStockTransfer(StockTransferRequestModel obj)
        {
            try
            {
                StockTransfer st = _StockTransferRepo.GetAsQueryable().Where(k => k.StockTransferStvno == obj.voucherNo).SingleOrDefault();
                if (st != null)
                {
                    st.StockTransferDelStatus = true;
                    _StockTransferRepo.Update(st);
                }
                else
                {
                    return false;
                }
                VouchersNumbers vn = _VoucherNumberRepo.GetAsQueryable().Where(k => k.VouchersNumbersVNoNu == int.Parse(obj.voucherNo)).SingleOrDefault();
                if (vn != null)
                {
                    vn.VouhersNumbersDelStatus = true;
                    _VoucherNumberRepo.Update(vn);
                }
                else
                {
                    return false;
                }
                StockTransferDetails std = _StockTransferDetailRepo.GetAsQueryable().Where(k => k.VoucherNumber == int.Parse(obj.voucherNo)).SingleOrDefault();
                if (std != null)
                {
                    std.StockTransferDetailsDelStatus = true;
                    _StockTransferDetailRepo.Update(std);
                }
                else
                {
                    return false;
                }
                StockRegister sr = _StockRegisterRepo.GetAsQueryable().Where(k => k.StockRegisterRefVoucherNo == obj.voucherNo).SingleOrDefault();
                if (sr != null)
                {
                    sr.StockRegisterDelStatus = true;
                    _StockRegisterRepo.Update(sr);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        public async Task<bool> UpdateStockTransfer(StockTransferRequestModel obj)
        {
            StockTransfer st = _StockTransferRepo.GetAsQueryable().Where(k => k.StockTransferStvno == obj.voucherNo).SingleOrDefault();
            if (st != null)
            {
                st.StockTransferLocationIdFrom = int.Parse(obj.locationFrom);
                st.StockTransferLocationIdTo = int.Parse(obj.locationTo);
                st.StockTransferNarration = obj.narration;
                st.StockTransferJobId = int.Parse(obj.selectedJob);
                st.StockTransferDelStatus = false;
                st.StockTransferApproved = false;
                _StockTransferRepo.Update(st);
            }
            else
            {
                return false;
            }
            VouchersNumbers vn = _VoucherNumberRepo.GetAsQueryable().Where(k => k.VouchersNumbersVNoNu == int.Parse(obj.voucherNo)).SingleOrDefault();
            if (vn != null)
            {
                vn.VouchersNumbersVNoNu = decimal.Parse(obj.voucherNo);
                vn.VouhersNumbersDelStatus = false;
                vn.VouchersNumbersStatus = "P";
                vn.VouchersNumbersLocationId = decimal.Parse(obj.locationTo);
                _VoucherNumberRepo.Update(vn);
            }
            else
            {
                return false;
            }
            StockTransferDetails std = _StockTransferDetailRepo.GetAsQueryable().Where(k => k.VoucherNumber == int.Parse(obj.voucherNo)).SingleOrDefault();
            if (std != null)
            {
                std.StockTransferDetailsUnitId = int.Parse(obj.unit);
                std.StockTransferDetailsMaterialId = int.Parse(obj.itemId);
                std.StockTransferDetailsQty = double.Parse(obj.quantity);
                std.StockTransferDetailsRate = double.Parse(obj.amount);
                std.StockTransferDetailsRemarks = obj.narration;
                _StockTransferDetailRepo.Update(std);
            }
            else
            {
                return false;
            }
            StockRegister sr = _StockRegisterRepo.GetAsQueryable().Where(k => k.StockRegisterRefVoucherNo == obj.voucherNo).SingleOrDefault();
            if (sr != null)
            {
                sr.StockRegisterMaterialID = int.Parse(obj.itemId);
                sr.StockRegisterQuantity = int.Parse(obj.quantity);
                sr.StockRegisterAmount = int.Parse(obj.amount);
                sr.StockRegisterFCAmount = int.Parse(obj.amount);
                sr.StockRegisterStatus = "S";
                sr.StockRegisterTransType = "Stock Transfer";
                sr.StockRegisterRemarks = obj.narration;
                sr.StockRegisterLocationID = int.Parse(obj.locationTo);
                sr.StockRegisterUnitID = int.Parse(obj.unit);
                sr.StockRegisterJobID = int.Parse(obj.selectedJob);
                sr.StockRegisterDepID = int.Parse(obj.department);
                _StockRegisterRepo.Update(sr);
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}