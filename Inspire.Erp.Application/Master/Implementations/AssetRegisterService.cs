using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.DTO.Asset_Register;
using Inspire.Erp.Domain.Entities;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace Inspire.Erp.Application.Master.Implementations
{

    public class AssetRegisterService : IAssetRegisterService
    {
        private readonly InspireErpDBContext _context;
        private readonly IRepository<AssetPurchaseVoucherDetails> _assetPurchaseVoucherDetails;
        public AssetRegisterService(InspireErpDBContext context, IRepository<AssetPurchaseVoucherDetails> assetPurchaseVoucherDetails)
        {
            _context = context;
            _assetPurchaseVoucherDetails = assetPurchaseVoucherDetails;
        }
        public async Task<List<AssetRegisterDto>> GetAssetsRegisterAsync(DateTime? fromDate, DateTime? toDate, string searchText)
        {
            try
            {
                var result = await (
                            from p in _context.AssetPurchaseVoucherDetails
                            join pd in _context.AssetPurchaseVoucher on p.AstPurchaseID equals pd.AstPurID
                            join s in _context.Suppliers on pd.SupplierId equals s.SuppliersInsId into supplierJoin
                            from supplier in supplierJoin.DefaultIfEmpty() // LEFT JOIN for Suppliers
                            join a in _context.AssetMaster on p.AstID equals a.AssetMasterAssetId into assetJoin
                            from asset in assetJoin.DefaultIfEmpty() // LEFT JOIN for AssetMaster
                            where (fromDate == null || p.AstPurDate >= fromDate) &&
                            (toDate == null || p.AstPurDate <= toDate) &&
                            (string.IsNullOrEmpty(searchText) ||
                            (asset != null && asset.AssetMasterAssetName.Contains(searchText)) ||
                           (supplier != null && supplier.SuppliersInsName.Contains(searchText)))
                            select new
                            {
                                AstPurchaseID = p.AstPurchaseID,
                                AstID = p.AstID,
                                SupplierName = supplier != null ? supplier.SuppliersInsName : null,
                                Description = asset != null ? asset.AssetMasterAssetName : null,
                                AstPurDate = p.AstPurDate,
                                Amount = p.Amount,
                                DepPerc = p.DepPerc
                            }).ToListAsync();

                var assetDetails = new List<AssetRegisterDto>();

                foreach (var item in result)
                {
                    var accDepAmt = await CalculateAccDepAmount(item.AstPurchaseID.ToString());
                    var bookValue = Math.Max((decimal)item.Amount - (decimal)accDepAmt, 0);
                    var accDepPerc = item.Amount > 0 ? ((decimal)accDepAmt / item.Amount) * 100 : 0;
                    var depPercBal = 100 - accDepPerc;

                    assetDetails.Add(new AssetRegisterDto
                    {
                        PurchaseNo = item.AstPurchaseID,
                        AssetId = (decimal)item.AstID,
                        Supplier = item.SupplierName,
                        Description = item.Description,
                        PurchaseDate = item.AstPurDate.ToString(),
                        Amount = (decimal)accDepAmt,
                        BookValue = bookValue,
                        BalanceDepreciationPercentage = (decimal)item.DepPerc,
                        AccumulatedDepreciationPercentage = Math.Round((decimal)accDepPerc, 2),
                        DepPercBal = Math.Round((decimal)depPercBal, 2)
                    });

                  await UpdateAssetDetailsAsync(item.AstPurchaseID, item.AstID, accDepAmt, bookValue);
                }
                return assetDetails;
            }
            catch (Exception ex ) 
            {

                throw;
            }
        }
        private async Task<double> CalculateAccDepAmount(string astPurchaseId)
        {
            double totalAccDepAmount = 0;
            var depreciationRecords = await _context.AssetDepDetails
                .Where(d => d.AssetDepDetailsPurchaseId == astPurchaseId)
                .ToListAsync();

            if(depreciationRecords.Count > 0)
               totalAccDepAmount = (double)depreciationRecords.Sum(d => d.AssetDepDetailsDepAmount);


            return totalAccDepAmount;
        }
        private async Task UpdateAssetDetailsAsync(int astPurchaseId, decimal? astId, double accDepAmt, decimal bookValue)
        {
            
            var assetDetail = await _context.AssetPurchaseVoucherDetails
                .FirstOrDefaultAsync(p => p.AstPurchaseID == astPurchaseId && p.AstID == astId);

          
            if (assetDetail != null)
            {
                assetDetail.AccDepAmt = (decimal?)accDepAmt;
                assetDetail.BookValue = bookValue;
                await _context.SaveChangesAsync();
            }
        }

    }
}
