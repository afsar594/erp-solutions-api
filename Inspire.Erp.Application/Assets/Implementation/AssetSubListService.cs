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
        private IRepository<MasterAccountsTable> _masterAccountTable;

        public AssetSubListService(IRepository<AssetMaster> assetsubmaster, IRepository<VouchersNumbers> vouchersNumbers, IRepository<AssetSubListHeader> assets, IRepository<AssetSubMaster> assetSubMaster, IRepository<ProgramSettings> programsettingsRepository, IRepository<MasterAccountsTable> masterAccountTable)
        {
            _assetmaster = assetsubmaster;
            _vouchersNumbers = vouchersNumbers;
            _assetSubListHeader = assets;
            _assetSubMaster = assetSubMaster;
            _programsettingsRepository = programsettingsRepository;
            _masterAccountTable = masterAccountTable;
        }


        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                // Fetch the prefix for the voucher
                var prefix = _programsettingsRepository.GetAsQueryable()
                    .Where(k => k.ProgramSettingsKeyValue == Prefix.AssetSubMasterVoucher_Prefix)
                    .FirstOrDefault()?.ProgramSettingsTextValue ?? "ASM"; // Default to "AS" if not found

                // Find the highest existing voucher number and increment by 1
                int vnoMaxVal = Convert.ToInt32(_vouchersNumbers.GetAsQueryable()
                    .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.AssetSubMasterVoucher_TYPE)
                    .DefaultIfEmpty()
                    .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;

                // Create a new voucher with the incremented number
                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = "ASM" + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.AssetSubMasterVoucher_TYPE, // Ensure the type is correct
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

        public List<AssetSubMaster> InsertPermissionApproval(AssetSubMasterViewModel model)
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


        public AssetSubMasterListVM GetAssetSubMasterDetailById(int id)
        {
            try
            {
                var detailData = _assetSubMaster.GetAsQueryable()
                    .Where(x => x.SubAssetID == id)
                    .Select(x => new AssetSubMasterListVM
                    {

                        GroupID = x.GroupID,
                        SubAssetID = x.SubAssetID,
                        SlNo = (double?)x.SlNo,
                        AssetDescription = x.AssetDescription,
                        AssetCode = x.AssetCode,
                        PurchDate = x.PurchDate,
                        PurchPrice = (double?)x.PurchPrice,
                        DepPerc = (double?)x.DepPerc,
                        DepDate = x.DepDate,
                        Days = (double?)x.Days,
                        PerDay = (double?)x.PerDay,
                        AccDep = (double?)x.AccDep,
                        BookVaue = (double?)x.BookVaue,
                        VoucherNo = x.VoucherNo,
                        IsEdit = x.IsEdit,
                        Remarks = x.Remarks
                    })
                    .FirstOrDefault(); // Get the first match or null if not found

                return detailData;
            }
            catch (Exception ex)
            {
                throw ex; // Consider logging the exception as well
            }
        }


        public List<AssetSubMasterListVM> GetAllAssetSubMasterDetails()
        {
            try
            {
                var detailData = _assetSubMaster.GetAsQueryable()
                    .Select(x => new AssetSubMasterListVM
                    {
                        GroupID = x.GroupID,
                        SubAssetID = x.SubAssetID,
                        SlNo = (double?)x.SlNo,
                        AssetDescription = x.AssetDescription,
                        AssetCode = x.AssetCode,
                        PurchDate = x.PurchDate,
                        PurchPrice = (double?)x.PurchPrice,
                        DepPerc = (double?)x.DepPerc,
                        DepDate = x.DepDate,
                        Days = (double?)x.Days,
                        PerDay = (double?)x.PerDay,
                        AccDep = (double?)x.AccDep,
                        BookVaue = (double?)x.BookVaue,
                        VoucherNo = x.VoucherNo,
                        IsEdit = x.IsEdit,
                        Remarks = x.Remarks
                    })
                    .ToList(); // Materialize the query to a List

                return detailData;
            }
            catch (Exception ex)
            {
                // Log the exception
               // _logger.LogError(ex, "Error retrieving all AssetSubMaster records");
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<List<MasterAccountsTable>> GetAllAssets()
        {
            try
            {
                var detailData = await _masterAccountTable.GetAsQueryable()
                    .Where(x => x.MaSubHead == "Fixed Asset")
                    .ToListAsync();
                return detailData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteAssetSubMasterById(int id)
        {
            try
            {
                var assetSubMaster = _assetSubMaster.GetAsQueryable().FirstOrDefault(x => x.SubAssetID == id);

                if (assetSubMaster == null)
                {
                    return false; // Return false if the record doesn't exist
                }

                _assetSubMaster.Delete(assetSubMaster); // Assuming your repository has a Delete method
                _assetSubMaster.SaveChangesAsync(); // Save changes to the database

                return true; // Return true if deletion was successful
            }
            catch (Exception ex)
            {
                throw ex; // Consider logging the error and returning an appropriate result
            }
        }
    }

}
