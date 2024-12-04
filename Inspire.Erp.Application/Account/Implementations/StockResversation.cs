using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.implementations
{
    public class StockResversation : IStockResversation
    {
        IRepository<StockReservation> _stockReservationrepository;
        IRepository<StockReservationDetails> _stockReservationDetailsrepository;
        public StockResversation(
             IRepository<StockReservation> stockReservationrepository,
              IRepository<StockReservationDetails> stockReservationDetailsrepository)
        {
            _stockReservationrepository = stockReservationrepository;
            _stockReservationDetailsrepository = stockReservationDetailsrepository;
        }

        public async Task<StockReservationModel> InsertStockReservation(StockReservation stockReservation, List<StockReservationDetails> stockReservationDetails)
        {
            try
            {
                 _stockReservationrepository.BeginTransaction();

                int maxcount = 0;
                maxcount = Convert.ToInt32(_stockReservationrepository.GetAsQueryable().DefaultIfEmpty().Max(o => o == null ? 0 : o.StockReservationId) + 1);
                stockReservation.StockReservationId = maxcount;

                _stockReservationrepository.Insert(stockReservation);                

                int maxcount1 = 0;
                maxcount1 = Convert.ToInt32(_stockReservationDetailsrepository.GetAsQueryable().DefaultIfEmpty().Max(o => o == null ? 0 : o.StockReservationDetailsRId) + 1);
                stockReservation.StockReservationDetails.ForEach(elemt =>
                {
                    elemt.StockReservationDetailsRId = maxcount1;
                    maxcount1++;
                });

                _stockReservationDetailsrepository.InsertList(stockReservation.StockReservationDetails);
                _stockReservationrepository.TransactionCommit();
                return this.GetstockReservationbyId(stockReservation.StockReservationId);
            }
            catch (Exception ex)
            {
                _stockReservationrepository.TransactionRollback();
                throw ex;
            }
        }

        public async Task<StockReservationModel> UpdateStockReservation(StockReservation stockReservation, List<StockReservationDetails> stockReservationDetails)
        {
            try
            {
                _stockReservationrepository.BeginTransaction();
                int maxcount1 = 0;
                maxcount1 = Convert.ToInt32(
                    _stockReservationDetailsrepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.StockReservationDetailsRId) + 1);

                foreach (var item in stockReservation.StockReservationDetails)
                    if (item.StockReservationDetailsRdId == 0)
                    {
                        item.StockReservationDetailsRdId = maxcount1;
                        _stockReservationDetailsrepository.Insert(item);
                    }
                    else
                    {
                        _stockReservationDetailsrepository.Update(item);
                    }
                _stockReservationrepository.Update(stockReservation);

                _stockReservationrepository.TransactionCommit();
                return this.GetstockReservationbyId(stockReservation.StockReservationId);
            }
            catch (Exception ex)
            {
                _stockReservationrepository.TransactionRollback();
                throw ex;
            }
        }

        public IEnumerable<StockReservation> DeleteStockReservation(int Id)
        {
            try
            {
                var dataObj = _stockReservationrepository.GetAsQueryable().Where(o => o.StockReservationId == Id).FirstOrDefault();
                dataObj.StockReservationDelStatus = true;
                var detailData = _stockReservationDetailsrepository.GetAsQueryable().Where(o => o.StockReservationDetailsRdId == Id).ToList();
                detailData.ForEach(elemt =>
                {
                    elemt.StockReservationDetailsDelStatus = true;
                });
                _stockReservationrepository.Update(dataObj);
                _stockReservationDetailsrepository.UpdateList(detailData);
                return _stockReservationrepository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StockReservationModel GetstockReservationbyId(int? Id)
        {
            try
            {
                StockReservationModel xs = new StockReservationModel();
                xs.StockReservation = _stockReservationrepository.GetAsQueryable().Where(k => k.StockReservationId == Id).FirstOrDefault();
                xs.StockReservationDetails = _stockReservationDetailsrepository.GetAsQueryable().Where(x => x.StockReservationDetailsRId == Id && (x.StockReservationDetailsDelStatus == false || x.StockReservationDetailsDelStatus == null)).ToList();
                return xs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public StockReservation GetStockReservationById(int? id)
        {
            try
            {
                var StockReservation = new StockReservation();
                StockReservation = _stockReservationrepository.GetAsQueryable().FirstOrDefault(x => x.StockReservationId == Convert.ToInt32(id));
                StockReservation.StockReservationDetails = _stockReservationDetailsrepository.GetAsQueryable().Where(x => x.StockReservationDetailsRId == Convert.ToInt32(id) && x.StockReservationDetailsDelStatus != true).ToList();
                return StockReservation;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
       
}
