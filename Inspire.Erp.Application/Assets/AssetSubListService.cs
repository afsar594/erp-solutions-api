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
using Inspire.Erp.Domain.Models;


namespace Inspire.Erp.Application.Assets
{
    public class AssetSubListService : IAssetSubListService
    {
        private IRepository<AssetMaster> _assetsubmaster;
        private IRepository<VouchersNumbers> _vouchersNumbers;
        private IRepository<AssetSubListHeader> _assetSubListHeader;
        private IRepository<AssetSubMaster> _assetSubMaster;

        public AssetSubListService(IRepository<AssetMaster> assetsubmaster, IRepository<VouchersNumbers> vouchersNumbers, IRepository<AssetSubListHeader> assets, IRepository<AssetSubMaster> assetSubMaster)
        {
            _assetsubmaster = assetsubmaster;
            _vouchersNumbers = vouchersNumbers;
            _assetSubListHeader = assets;
            _assetSubMaster = assetSubMaster;
        }

        public List<AssetSubMaster> InsertPermissionApproval(List<AssetSubMaster> __AssetSubMaster)
        {
            try
            {
                _assetSubMaster.BeginTransaction();

                foreach (var item in __AssetSubMaster)
                {
                    if (item.IsEdit == false)
                    {
                        _assetSubMaster.Insert(item);
                    }
                    else 
                    {
                        _assetSubMaster.Update(item);
                    }
                   
                }
                _assetSubMaster.TransactionCommit();
                return __AssetSubMaster;
            }
            catch (Exception ex)
            {
                _assetSubMaster.TransactionRollback();
                throw ex;
            }
        }


        public async Task<string> GenerateVoucherNumberAsync(string assetsub)
        {
           

            // Get the latest voucher number for the asset type
            var latestVoucherNumber =  GetLatestVoucherNumberQuery(assetsub);

            // Increment to generate the new voucher number
            decimal newVoucherNumber = Convert.ToDecimal(latestVoucherNumber) + 1;
            // Combine prefix and new number to create the full voucher number
            string newVoucherNo = $"{newVoucherNumber}";

            // Insert the new voucher number into Vouchers_Numbers table
            var voucherNumber = new VouchersNumbers
            {
                VouchersNumbersVNo = newVoucherNo,
                VouchersNumbersVNoNu = newVoucherNumber,
                VouchersNumbersVType = assetsub,
                VouchersNumbersFsno = 1,
                VouchersNumbersStatus = "P",
                VouchersNumbersLocationId = 1
            };

            // Save the new voucher number to the database
            _vouchersNumbers.Insert(voucherNumber);
           

            // Return the generated voucher number
            return newVoucherNo;
        }


        public async Task<bool> CreateVoucherHeaderAsync(AssetSubListHeader model)
        {
            try
            {
                // Create the voucher header object
                var voucherHeader = new AssetSubListHeader
                {
                    VoucherNo = model.VoucherNo,           // Voucher Number
                    VoucherDate = DateTime.Now,      // Current Date
                    RefNo = model.RefNo,                   // Reference Number
                    Description = model.Description,       // Description or Narration
                    GrandTotal = model.GrandTotal          // Grand Total
                };

                // Add the voucher _header to the database
                _assetSubListHeader.Insert(voucherHeader);

                // Save changes to the database
              

                // Return true if the voucher was saved successfully
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle accordingly
                Console.WriteLine($"Error while saving voucher header: {ex.Message}");

                // Return false to indicate the save was unsuccessful
                return false;
            }
        }

        private IQueryable<object> GetLatestVoucherNumberQuery(string assetsub)
        {
            // Ensure that V_Type exists in the _vouchersNumbers entity and assetsub is valid
            var query = _vouchersNumbers.GetAsQueryable()
                .Where(v => v.VouchersNumbersVType == assetsub) // Check if V_Type matches assetsub
                .Select(v => new
                {
                    v.VouchersNumbersVNo,     // Voucher Number
                    v.VouchersNumbersVNoNu,  // Voucher Number Numeric
                    v.VouchersNumbersVType    // Voucher Type
                });

            return query;
        }



        public IQueryable<object> GetVoucherDetails()
        {
            var detailData = _assetsubmaster.GetAsQueryable()
                .Select(x => new
                {
                    x.AssetMasterAssetCode,
                    x.AssetMasterAssetName
                });

            return detailData;
        }



        public IQueryable GetAllAssets()
        {
            try
            {
                var detailData = _assetsubmaster.GetAsQueryable()
                 .Select(x => new
                 {
                     x.AssetMasterAssetCode,
                     x.AssetMasterAssetName
                 });
                return detailData;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }

}
