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

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentMasterService : IHrDocumentMasterService
    {
        private IRepository<HrDocumentMaster> repository;
        private readonly IHrDocumentTypeService _documentTypeRepoService;
        private readonly IHrStaffMasterService _staffRepoService;
        private readonly IHrVehicleMasterService _vehicleRepoService;
        private readonly InspireErpDBContext _Context;

        public HrDocumentMasterService(IRepository<HrDocumentMaster> repository, IHrDocumentTypeService documentTypeRepo, IHrStaffMasterService staffRepo
        , IHrVehicleMasterService vehicleRepo, InspireErpDBContext Context)
        {
            this.repository = repository;
            _documentTypeRepoService = documentTypeRepo;
            _staffRepoService = staffRepo;
            _vehicleRepoService = vehicleRepo;
            _Context = Context;
        }
        public IEnumerable<HrDocumentMaster> InsertHrDocumentMaster(HrDocumentMaster hrDocumentMaster)
        {
            try
            {
                this.repository.Insert(hrDocumentMaster);
                return GetAllStaffDocumentMaster();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentMaster> UpdateHrDocumentMaster(HrDocumentMaster hrDocumentMaster)
        {
            try
            {
                this.repository.Update(hrDocumentMaster);
                return GetAllStaffDocumentMaster();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentMaster> DeleteHrDocumentMaster(HrDocumentMaster hrDocumentMaster)
        {
            try
            {
                this.repository.Delete(hrDocumentMaster);
                return GetAllStaffDocumentMaster();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentMaster> GetAllHrDocumentMasters()
        {
            try
            {
                return this.repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<HrDocumentImages> getDocumentImagesByMasterId(long Id)
        {
            try
            {
                return _Context.HrDocumentImages.Where(o => o.DocumentId == Id).ToList();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable GetAllDocumentsStaff()
        {
            try
            {
                var dataList = _Context.HrDocumentMaster.Where(o => o.StaffId != null).ToList();
                var StaffList = _Context.HrStaffMaster.ToList();
                var finalData = dataList.Select(o => new
                {
                    StaffId = o.StaffId,
                    StaffName = StaffList.FirstOrDefault(s => s.StaffId == o.StaffId).FullName,
                }).GroupBy(g => new
                {
                    StaffId = g.StaffId
                }).Select(o => new
                {
                    StaffId = o.Key.StaffId,
                    StaffName = o.FirstOrDefault().StaffName,
                    MasterList = dataList.Where(d => d.StaffId == o.Key.StaffId).ToList(),
                }).AsQueryable();
                return finalData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable GetAllDocumentsVehicle()
        {
            try
            {
                var dataList = _Context.HrDocumentMaster.Where(o => o.VehicleId != null).ToList();
                var VehicleList = _Context.HrVehicleMaster.ToList();
                var finalData = dataList.Select(o => new
                {
                    VehicleId = o.VehicleId,
                    VehicleObj = VehicleList.FirstOrDefault(s => s.Id == o.VehicleId),
                }).GroupBy(g => new
                {
                    VehicleId = g.VehicleId
                }).Select(o => new
                {
                    VehicleId = o.Key.VehicleId,
                    VehicleName = o.FirstOrDefault().VehicleObj.VehicleNo + " " + o.FirstOrDefault().VehicleObj.VehicleTye,
                    MasterList = dataList.Where(d => d.VehicleId == o.Key.VehicleId).ToList(),
                }).AsQueryable();
                return finalData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<HrDocumentMaster> GetAllStaffDocumentMaster()
        {
            try
            {
                var masterData = repository.GetAll().Where(o => o.StaffId != null).Select(x => new HrDocumentMaster()
                {
                    DocumentId = x.DocumentId,
                    DocumentTypeId = x.DocumentTypeId,
                    StaffId = x.StaffId,
                    CompanyId = x.CompanyId,
                    VehicleId = x.VehicleId,
                    OtherId = x.OtherId,
                    MasterType = x.MasterType,
                    DocName = x.DocName,
                    DocNo = x.DocNo,
                    Remarks = x.Remarks,
                    IssueDate = x.IssueDate,
                    ExpDate = x.ExpDate,
                    DocImage = x.DocImage,
                    Status = x.Status,
                    DocStatusId = x.DocStatusId
                }).ToList().GroupBy(o => new
                {
                    StaffId = o.StaffId,
                }).Select(x => new HrDocumentMaster()
                {
                    DocumentId = x.FirstOrDefault().DocumentId,
                    DocumentTypeId = x.FirstOrDefault().DocumentTypeId,
                    StaffId = x.Key.StaffId,
                    CompanyId = x.FirstOrDefault().CompanyId,
                    VehicleId = x.FirstOrDefault().VehicleId,
                    OtherId = x.FirstOrDefault().OtherId,
                    MasterType = x.FirstOrDefault().MasterType,
                    DocName = x.FirstOrDefault().DocName,
                    DocNo = x.FirstOrDefault().DocNo,
                    Remarks = x.FirstOrDefault().Remarks,
                    IssueDate = x.FirstOrDefault().IssueDate,
                    ExpDate = x.FirstOrDefault().ExpDate,
                    DocImage = x.FirstOrDefault().DocImage,
                    Status = x.FirstOrDefault().Status,
                    DocStatusId = x.FirstOrDefault().DocStatusId
                }).ToList();
                return masterData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<HrDocumentMaster> GetAllVeicleDocumentMaster()
        {
            try
            {
                var masterData = repository.GetAll().Where(o => o.VehicleId != null).Select(x => new HrDocumentMaster()
                {
                    DocumentId = x.DocumentId,
                    DocumentTypeId = x.DocumentTypeId,
                    StaffId = x.StaffId,
                    CompanyId = x.CompanyId,
                    VehicleId = x.VehicleId,
                    OtherId = x.OtherId,
                    MasterType = x.MasterType,
                    DocName = x.DocName,
                    DocNo = x.DocNo,
                    Remarks = x.Remarks,
                    IssueDate = x.IssueDate,
                    ExpDate = x.ExpDate,
                    DocImage = x.DocImage,
                    Status = x.Status,
                    DocStatusId = x.DocStatusId
                }).ToList().GroupBy(o => new
                {
                    VehicleId = o.VehicleId,
                }).Select(x => new HrDocumentMaster()
                {
                    DocumentId = x.FirstOrDefault().DocumentId,
                    DocumentTypeId = x.FirstOrDefault().DocumentTypeId,
                    StaffId = x.FirstOrDefault().StaffId,
                    CompanyId = x.FirstOrDefault().CompanyId,
                    VehicleId = x.Key.VehicleId,
                    OtherId = x.FirstOrDefault().OtherId,
                    MasterType = x.FirstOrDefault().MasterType,
                    DocName = x.FirstOrDefault().DocName,
                    DocNo = x.FirstOrDefault().DocNo,
                    Remarks = x.FirstOrDefault().Remarks,
                    IssueDate = x.FirstOrDefault().IssueDate,
                    ExpDate = x.FirstOrDefault().ExpDate,
                    DocImage = x.FirstOrDefault().DocImage,
                    Status = x.FirstOrDefault().Status,
                    DocStatusId = x.FirstOrDefault().DocStatusId
                }).ToList();
                return masterData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<HrDocumentMaster> GetDocumentMasterByStaffID(long staffId)
        {
            try
            {
                List<HrDocumentMaster> dataList = repository.GetAll().Where(o => o.StaffId == staffId).ToList();
                return dataList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<HrDocumentMaster> GetDocumentMasterByVehicleID(long vehicleId)
        {
            try
            {
                List<HrDocumentMaster> dataList = repository.GetAll().Where(o => o.VehicleId == vehicleId).ToList();
                return dataList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable GetStaffDocumentReport(List<long?> staffIds, List<long?> documentTypeIds, long? documentStatudId)
        {
            try
            {
                IEnumerable<HrStaffMaster> hrStaffMasterList;
                IEnumerable<HrDocumentType> hrdocumentTypeList;
                Console.WriteLine("Step One");
                if (staffIds != null && staffIds.Count() > 0)
                {
                    hrStaffMasterList = _staffRepoService.GetAllHrStaffMaster().Where(o => staffIds.Contains(o.StaffId)).ToList();
                }
                else
                {
                    hrStaffMasterList = _staffRepoService.GetAllHrStaffMaster().ToList();
                }
                Console.WriteLine("Step Two");
                if (documentTypeIds != null && documentTypeIds.Count() > 0)
                {
                    hrdocumentTypeList = _documentTypeRepoService.GetAllHrDocumentType().Where(o => documentTypeIds.Contains(o.DocumentTypeId)).ToList();
                }
                else
                {
                    hrdocumentTypeList = _documentTypeRepoService.GetAllHrDocumentType().ToList();
                }
                Console.WriteLine("Step Three");
                var dataList = (from hdr in repository.GetAll().Where(o => documentStatudId == null ? true : documentStatudId == o.DocStatusId)
                                join type in hrdocumentTypeList on hdr.DocumentTypeId equals type.DocumentTypeId
                                join staff in hrStaffMasterList on hdr.StaffId equals staff.StaffId
                                select new
                                {
                                    DocumentId = hdr.DocumentId,
                                    DocName = hdr.DocName,
                                    DocNo = hdr.DocNo,
                                    IssueDate = hdr.IssueDate,
                                    ExpDate = hdr.ExpDate,
                                    StaffId = hdr.StaffId,
                                    StaffName = staff.FullName,
                                    StaffCode = staff.StaffCode,
                                    DocumentType = type.DocumentType,
                                    // expiredInDays = Convert.ToDateTime(hdr.ExpDate - hdr.IssueDate).Day.ToString(),
                                }).ToList().GroupBy(o => new
                                {
                                    StaffId = o.StaffId,
                                }).Select(o => new
                                {
                                    StaffId = o.Key.StaffId,
                                    StaffName = o.LastOrDefault().StaffName,
                                    StaffCode = o.LastOrDefault().StaffCode,
                                    dataList = o.Select(s => new
                                    {
                                        DocumentId = o.LastOrDefault().DocumentId,
                                        DocName = o.LastOrDefault().DocName,
                                        DocNo = o.LastOrDefault().DocNo,
                                        IssueDate = o.LastOrDefault().IssueDate,
                                        ExpDate = o.LastOrDefault().ExpDate,
                                        DocumentType = o.LastOrDefault().DocumentType,
                                        // expiredInDays = o.LastOrDefault().expiredInDays,
                                    }).ToList()
                                }).AsQueryable();
                Console.WriteLine("DATA LIST = " + dataList.Count());
                return dataList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable GetVehicleDocumentReport(List<long?> vehicleIds, List<long?> documentTypeIds, long? documentStatudId)
        {
            try
            {
                IEnumerable<HrVehicleMaster> hrVehicleMasterList;
                IEnumerable<HrDocumentType> hrdocumentTypeList;
                if (vehicleIds.Count() > 0)
                {
                    hrVehicleMasterList = _vehicleRepoService.GetAllHrVehicleMaster().Where(o => vehicleIds.Contains(o.Id)).ToList();
                }
                else
                {
                    hrVehicleMasterList = _vehicleRepoService.GetAllHrVehicleMaster().ToList();
                }

                if (documentTypeIds.Count() > 0)
                {
                    hrdocumentTypeList = _documentTypeRepoService.GetAllHrDocumentType().Where(o => documentTypeIds.Contains(o.DocumentTypeId)).ToList();
                }
                else
                {
                    hrdocumentTypeList = _documentTypeRepoService.GetAllHrDocumentType().ToList();
                }

                var dataList = (from hdr in repository.GetAll().Where(o => documentStatudId == null ? true : documentStatudId == o.DocStatusId)
                                join type in hrdocumentTypeList on hdr.DocumentTypeId equals type.DocumentTypeId
                                join vehcile in hrVehicleMasterList on hdr.VehicleId equals vehcile.Id
                                select new
                                {
                                    DocumentId = hdr.DocumentId,
                                    DocName = hdr.DocName,
                                    DocNo = hdr.DocNo,
                                    IssueDate = hdr.IssueDate,
                                    ExpDate = hdr.ExpDate,
                                    VehicleId = hdr.VehicleId,
                                    VehicleNo = vehcile.VehicleNo,
                                    VehicleTye = vehcile.VehicleTye,
                                    DocumentType = type.DocumentType,
                                    expiredInDays = Convert.ToDateTime(hdr.ExpDate - hdr.IssueDate).Day.ToString(),
                                }).ToList().GroupBy(o => new
                                {
                                    VehicleId = o.VehicleId,
                                }).Select(o => new
                                {
                                    VehicleId = o.Key.VehicleId,
                                    VehicleNo = o.LastOrDefault().VehicleNo,
                                    VehicleTye = o.LastOrDefault().VehicleTye,
                                    dataList = o.Select(s => new
                                    {
                                        DocumentId = o.LastOrDefault().DocumentId,
                                        DocName = o.LastOrDefault().DocName,
                                        DocNo = o.LastOrDefault().DocNo,
                                        IssueDate = o.LastOrDefault().IssueDate,
                                        ExpDate = o.LastOrDefault().ExpDate,
                                        VehicleId = o.LastOrDefault().VehicleId,
                                        DocumentType = o.LastOrDefault().DocumentType,
                                        expiredInDays = o.LastOrDefault().expiredInDays,
                                    }).ToList()
                                }).AsQueryable();
                Console.WriteLine("DATA LIST = " + dataList.Count());
                return dataList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable GetCompanyDocumentReport(List<long?> companyid, List<long?> documentTypeIds, long? documentStatudId)
        {
            try
            {
                IEnumerable<HrVehicleMaster> hrVehicleMasterList;
                IEnumerable<HrDocumentType> hrdocumentTypeList;
                if (companyid.Count() > 0)
                {
                    hrVehicleMasterList = _vehicleRepoService.GetAllHrVehicleMaster().Where(o => companyid.Contains(o.Id)).ToList();
                }
                else
                {
                    hrVehicleMasterList = _vehicleRepoService.GetAllHrVehicleMaster().ToList();
                }

                if (documentTypeIds.Count() > 0)
                {
                    hrdocumentTypeList = _documentTypeRepoService.GetAllHrDocumentType().Where(o => documentTypeIds.Contains(o.DocumentTypeId)).ToList();
                }
                else
                {
                    hrdocumentTypeList = _documentTypeRepoService.GetAllHrDocumentType().ToList();
                }

                var dataList = (from hdr in repository.GetAll().Where(o => documentStatudId == null ? true : documentStatudId == o.DocStatusId)
                                join type in hrdocumentTypeList on hdr.DocumentTypeId equals type.DocumentTypeId
                                join vehcile in hrVehicleMasterList on hdr.VehicleId equals vehcile.Id
                                select new
                                {
                                    DocumentId = hdr.DocumentId,
                                    DocName = hdr.DocName,
                                    DocNo = hdr.DocNo,
                                    IssueDate = hdr.IssueDate,
                                    ExpDate = hdr.ExpDate,
                                    VehicleId = hdr.VehicleId,
                                    VehicleNo = vehcile.VehicleNo,
                                    VehicleTye = vehcile.VehicleTye,
                                    DocumentType = type.DocumentType,
                                    expiredInDays = Convert.ToDateTime(hdr.ExpDate - hdr.IssueDate).Day.ToString(),
                                }).ToList().GroupBy(o => new
                                {
                                    VehicleId = o.VehicleId,
                                }).Select(o => new
                                {
                                    VehicleId = o.Key.VehicleId,
                                    VehicleNo = o.LastOrDefault().VehicleNo,
                                    VehicleTye = o.LastOrDefault().VehicleTye,
                                    dataList = o.Select(s => new
                                    {
                                        DocumentId = o.LastOrDefault().DocumentId,
                                        DocName = o.LastOrDefault().DocName,
                                        DocNo = o.LastOrDefault().DocNo,
                                        IssueDate = o.LastOrDefault().IssueDate,
                                        ExpDate = o.LastOrDefault().ExpDate,
                                        VehicleId = o.LastOrDefault().VehicleId,
                                        DocumentType = o.LastOrDefault().DocumentType,
                                        expiredInDays = o.LastOrDefault().expiredInDays,
                                    }).ToList()
                                }).AsQueryable();
                Console.WriteLine("DATA LIST = " + dataList.Count());
                return dataList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable GetDocumentTypeReport(HrDocumentTypeFilter docFilter)
        {
            try
            {
                //var result = from docMaster in _Context.HrDocumentMaster.AsNoTracking()
                //             join docType in _Context.HrDocumentType.AsNoTracking()
                //             on docMaster.DocumentTypeId equals docType.DocumentTypeId
                //             join companyMaster in _Context.HrCompanyMaster.AsNoTracking()
                //             on docMaster.CompanyId equals companyMaster.CompanyId into companyMasterGroup
                //             from company in companyMasterGroup.DefaultIfEmpty()
                //             where docMaster.MasterType == docFilter.MasterType
                //             && docMaster.DocStatusId == docFilter.statusId
                //             && (docMaster.IssueDate >= docFilter.fromdate && docMaster.IssueDate <= docFilter.todate)
                //             select new
                //             {
                //                 docMaster.DocumentId,
                //                 docMaster.DocumentTypeId,
                //                 Staffid = docMaster.StaffId ?? 0,
                //                 docMaster.CompanyId,
                //                 VehicleID = docMaster.VehicleId ?? 0,
                //                 OtherID = docMaster.OtherId ?? 0,
                //                 docMaster.MasterType,
                //                 docMaster.DocName,
                //                 docMaster.DocNo,
                //                 docMaster.Remarks,
                //                 docMaster.IssueDate,
                //                 docMaster.ExpDate,
                //                 docType.DocumentType,
                //                 docType.DefaultType,
                //                 Status = docMaster.DocStatusId ?? 0,
                //                 DocStatusID = docMaster.DocStatusId ?? 0
                //             };
                 return null;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }


    }
}
