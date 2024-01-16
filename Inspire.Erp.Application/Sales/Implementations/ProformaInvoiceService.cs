using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Sales.Interface;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Domain.Entities;
namespace Inspire.Erp.Application.Sales.Implementation
{
    public class ProformaInvoiceService : IProformaInvoice
    {
        private IRepository<ProformaInvoice> repository;
        private IRepository<ProformaInvoiceDetails> detailrepository;

        public ProformaInvoiceService(IRepository<ProformaInvoice> repository, IRepository<ProformaInvoiceDetails> _detailrepository)
        {
            this.repository = repository;
            detailrepository = _detailrepository;
        }
        public IEnumerable<ProformaInvoice> DeleteProformaInvoice(int Id)
        {
            try
            {
                var dataObj = repository.GetAll().Where(o => o.ProformaInvoiceId == Id).FirstOrDefault();
                dataObj.ProformaInvoiceDelStatus = true;
                var detailData = detailrepository.GetAll().Where(o => o.ProformaInvoiceDetailsSlNo == Id).ToList();
                detailData.ForEach(elemt =>
                {
                    elemt.ProformaInvoiceDetailsDelStatus = true;
                });
                repository.Update(dataObj);
                detailrepository.UpdateList(detailData);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProformaInvoice GetProformaInvoiceById(int id)
        {
            try
            {
                return repository.GetAsQueryable().FirstOrDefault(x => x.ProformaInvoiceId == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<ProformaInvoice> GetProformaInvoices()
        {
            try
            {
                var detailList = detailrepository.GetAll().ToList();
                var data = repository.GetAll().Select(o => new
                {
                    ProformaInvoice = o,
                    ProformaInvoiceDetails = detailList.Where(a => a.ProformaInvoiceDetailsSlNo == o.ProformaInvoiceId).ToList()
                }).ToList();
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<ProformaInvoice> InsertProformaInvoice(ProformaInvoice proformaInvoice)
        {
            try
            {
                repository.Insert(proformaInvoice);
                proformaInvoice.ProformaInvoiceDetails.ForEach(elemt =>
                {
                    elemt.ProformaInvoiceDetailsSlNo = proformaInvoice.ProformaInvoiceId;
                });
                detailrepository.InsertList(proformaInvoice.ProformaInvoiceDetails);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<ProformaInvoice> UpdateProformaInvoice(ProformaInvoice proformaInvoice)
        {
            try
            {
                repository.Update(proformaInvoice);
                detailrepository.UpdateList(proformaInvoice.ProformaInvoiceDetails);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
