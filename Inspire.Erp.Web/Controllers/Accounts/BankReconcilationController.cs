using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankReconcilationController : ControllerBase
    {
        public IBankReconcilation _ibn;
        public BankReconcilationController(IBankReconcilation ibn)
        {
            _ibn = ibn;
        }

        [HttpGet("GetAccountListForDropdown")]
        public async Task<List<MasterAccountsTable>> GetAccountListForDropdown()
        {
            return await _ibn.GetAccountListForDropdown();
        }
        [HttpGet("BankLedger/{bankAccNo}")]
        public async Task<IEnumerable<BankLedger>> BankLedger(string bankAccNo)
        {
            return await _ibn.BankLedger(bankAccNo);
        }
    }
}
