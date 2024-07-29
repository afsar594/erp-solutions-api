using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.StoreWareHouse.Implementation
{
    public class DamageEntry : IDamageEntry
    {
        //private readonly IRepository<DamageEntryResponse> _DamageEntryResponse;
        private readonly IRepository<DamageMaster> _DamageMasterRepo;
        private readonly IRepository<DamageDetails> _DamageDetailsRepo;
        private readonly IRepository<VouchersNumbers> _VouchersNumbersRepo;
        private readonly IRepository<StockRegister> _StockRegisterRepo;
        private readonly IRepository<ProgramSettings> _programsettingsRepository;
        private readonly IRepository<ItemMaster> _itemMasterRepository;
        private readonly IRepository<LocationMaster> _locationMasterRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<StockRegister> _stockRegisterRepository;
        private static string prefix;
        public DamageEntry(IRepository<DamageDetails> _DetailsRepo,
            IRepository<VouchersNumbers> _VNrepo,
            IRepository<StockRegister> _SRrepo,
            IRepository<DamageMaster> _rep,
            IRepository<ProgramSettings> programsettingsRepository,
              IRepository<ItemMaster> itemMasterRepository,
            IRepository<LocationMaster> locationMasterRepository,
            IRepository<VouchersNumbers> voucherNumbersRepository,
            IRepository<StockRegister> stockRegisterRepository)
        {
            _programsettingsRepository = programsettingsRepository;
            _VouchersNumbersRepo = _VNrepo;
            _StockRegisterRepo = _SRrepo;
            _DamageMasterRepo = _rep;
            _DamageDetailsRepo = _DetailsRepo;
            _itemMasterRepository = itemMasterRepository;
            _locationMasterRepository = locationMasterRepository;
            _voucherNumbersRepository = voucherNumbersRepository;
            _stockRegisterRepository = stockRegisterRepository;
        }
        //public VouchersNumbers DamageEntryPrefix()
        //{
        //    try
        //    {
        //        prefix = _programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.DE_Prefix).FirstOrDefault().ProgramSettingsTextValue;
        //        int vnoMaxVal = Convert.ToInt32(_VouchersNumbersRepo.GetAsQueryable()
        //                                                .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.DamageEntry_TYPE)
        //                                                .DefaultIfEmpty()
        //                                                .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


        //        //var prefix = "DE";
        //        //int vnoMaxVal = 1;


        //        VouchersNumbers vouchersNumbers = new VouchersNumbers
        //        {
        //            VouchersNumbersVNo = prefix + vnoMaxVal,
        //            VouchersNumbersVNoNu = vnoMaxVal,
        //            VouchersNumbersVType = VoucherType.DamageEntry_TYPE,
        //            VouchersNumbersFsno = 1,
        //            VouchersNumbersStatus = AccountStatus.Pending,
        //            VouchersNumbersVoucherDate = DateTime.Now,

        //        };
        //        _VouchersNumbersRepo.Insert(vouchersNumbers);
        //        return vouchersNumbers;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        //public IEnumerable<DamageEntryModel> DamageEntryResponse()
        //{
        //    try
        //    {
        //        var data = (from dm in _DamageMasterRepo.GetAsQueryable()
        //                    join dm_dt in _DamageDetailsRepo.GetAsQueryable()
        //                    on dm.DamageMasterVoucherNumber equals dm_dt.DamageDetailsVoucherNumber
        //                    join stk in _StockRegisterRepo.GetAsQueryable()
        //                    on dm.DamageMasterVoucherNumber equals stk.StockRegisterRefVoucherNo
        //                    join itmMaster in _itemMasterRepository.GetAsQueryable()
        //                    on stk.StockRegisterMaterialID equals Convert.ToInt32(itmMaster.ItemMasterItemId)
        //                    join loc in _locationMasterRepository.GetAsQueryable()
        //                    on dm.DamageMasterLocationId equals loc.LocationMasterLocationId
        //                    where (dm.DamageMasterDelStatus == false)
        //                    select new DamageEntryModel
        //                    {
        //                        LocationMasterLocationName = loc.LocationMasterLocationName,
        //                        ItemMasterItemId = itmMaster.ItemMasterItemId,
        //                        ItemMasterItemName = itmMaster.ItemMasterItemName,
        //                        DamageDetailsVoucherNumber = dm_dt.DamageDetailsVoucherNumber,
        //                        StockRegisterMaterialId = stk.StockRegisterMaterialID,
        //                        DamageMasterID = dm.DamageMasterId,
        //                        DamageMasterVdat = dm.DamageMasterVdate,
        //                        DamageMasterLocationID = dm.DamageMasterLocationId,
        //                        DamageMasterNarration = dm.DamageMasterNarration,
        //                        DamageDetailsMaterialID = dm_dt.DamageDetailsMaterialId,
        //                        DamageDetailsQTY = dm_dt.DamageDetailsQty,
        //                        DamageDetailsPrice = dm_dt.DamageDetailsPrice,
        //                        DamageDetailsAmount = dm_dt.DamageDetailsAmount,
        //                        DamageDetailsRemarks = dm_dt.DamageDetailsRemarks,
        //                        DamageDetailsUnitID = dm_dt.DamageDetailsUnitId
        //                    }).Distinct().ToList();
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public async Task<bool> submitDamageEntry(DemageRequest obj)
        //{
        //    try
        //    {
        //        _VouchersNumbersRepo.Insert(obj.voucherNumberList[0]);
        //        _DamageMasterRepo.InsertList(obj.damageMasterList);
        //        _DamageDetailsRepo.InsertList(obj.damageDetailsList);
        //        _StockRegisterRepo.InsertList(obj.stockRegisterList);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //        return false;
        //    }
        //}

        public DamageMaster InsertDamageEntry(DamageMaster DamageMaster)
        {
            try
            {


                string openingstockVoucherNumber = this.GenerateVoucherNo(DamageMaster.DamageMasterVdate.Value).VouchersNumbersVNo;
                DamageMaster.DamageMasterVoucherNumber = openingstockVoucherNumber;

                _DamageMasterRepo.BeginTransaction();
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _DamageMasterRepo.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.DamageMasterId) + 1);
                DamageMaster.DamageMasterId = maxcount;

                _DamageMasterRepo.Insert(DamageMaster);

                DamageMaster.DamageDetails.ForEach(elemt =>
                {
                    elemt.DamageDetailsId = maxcount;
                    elemt.DamageDetailsVoucherNumber = DamageMaster.DamageMasterVoucherNumber;
                });
                _DamageDetailsRepo.InsertList(DamageMaster.DamageDetails);

                var stockReg = new List<StockRegister>();
                foreach (var item in DamageMaster.DamageDetails)
                {

                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = DamageMaster.DamageMasterVoucherNumber,
                        StockRegisterRefVoucherNo = DamageMaster.DamageMasterVoucherNumber,
                        StockRegisterVoucherDate = DamageMaster.DamageMasterVdate,
                        StockRegisterMaterialID = item.DamageDetailsMaterialId,
                        StockRegisterQuantity = (decimal)item.DamageDetailsQty,
                        StockRegisterSIN = (decimal)item.DamageDetailsQty,
                        StockRegisterSout = (decimal)item.DamageDetailsQty,
                        StockRegisterRate = 0,
                        StockRegisterAmount = (decimal)item.DamageDetailsPrice,
                        StockRegisterFCAmount = 0,
                        StockRegisterAssignedDate = DamageMaster.DamageMasterVdate,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterTransType = VoucherType.DamageEntry_TYPE,
                        StockRegisterUnitID = item.DamageDetailsUnitId,
                        StockRegisterLocationID = DamageMaster.DamageMasterLocationId,
                        StockRegisterExpDate = null,
                        StockRegisterDepID = 1,
                        StockRegisterDelStatus = false
                    });
                }
                _stockRegisterRepository.InsertList(stockReg);


                _DamageMasterRepo.TransactionCommit();
                return this.GetDamageById(DamageMaster.DamageMasterId);
            }
            catch (Exception ex)
            {
                _DamageMasterRepo.TransactionRollback();
                throw ex;
            }
        }
        public DamageMaster UpdateDamageEntry(DamageMaster DamageMaster)
        {
            try
            {
                _DamageMasterRepo.BeginTransaction();
                _DamageMasterRepo.Update(DamageMaster);
                _DamageDetailsRepo.UpdateList(DamageMaster.DamageDetails);
                _DamageMasterRepo.TransactionCommit();
                return this.GetDamageById(DamageMaster.DamageMasterId);
            }
            catch (Exception ex)
            {
                _DamageMasterRepo.TransactionRollback();
                throw ex;
            }
        }
        public DamageMaster GetDamageById(int? id)
        {
            try
            {
                var DamageMaster = new DamageMaster();
                DamageMaster = _DamageMasterRepo.GetAsQueryable().FirstOrDefault(x => x.DamageMasterId == Convert.ToInt32(id));
                DamageMaster.DamageDetails = _DamageDetailsRepo.GetAsQueryable().Where(x => x.DamageDetailsId == Convert.ToInt32(id)).ToList();
                return DamageMaster;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<DamageMaster> GetDamageEntry()
        {
            try
            {
                var detailList = _DamageDetailsRepo.GetAll().ToList();
                var data = _DamageMasterRepo.GetAll().Select(o => new
                {
                    DamageMaster = o,
                    _DamageDetailsRepo = detailList.Where(a => Convert.ToInt32(a.DamageDetailsId) == Convert.ToInt32(o.DamageMasterId)).ToList()
                }).ToList();
                return _DamageMasterRepo.GetAll();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<DamageMaster> DeleteDamageEntry(string Id)
        {
            try
            {
                var dataObj = _DamageMasterRepo.GetAsQueryable().Where(o => o.DamageMasterVoucherNumber == Id).FirstOrDefault();
                dataObj.DamageMasterDelStatus = true;
                var detailData = _DamageDetailsRepo.GetAsQueryable().Where(o => o.DamageDetailsVoucherNumber == Id).ToList();
                detailData.ForEach(elemt =>
                {
                    elemt.DamageDetailsDelStatus = true;
                });
                _DamageMasterRepo.Update(dataObj);
                _DamageDetailsRepo.UpdateList(detailData);
                return _DamageMasterRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.DE_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.DamageEntry_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.DamageEntry_TYPE,
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
        public DamageMaster GetDamageEntryVoucherNo(string VNo)
        {
            try
            {

                DamageMaster Damageentry = new DamageMaster();
                Damageentry = _DamageMasterRepo.GetAsQueryable().Where(k => k.DamageMasterVoucherNumber == VNo).SingleOrDefault();
                Damageentry.DamageDetails = _DamageDetailsRepo.GetAsQueryable().Where(x => x.DamageDetailsVoucherNumber == VNo && (x.DamageDetailsDelStatus == false || x.DamageDetailsDelStatus == null)).ToList();
                return Damageentry;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
