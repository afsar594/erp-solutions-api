using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master

{
    public class SupplierMasterService : ISupplierMasterService
    {
        private readonly IRepository<SuppliersMaster> supplierrepository;
        private readonly IRepository<PurchaseVoucherDetails> pVoucherDetailsrepo;
        private readonly IRepository<PurchaseVoucher> pVoucherrepo;
        private readonly IRepository<MasterAccountsTable> _masterAccountTable;
        private readonly IUtilityService _utilityService;
        private string SupplierRelativeNo = "";
        public SupplierMasterService(IRepository<SuppliersMaster> _supplierrepository, IRepository<PurchaseVoucher> _pVoucherrepo,
            IUtilityService utilityService, IConfiguration config,
                                     IRepository<MasterAccountsTable> masterAccountTable, IRepository<PurchaseVoucherDetails> _pVoucherDetailsrepo)
        {
            supplierrepository = _supplierrepository;
            pVoucherDetailsrepo = _pVoucherDetailsrepo;
            pVoucherrepo = _pVoucherrepo;
            _masterAccountTable = masterAccountTable;
            _utilityService = utilityService;
            SupplierRelativeNo = config["ApplicationSettings:SupplierRelativeNo"];
        }
        public async Task<IEnumerable<SuppliersMaster>> InsertSupplier(SuppliersMaster SuppliersMaster)
        {
            bool valid = false;
            try
            {
                var fs = await _utilityService.GetFinancialPeriods();
                valid = true;
                int mxc = 0;
                var maxId = _masterAccountTable.GetAsQueryable().Max(x => x.MaSno);
                string accNo = "AS" + maxId + 1;
                mxc = (int)supplierrepository.GetAsQueryable().Where(k => k.SuppliersMasterSupplierId != null).Select(x => x.SuppliersMasterSupplierId).Max();
                if (mxc == null) { mxc = 1; } else { mxc = mxc + 1; }

                SuppliersMaster.SuppliersMasterSupplierId = mxc;
                SuppliersMaster.SuppliersMasterSupplierReffAcNo = accNo;
                supplierrepository.Insert(SuppliersMaster);
                var masterAccount = _masterAccountTable.GetAsQueryable().Where(x => x.MaAccNo == SupplierRelativeNo).FirstOrDefault();
                MasterAccountsTable masterAccountsTable = new MasterAccountsTable()
                {
                    MaAccName = SuppliersMaster.SuppliersMasterSupplierName,
                    MaAccNo = accNo,
                    MaAccType = "S",
                    MaImageKey = masterAccount.MaImageKey,
                    MaAssetDate = DateTime.Now,
                    MaFsno = fs.Result.FinancialPeriodsFsno,
                    MaMainHead = masterAccount.MaMainHead,
                    MaRelativeNo = masterAccount.MaAccNo,
                    MaSubHead = masterAccount.MaSubHead,
                    MaUserId = 0,
                    MaDateCreated = DateTime.Now,
                    MaStatus = "R",

                };
                await _utilityService.SaveMasterAccountTable(masterAccountsTable);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel log = new AddActivityLogViewModel()
                {
                    Page = "Supplier Master",
                    Section = "Add Supplier Master",
                    Entity = "Supplier Master",

                };
                await _utilityService.AddUserTrackingLog(log);
                #endregion

                _masterAccountTable.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //currencyrepository.Dispose();
            }
            return this.GetAllSupplier();
        }
        public IEnumerable<SuppliersMaster> UpdateSupplier(SuppliersMaster SuppliersMaster)
        {
            bool valid = false;
            try
            {
                supplierrepository.Update(SuppliersMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //currencyrepository.Dispose();
            }
            return this.GetAllSupplier();
        }
        public IEnumerable<SuppliersMaster> DeleteSupplier(SuppliersMaster SuppliersMaster)
        {
            bool valid = false;
            try
            {
                supplierrepository.Delete(SuppliersMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //currencyrepository.Dispose();
            }
            return this.GetAllSupplier();
        }

        public IEnumerable<SuppliersMaster> GetAllSupplier()
        {
            IEnumerable<SuppliersMaster> SuppliersMaster;
            try
            {
                SuppliersMaster = supplierrepository.GetAll();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //currencyrepository.Dispose();
            }
            return SuppliersMaster;
        }
        public IEnumerable<SuppliersMaster> GetAllSupplierById(int id)
        {
            IEnumerable<SuppliersMaster> SuppliersMaster;
            try
            {
                SuppliersMaster = supplierrepository.GetAsQueryable().Where(k => k.SuppliersMasterSupplierId == id).Select(k => k);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return SuppliersMaster;

        }

        public IEnumerable<ItemMasterSupplierDetais> GetUpdatedSupplierDetailsByItem(int itemId)
        {
            //Select max(Pur_dt),MAX(PD.rate) as Rate,(isnull(S.INS_Code, '') + ' :: ' + S.INS_Name) as INS_Name from Purchase_Voucher_Details PD INNER JOIN
            //Purchase_Voucher P ON P.purchaseid = PD.purchaseid INNER JOIN Suppliers S ON P.Sp_Id = S.INS_ID Where mat_id = (Selectiing Items ItemID)

            IEnumerable<ItemMasterSupplierDetais> supplierDetails = (from PD in pVoucherDetailsrepo.GetAsQueryable()
                    join P in pVoucherrepo.GetAsQueryable()
                    on PD.PurchaseVoucherDetailsGrnNo equals P.PurchaseVoucherDayBookNo
                    join S in supplierrepository.GetAsQueryable()
                    //on P.PurchaseVoucherSPID equals S.SuppliersMasterSupplierId
                    on P.PurchaseVoucherSPID equals S.SuppliersMasterSupplierId
                    where PD.PurchaseVoucherDetailsMaterialId == itemId
                    select new
                    {
                        Supplier = S.SuppliersMasterSupplierName,
                        SupplierId = S.SuppliersMasterSupplierId,
                        PurchaseDate = P.PurchaseVoucherPurchaseDate,
                        Rate = PD.PurchaseVoucherDetailsRate
                    }).GroupBy(arg => new
                    {
                        arg.Supplier,
                        arg.SupplierId
                    }).Select(grouping => new ItemMasterSupplierDetais
                    {
                        Supplier = grouping.Key.Supplier,
                        SupplierId=  (int) grouping.Key.SupplierId,
                        PurchaseDate = Convert.ToDateTime(grouping.Max(arg => arg.PurchaseDate)),
                        Rate = (float)grouping.Max(arg => arg.Rate)
                    });


            return supplierDetails;
        }


    }
}
