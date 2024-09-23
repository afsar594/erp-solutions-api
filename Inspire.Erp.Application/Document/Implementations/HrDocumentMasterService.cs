using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Stock;
using Inspire.Erp.Domain.Modals;
using SendGrid.Helpers.Mail;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.DTO.WorkGropPermission;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentMasterService : IHrDocumentMasterService
    {
        private IRepository<DocumentMaster> repository;
        private IRepository<DocumentMaster> _documentMasterrepository;
        private readonly IHrDocumentTypeService _documentTypeRepoService;
        private readonly IHrStaffMasterService _staffRepoService;
        private readonly IHrVehicleMasterService _vehicleRepoService;
        private readonly IRepository<ProgramSettings> _programsettingsrepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly InspireErpDBContext _Context;
        private IRepository<HrDocumentType> _documentHrDocumentTypey;
        private IRepository<StationMaster> _stationMaster;
        public HrDocumentMasterService(IRepository<DocumentMaster> repository, IHrDocumentTypeService documentTypeRepo, IHrStaffMasterService staffRepo
        , IHrVehicleMasterService vehicleRepo, InspireErpDBContext Context, IRepository<DocumentMaster> documentMasterrepository, IRepository<ProgramSettings> programsettingsrepository, 
            IRepository<ProgramSettings> programsettingsRepository, IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<HrDocumentType> documentHrDocumentTypey, IRepository<StationMaster> stationMaster)
        {
            this.repository = repository;
            _documentTypeRepoService = documentTypeRepo;
            _staffRepoService = staffRepo;
            _vehicleRepoService = vehicleRepo;
            _Context = Context;
            _documentMasterrepository = documentMasterrepository;
            _programsettingsrepository = programsettingsrepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbersRepository;
            _documentHrDocumentTypey = documentHrDocumentTypey;
            _stationMaster = stationMaster;
        }
        //Start

        public DocumentMaster InsertHrDocumentMaster(DocumentMaster hrDocumentMaster)
        {
            bool valid = false;
            try
            {
                valid = true;
                string openingstockVoucherNumber = this.GenerateVoucherNo(hrDocumentMaster.IssueDate).VouchersNumbersVNo;
                hrDocumentMaster.DocumentvoucherNo = openingstockVoucherNumber;

                _documentMasterrepository.BeginTransaction();
                _documentMasterrepository.Insert(hrDocumentMaster);
                _documentMasterrepository.TransactionCommit();
                
                return this.GetDocumentMasterbyvoucher(hrDocumentMaster.DocumentvoucherNo);

            }
            catch (Exception ex)
            {
                _documentMasterrepository.TransactionRollback();
                throw ex;
            }
        }
        public DocumentMaster GetDocumentMasterbyvoucher(string VNo)
        {
            try
            {
                DocumentMaster DocumentMaster = new DocumentMaster();
                DocumentMaster = _documentMasterrepository.GetAsQueryable().Where(k => k.DocumentvoucherNo == VNo).SingleOrDefault();
                return DocumentMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        public DocumentMaster UpdateHrDocumentMaster(DocumentMaster DocumentMasters)
        {
            try
            {
                _documentMasterrepository.BeginTransaction();

                 //DocumentMaster = _documentMasterrepository.GetAsQueryable().Where(rec => rec.DocumentId == Convert.ToInt32(DocumentMaster.DocumentId)).FirstOrDefault();

                _documentMasterrepository.Update(DocumentMasters);

                _documentMasterrepository.TransactionCommit();

                return this.GetDocumentMasterbyvoucher(DocumentMasters.DocumentvoucherNo);
            }
            catch (Exception ex)
            {
                _documentMasterrepository.TransactionRollback();
                throw ex;
            }
        }
        public IEnumerable<DocumentMaster> DeleteDocumentMaster(long Id)
        {
            try
            {
                var dataObj = _documentMasterrepository.GetAsQueryable().Where(o => o.DocumentId == Id).FirstOrDefault();


                dataObj.Status = true;

                _documentMasterrepository.Update(dataObj);
                return _documentMasterrepository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DocumentMaster GetDocumentMasterById(int? id)
        {
            try
            {
                var DocumentMaster = new DocumentMaster();
                DocumentMaster = _documentMasterrepository.GetAsQueryable().FirstOrDefault(x => x.DocumentId == Convert.ToInt32(id));
                return DocumentMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<DocumentMaster> GetAllHrDocumentMasters()
        {
            try
            {
                var data = _documentMasterrepository.GetAll().Where(a => a.Status != true);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
  
        public Response<List<DocumentMasterlist>> GetDocumentReport(Documentfilter model)
        {
                List<DocumentMasterlist> Document = new List<DocumentMasterlist>();
                var query = _documentMasterrepository.GetAsQueryable();

                if (model.DocumentTypeId > 0)
                {
                    query = query.Where(im => im.DocumentTypeId == model.DocumentTypeId && im.Status != true);
                }
                if (model.CompanyId > 0)
                {
                    query = query.Where(im => im.CompanyId == model.CompanyId && im.Status != true);
                }
                if (model.isDateChecked)
                {
                    query = query.Where(a => a.IssueDate >= model.IssueDatefrom && a.IssueDate <= model.IssueDateto);
                }
                Document = (from ss in query
                            join a in _documentHrDocumentTypey.GetAsQueryable() on ss.DocumentTypeId equals (long)a.DocumentTypeId
                            join s in _stationMaster.GetAsQueryable() on ss.CompanyId equals s.StationMasterCode
                            select new DocumentMasterlist
                            {
                                DocumentTypeId = ss.DocumentTypeId,
                                CompanyId = ss.CompanyId,
                                Companyname= s.StationMasterAddress ?? "",
                                Documenttype = a.DocumentType ?? "",
                                MasterType = ss.MasterType ?? "",
                                DocName = ss.DocName ?? "",
                                Remarks = ss.Remarks ?? "",
                                IssueDate = Convert.ToDateTime(ss.IssueDate),
                                DocumentvoucherNo = ss.DocumentvoucherNo ?? "",
                                DocNo = ss.DocNo
                            }).ToList();

                return new Response<List<DocumentMasterlist>>
                {
                    Valid = true,
                    Result = Document,
                    Message = "Document Data Found"
                };
            
        }

    }
}
