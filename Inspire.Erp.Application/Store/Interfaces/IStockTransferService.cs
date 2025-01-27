using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Store.Interface
{
    public interface IStockTransferService
    {
        public Task<Response<decimal>> getStockItemResponse( int? itemId,int? locationId);
        public Task<StockTransferModel> InsertStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
             , List<StockRegister> stockRegister);
        //public Task<bool>  ddeleteStockTransfer(StockTransferRequestModel obj);

        public Task<bool> DeleteStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
         , List<StockRegister> stockRegister);
        //public Task<bool> UpdateStockTransfer(StockTransferRequestModel obj);

        public Task<StockTransferModel> UpdateStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
            , List<StockRegister> stockRegister);
        public IEnumerable<StockTransferResponseModel> getStockTransferReport();

        public IEnumerable<StockTransfer> GetStockTransfer();
        public StockTransferModel GetSavedStockTransferDetails(int pvno);
        public IEnumerable<StockTransfer> DeleteStockTransfers(string Id);
        public ResponseInfo getStockitembyId(int Id);
        public Task<ResponseInfo> getStockTransferReport(reportModel model);
    }
}