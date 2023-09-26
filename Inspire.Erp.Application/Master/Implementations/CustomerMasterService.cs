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
    public class CustomerMasterService : ICustomerMasterService
    {
        private IRepository<CustomerMaster> customerrepository;
        private readonly IRepository<MasterAccountsTable> _masterAccountTable;
        private readonly IUtilityService _utilityService;
        private string DeptorsRelativeNo = "";

        public CustomerMasterService(IRepository<CustomerMaster> _customerrepository,IConfiguration config,
            IRepository<MasterAccountsTable> masterAccountTable, IUtilityService utilityService)
        {
            customerrepository = _customerrepository;
            _masterAccountTable = masterAccountTable;
            _utilityService = utilityService;
            DeptorsRelativeNo = config["ApplicationSettings:Debtors"];
        }
        public async Task<IEnumerable<CustomerMaster>> InsertCustomer(CustomerMaster customerMaster)
        {
            bool valid = false;
            try
            {
                var fs = await _utilityService.GetFinancialPeriods();
                valid = true;
                int mxc = 0;
                var maxId = _masterAccountTable.GetAsQueryable().Max(x => x.MaSno);
                string accNo = "AS" + maxId + 1;
                mxc = customerrepository.GetAsQueryable().Where(k => k.CustomerMasterCustomerNo != null).Select(x => x.CustomerMasterCustomerNo).Max();
                if (mxc == null) { mxc = 1; } else { mxc = mxc + 1; }

                customerMaster.CustomerMasterCustomerNo = mxc;
                customerMaster.CustomerMasterCustomerReffAcNo = accNo;
                customerrepository.Insert(customerMaster);
                var masterAccount = _masterAccountTable.GetAsQueryable().Where(x => x.MaAccNo == DeptorsRelativeNo).FirstOrDefault();
                var subhead = _masterAccountTable.GetAsQueryable().Where(x => x.MaRelativeNo == masterAccount.MaAccNo).FirstOrDefault();
                var head = _masterAccountTable.GetAsQueryable().Where(x => x.MaRelativeNo == subhead.MaAccNo).FirstOrDefault();
                MasterAccountsTable masterAccountsTable = new MasterAccountsTable()
                {
                    MaAccName = customerMaster.CustomerMasterCustomerName,
                    MaAccNo= accNo,
                    MaAccType = "G",
                    MaImageKey = masterAccount.MaImageKey,
                    MaAssetDate = DateTime.Now,
                    MaFsno = fs.Result.FinancialPeriodsFsno,
                    MaMainHead = head.MaMainHead,
                    MaRelativeNo = masterAccount.MaAccNo,
                    MaSubHead = subhead.MaSubHead,
                    MaUserId = 0,
                    MaDateCreated = DateTime.Now,
                    MaStatus = "R",

                };
                await _utilityService.SaveMasterAccountTable(masterAccountsTable);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel log = new AddActivityLogViewModel()
                {
                    Page = "Customer Master",
                    Section = "Add Customer Master",
                    Entity = "Customer Master",

                };
                await _utilityService.AddUserTrackingLog(log);
                #endregion
                customerrepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //customerrepository.Dispose();
            }
            return this.GetAllCustomer();
        }
        public IEnumerable<CustomerMaster> UpdateCustomer(CustomerMaster customerMaster)
        {
            bool valid = false;
            try
            {
                customerrepository.Update(customerMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //customerrepository.Dispose();
            }
            return this.GetAllCustomer();
        }
        public IEnumerable<CustomerMaster> DeleteCustomer(CustomerMaster customerMaster)
        {
            bool valid = false;
            try
            {
                customerrepository.Delete(customerMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //customerrepository.Dispose();
            }
            return this.GetAllCustomer();
        }
        public IEnumerable<CustomerMaster> GetAllCustomer()
        {
            IEnumerable<CustomerMaster> customerMasters;
            try
            {
                customerMasters = customerrepository.GetAll();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //customerrepository.Dispose();
            }
            return customerMasters;
        }

        public IEnumerable<CustomerMaster> GetAllCustomerById(int id)
        {
            IEnumerable<CustomerMaster> customerMasters;
            try
            {
                customerMasters = customerrepository.GetAsQueryable().Where(k => k.CustomerMasterCustomerNo == id).Select(k => k);

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //cityrepository.Dispose();
            }
            return customerMasters;

        }
    }
}
