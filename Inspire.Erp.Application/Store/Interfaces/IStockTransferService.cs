using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Store.Interface
{
    public interface IStockTransferService
    {
        public Task<List<StockItemResponse>> getStockItemResponse();
        public Task<bool> InsertStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
             , List<StockRegister> stockRegister);
        //public Task<bool> deleteStockTransfer(StockTransferRequestModel obj);

        public Task<bool> DeleteStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
         , List<StockRegister> stockRegister);
        //public Task<bool> UpdateStockTransfer(StockTransferRequestModel obj);

        public Task<bool> UpdateStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
            , List<StockRegister> stockRegister);
        public IEnumerable<StockTransferResponseModel> getStockTransferReport();
    }
}