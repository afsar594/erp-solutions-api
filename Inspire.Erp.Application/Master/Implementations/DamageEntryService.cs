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
        private readonly IRepository<DamageEntryResponse> _DamageEntryResponse;
        private readonly IRepository<DamageMaster> _DamageMasterRepo;
        private readonly IRepository<DamageDetails> _DamageDetailsRepo;
        private readonly IRepository<VouchersNumbers> _VouchersNumbersRepo;
        private readonly IRepository<StockRegister> _StockRegisterRepo;
        private readonly IRepository<ProgramSettings> _programsettingsRepository;
        private readonly IRepository<ItemMaster> _itemMasterRepository;
        private readonly IRepository<LocationMaster> _locationMasterRepository;
        private static string prefix;
        public DamageEntry(IRepository<DamageDetails> _DetailsRepo,
            IRepository<DamageEntryResponse> _repo,
            IRepository<VouchersNumbers> _VNrepo,
            IRepository<StockRegister> _SRrepo,
            IRepository<DamageMaster> _rep,
            IRepository<ProgramSettings> programsettingsRepository,
              IRepository<ItemMaster> itemMasterRepository,
            IRepository<LocationMaster> locationMasterRepository)
        {
            _programsettingsRepository = programsettingsRepository;
            _VouchersNumbersRepo = _VNrepo;
            _StockRegisterRepo = _SRrepo;
            _DamageMasterRepo = _rep;
            _DamageEntryResponse = _repo;
            _DamageDetailsRepo = _DetailsRepo;
            _itemMasterRepository = itemMasterRepository;
            _locationMasterRepository = locationMasterRepository;
        }
        public VouchersNumbers DamageEntryPrefix()
        {
            try
            {
                prefix = _programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.DE_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_VouchersNumbersRepo.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.DamageEntry_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                //var prefix = "DE";
                //int vnoMaxVal = 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.DamageEntry_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = DateTime.Now,

                };
                _VouchersNumbersRepo.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public IEnumerable<DamageEntryModel> DamageEntryResponse()
        {
            try
            {
                var data = (from dm in _DamageMasterRepo.GetAsQueryable()
                            join dm_dt in _DamageDetailsRepo.GetAsQueryable()
                            on dm.DamageMasterVoucherNumber equals dm_dt.DamageDetailsVoucherNumber
                            join stk in _StockRegisterRepo.GetAsQueryable()
                            on dm.DamageMasterVoucherNumber equals stk.StockRegisterRefVoucherNo
                            join itmMaster in _itemMasterRepository.GetAsQueryable()
                            on stk.StockRegisterMaterialID equals Convert.ToInt32(itmMaster.ItemMasterItemId)
                            join loc in _locationMasterRepository.GetAsQueryable()
                            on dm.DamageMasterLocationId equals loc.LocationMasterLocationId
                            where (dm.DamageMasterDelStatus == false)
                            select new DamageEntryModel
                            {
                                LocationMasterLocationName = loc.LocationMasterLocationName,
                                ItemMasterItemId = itmMaster.ItemMasterItemId,
                                ItemMasterItemName = itmMaster.ItemMasterItemName,
                                DamageDetailsVoucherNumber = dm_dt.DamageDetailsVoucherNumber,
                                StockRegisterMaterialId = stk.StockRegisterMaterialID,
                                DamageMasterID = dm.DamageMasterId,
                                DamageMasterVdat = dm.DamageMasterVdate,
                                DamageMasterLocationID = dm.DamageMasterLocationId,
                                DamageMasterNarration = dm.DamageMasterNarration,
                                DamageDetailsMaterialID = dm_dt.DamageDetailsMaterialId,
                                DamageDetailsQTY = dm_dt.DamageDetailsQty,
                                DamageDetailsPrice = dm_dt.DamageDetailsPrice,
                                DamageDetailsAmount = dm_dt.DamageDetailsAmount,
                                DamageDetailsRemarks = dm_dt.DamageDetailsRemarks,
                                DamageDetailsUnitID = dm_dt.DamageDetailsUnitId
                            }).Distinct().ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> submitDamageEntry(DemageRequest obj)
        {
            try
            {
                _VouchersNumbersRepo.Insert(obj.voucherNumberList[0]);
                _DamageMasterRepo.InsertList(obj.damageMasterList);
                _DamageDetailsRepo.InsertList(obj.damageDetailsList);
                _StockRegisterRepo.InsertList(obj.stockRegisterList);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }
        public async Task<bool> deleteDamageEntry(DamageMaster DamageMaster)
        {
            try
            {
                DamageMaster dm = _DamageMasterRepo.GetAsQueryable().Where(k => k.DamageMasterVoucherNumber == DamageMaster.DamageMasterVoucherNumber).FirstOrDefault();
                dm.DamageMasterDelStatus = true;
                _DamageMasterRepo.Update(dm);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        public async Task<bool> updateDamageEntry(DamageEntryResponse Obj)
        {
            try
            {
                DamageMaster damageMaster = _DamageMasterRepo.GetAsQueryable().Where(k => k.DamageMasterVoucherNumber == Obj.Damage_Details_Voucher_Number).FirstOrDefault();
                if (damageMaster == null)
                {
                    return false;
                }
                damageMaster.DamageMasterNarration = Obj.Damage_Master_Narration;
                _DamageMasterRepo.Update(damageMaster);

                DamageDetails damageDetails = _DamageDetailsRepo.GetAsQueryable().Where(k => k.DamageDetailsVoucherNumber == Obj.Damage_Details_Voucher_Number).FirstOrDefault();
                if (damageDetails == null)
                {
                    return false;
                }
                damageDetails.DamageDetailsQty = Obj.Damage_Details_QTY;
                damageDetails.DamageDetailsAmount = Obj.Damage_Details_Amount;
                damageDetails.DamageDetailsRemarks = Obj.Damage_Details_Remarks;
                _DamageDetailsRepo.Update(damageDetails);

                StockRegister stockRegister = _StockRegisterRepo.GetAsQueryable().Where(k => k.StockRegisterRefVoucherNo == Obj.Damage_Details_Voucher_Number).FirstOrDefault();
                if (stockRegister == null)
                {
                    return false;
                }
                stockRegister.StockRegisterUsedQTY = decimal.Parse(Obj.Damage_Details_QTY.ToString());
                stockRegister.StockRegisterSout = decimal.Parse(Obj.Damage_Details_QTY.ToString());
                stockRegister.StockRegisterAmount = decimal.Parse(Obj.Damage_Details_Amount.ToString());
                stockRegister.StockRegisterFCAmount = decimal.Parse(Obj.Damage_Details_Amount.ToString());
                stockRegister.StockRegisterRemarks = Obj.Damage_Details_Remarks;
                _StockRegisterRepo.Update(stockRegister);
                return true;
            }
            catch (Exception ex)
            {
                throw;
                return false;
            }
        }
    }
}