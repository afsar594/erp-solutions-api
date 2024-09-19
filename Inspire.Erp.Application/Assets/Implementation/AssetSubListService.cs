using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Domain.Models.Approvals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using SendGrid.Helpers.Mail;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Application.Assets.Interface;
using Inspire.Erp.Domain.Models.ViewModels;




namespace Inspire.Erp.Application.Assets.Implementation
{
    public class AssetSubListService : IAssetSubListService
    {
        private IRepository<AssetMaster> _assetmaster;
        private IRepository<VouchersNumbers> _vouchersNumbers;
        private IRepository<AssetSubListHeader> _assetSubListHeader;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<AssetSubMaster> _assetSubMaster;

        public AssetSubListService(IRepository<AssetMaster> assetsubmaster, IRepository<VouchersNumbers> vouchersNumbers, IRepository<AssetSubListHeader> assets, IRepository<AssetSubMaster> assetSubMaster, IRepository<ProgramSettings> programsettingsRepository)
        {
            _assetmaster = assetsubmaster;
            _vouchersNumbers = vouchersNumbers;
            _assetSubListHeader = assets;
            _assetSubMaster = assetSubMaster;
            _programsettingsRepository = programsettingsRepository;
        }


        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = _programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.AssetSubMasterVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_vouchersNumbers.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.AssetSubMasterVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;

                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PurchaseVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _vouchersNumbers.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AssetSubMaster> InsertPermissionApproval( Erp.Domain.Models.ViewModels.AssetSubMasterViewModel model)
        {
            try

            {
                VouchersNumbers v = GenerateVoucherNo(DateTime.Now);

                string voucher = v.VouchersNumbersVNo;

              
               
                _assetSubMaster.BeginTransaction();

                foreach (var item in model.AssetSubMasterDetails)
                {
                    if (item.IsEdit == false)
                    {
                        model.AssetSubListHeader.VoucherNo = voucher;
                        item.VoucherNo = voucher;
                        _assetSubListHeader.Insert(model.AssetSubListHeader);
                        _assetSubMaster.Insert(item);
                    }
                    else
                    {
                        _assetSubListHeader.Update(model.AssetSubListHeader);
                        _assetSubMaster.Update(item);
                    }

                }
                _assetSubMaster.TransactionCommit();
                return model.AssetSubMasterDetails;
            }
            catch (Exception ex)
            {
                _assetSubMaster.TransactionRollback();
                throw ex;
            }
        }




        


        public IQueryable GetAllAssets()
        {
            try
            {
                var detailData = _assetmaster.GetAsQueryable()
                 .Select(x => new
                 {
                     x.AssetMasterAssetCode,
                     x.AssetMasterAssetName
                 });
                return detailData;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
