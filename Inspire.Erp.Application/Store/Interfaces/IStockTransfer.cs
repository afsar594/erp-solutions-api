using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Store.Interface
{
    public interface IStockTransfer
    {
        public IEnumerable<StockItemResponse> getStockItemResponse();
        public Task<bool> submitTransferReport(List<StockTransferRequestModel> obj);
        public Task<bool> deleteStockTransfer(StockTransferRequestModel obj);
        public Task<bool> UpdateStockTransfer(StockTransferRequestModel obj);
        public IEnumerable<StockTransferResponseModel> getStockTransferReport();
    }
}