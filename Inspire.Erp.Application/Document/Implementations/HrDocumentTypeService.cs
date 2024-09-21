using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Account.Interfaces;
using System.IO.Enumeration;
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Application.Sales.Interface;
using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Spire.Pdf.General.Paper.Uof;
using Serilog;
namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentTypeService : IHrDocumentTypeService
    {
        private IRepository<HrDocumentType> _HrDocumentTypeRepo;
        private IRepository<StationMaster> _StationMasterRepo;
        private readonly IRepository<ProgramSettings> _programsettingsrepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        public HrDocumentTypeService(IRepository<HrDocumentType> HrDocumentTypeRepo, IRepository<StationMaster> stationMasterRepo, IRepository<ProgramSettings> programsettingsrepository, IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<ProgramSettings> programsettingsRepository)
        {
            _HrDocumentTypeRepo = HrDocumentTypeRepo;
            _StationMasterRepo = stationMasterRepo;
            _programsettingsrepository = programsettingsrepository;
            _voucherNumbersRepository = voucherNumbersRepository;
            _programsettingsRepository = programsettingsRepository;
        }
        public HrDocumentType InsertDocumentType(HrDocumentType HrDocumentType)
        { 
            bool valid = false;
            try
            {
                valid = true;


                string openingstockVoucherNumber = this.GenerateVoucherNo(System.DateTime.Now).VouchersNumbersVNo;
                HrDocumentType.DocumentvoucherNo = openingstockVoucherNumber;

                _HrDocumentTypeRepo.BeginTransaction();

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _HrDocumentTypeRepo.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.DocumentTypeId) + 1);
                HrDocumentType.DocumentTypeId = maxcount;

                HrDocumentType.DocumentTypeId = maxcount;

                _HrDocumentTypeRepo.Insert(HrDocumentType);


                _HrDocumentTypeRepo.TransactionCommit();

                return this.GetAllHrDocumentTypebyvoucher(HrDocumentType.DocumentvoucherNo);

            }
            catch (Exception ex)
            {
                _HrDocumentTypeRepo.TransactionRollback();
                throw ex;
            }
           
        }
        public HrDocumentType UpdateDocumentType(HrDocumentType HrDocumentType)
        {
            try
            {
                _HrDocumentTypeRepo.BeginTransaction();

                int maxcount1 = 0;
                maxcount1 = Convert.ToInt32(
                    _HrDocumentTypeRepo.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.DocumentTypeId) + 1);

                _HrDocumentTypeRepo.Update(HrDocumentType);
                _HrDocumentTypeRepo.TransactionCommit();

                return this.GetAllHrDocumentTypebyvoucher(HrDocumentType.DocumentvoucherNo);
            }
            catch (Exception ex)
            {
                _HrDocumentTypeRepo.TransactionRollback();
                throw ex;
            }
        }
        public IEnumerable<HrDocumentType> DeleteDocumentType(long Id)
        {
            try
            {
                var dataObj = _HrDocumentTypeRepo.GetAsQueryable().Where(o => o.DocumentTypeId == Id).FirstOrDefault();
                dataObj.Deletestatus = true;
                _HrDocumentTypeRepo.Update(dataObj);
                return _HrDocumentTypeRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public HrDocumentType GetAllHrDocumentTypebyvoucher(string VNo)
        {
            try
            {
                HrDocumentType HrDocumentTypes = new HrDocumentType();
                HrDocumentTypes = _HrDocumentTypeRepo.GetAsQueryable().Where(k => k.DocumentvoucherNo == VNo).SingleOrDefault();
                return HrDocumentTypes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public HrDocumentType GetDocumentTypeById(int? id)
        {
            try
            {
                var HrDocumentType = new HrDocumentType();
                HrDocumentType = _HrDocumentTypeRepo.GetAsQueryable().FirstOrDefault(x => x.DocumentTypeId == Convert.ToInt32(id));
                return HrDocumentType;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<HrDocumentType> GetAllHrDocumentType()
        {
            try
            {
                var data = _HrDocumentTypeRepo.GetAll();
                return data;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<StationMasterList> GetAllCompanies()
        {
            IEnumerable<StationMasterList> StationMaster;
            try
            {
              
                return _StationMasterRepo.GetAll().Where(a => a.StationMasterDelStatus != true)
                    .Select(c => new StationMasterList { StationMasterCode = c.StationMasterCode, StationMasterStationName = c.StationMasterStationName });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //_StationMasterRepo.Dispose();
            }
            //return StationMaster;
        }
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.DocumentVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.DocumentVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.DocumentVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
