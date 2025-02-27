using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Sales.Interface;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Enums;
namespace Inspire.Erp.Application.Sales.Implementation
{
    public class ProformaInvoiceService : IProformaInvoice
    {
        private IRepository<ProformaInvoice> repository;
        private IRepository<ProformaInvoiceDetails> detailrepository;
        private IRepository<Domain.Entities.ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;

        public ProformaInvoiceService(IRepository<ProformaInvoice> repository, IRepository<ProformaInvoiceDetails> _detailrepository, IRepository<Domain.Entities.ProgramSettings> programsettingsRepository, IRepository<VouchersNumbers> voucherNumbersRepository)
        {
            this.repository = repository;
            detailrepository = _detailrepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbersRepository;

        }
        public IEnumerable<ProformaInvoice> DeleteProformaInvoice(int Id)
        {
            try
            {
                var dataObj = repository.GetAsQueryable().Where(o => o.ProformaInvoiceId == Id).FirstOrDefault();
                dataObj.ProformaInvoiceDelStatus = true;
                var detailData = detailrepository.GetAsQueryable().Where(o => o.ProformaInvoiceDetailsSlNo == Id).OrderBy(x => x.ProformaInvoiceDetailsSlNo).AsEnumerable().ToList();
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

        public ProformaInvoice GetProformaInvoiceById(int? id)
        {
            try
            {
                var proformaInvoice = new ProformaInvoice();
                proformaInvoice = repository.GetAsQueryable().FirstOrDefault(x => x.ProformaInvoiceId == id);
                proformaInvoice.ProformaInvoiceDetails = detailrepository.GetAsQueryable().Where(x => x.ProformaInvoiceDetailsSlNo == id && x.ProformaInvoiceDetailsVoucherno!=null).ToList();
                return proformaInvoice;
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
                var detailList = detailrepository.GetAll().OrderBy(x => x.ProformaInvoiceDetailsSlNo).AsEnumerable().ToList();
                var data = repository.GetAll().Select(o => new
                {
                    ProformaInvoice = o,
                    ProformaInvoiceDetails = detailList.Where(a => a.ProformaInvoiceDetailsSlNo == o.ProformaInvoiceId).ToList()
                });
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProformaInvoice InsertProformaInvoice(ProformaInvoice proformaInvoice)
        {
            try
            {

                repository.BeginTransaction();

                string proformaVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.ProformaInvoiceVoucher_TYPE, Prefix.ProformaInvouce_PREFIX, _programsettingsRepository, _voucherNumbersRepository);
                proformaInvoice.ProformaInvoiceVoucherno = proformaVoucherNumber;

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    repository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.ProformaInvoiceId) + 1);
                proformaInvoice.ProformaInvoiceId = maxcount;

                repository.Insert(proformaInvoice);
                proformaInvoice.ProformaInvoiceDetails.ForEach(elemt =>
                {
                    elemt.ProformaInvoiceDetailsSlNo = maxcount;
                    elemt.ProformaInvoiceDetailsVoucherno = proformaVoucherNumber;

                });
                detailrepository.InsertList(proformaInvoice.ProformaInvoiceDetails);
                repository.TransactionCommit();
                return this.GetProformaInvoiceById(proformaInvoice.ProformaInvoiceId);
            }
            catch (Exception ex)
            {
                repository.TransactionRollback();
                throw ex;
            }
        }

        public ProformaInvoice UpdateProformaInvoice(ProformaInvoice proformaInvoice)
        {
            try
            {
                repository.BeginTransaction();
                repository.Update(proformaInvoice);

                proformaInvoice.ProformaInvoiceDetails.ForEach(elemt =>
                {
                    elemt.ProformaInvoiceDetailsSlNo = proformaInvoice.ProformaInvoiceId;
                    elemt.ProformaInvoiceDetailsVoucherno = proformaInvoice.ProformaInvoiceVoucherno;

                });
                detailrepository.UpdateList(proformaInvoice.ProformaInvoiceDetails);
                repository.TransactionCommit();
                return this.GetProformaInvoiceById(proformaInvoice.ProformaInvoiceId);
            }
            catch (Exception ex)
            {
                repository.TransactionRollback();
                throw ex;
            }
        }

        public VouchersNumbers GetVouchersNumbers(string pvno)
        {
            try
            {
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == pvno).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ProformaInvoice GetProformaInvoiceDetailsByNo(string ProformaInvoiceVoucherNo)
        {
            try
            {
                ProformaInvoice voucherModel = new ProformaInvoice();
                voucherModel = repository.GetAsQueryable().Where(a => a.ProformaInvoiceVoucherno == ProformaInvoiceVoucherNo).FirstOrDefault();
                voucherModel.ProformaInvoiceDetails = detailrepository.GetAsQueryable().Where(o => o.ProformaInvoiceDetailsVoucherno == ProformaInvoiceVoucherNo && o.ProformaInvoiceDetailsDelStatus != true).ToList();
                return voucherModel;
            }

            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
