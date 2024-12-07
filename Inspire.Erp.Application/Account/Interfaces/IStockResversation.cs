using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace Inspire.Erp.Application.Account.Interface
{
    public interface IStockResversation
    {
         public Task<StockReservationModel> InsertStockReservation(StockReservation StockReservation, List<StockReservationDetails> jobInvoiceDetails);
        public Task<StockReservationModel> UpdateStockReservation(StockReservation StockReservation, List<StockReservationDetails> jobInvoiceDetails);
        
        public IEnumerable<StockReservation> DeleteStockReservation(int Id);
        public StockReservation GetStockReservationById(int? id);

        

    }  
}
