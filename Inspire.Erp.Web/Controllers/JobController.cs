using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Account;
using Inspire.Erp.Application.Account.Implementation;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.File;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IJobMasterService jobMasterService;
        private IFileService fileService;
        private IRepository<CurrencyMaster> currencyrepository;
        private IRepository<CountryMaster> countryrepository;
        private IChartofAccountsService chartofAccountsService;
        private IRepository<SalesManMaster> salesmanrepository;
        private IRepository<CustomerMaster> _customerMasterRepository;
        private IRepository<ProjectDescriptionMaster> ProjectDescriptionrepository;
        private IRepository<BankGuaranteeMaster> bankGuaranteerepository;
        private IRepository<CostCenterMaster> costcanterrepository;
        private readonly IRepository<JobMaster> _jobmasterrepository;
        public JobController(IJobMasterService _jobMasterService, IMapper mapper,
             IRepository<CurrencyMaster> _currencyrepository,
             IRepository<ProjectDescriptionMaster> _ProjectDescriptionrepository
            , IChartofAccountsService _chartofAccountsService,
             IRepository<BankGuaranteeMaster> _bankGuaranteerepository,
             IRepository<SalesManMaster> _salesmanrepository,
             IRepository<CustomerMaster> customerMasterRepository,
             IRepository<CostCenterMaster> _costcentermasterrepository,
            IFileService _fileService, IRepository<JobMaster> jobmasterrepository
            )
        {
            jobMasterService = _jobMasterService;
            _mapper = mapper;
            ProjectDescriptionrepository = _ProjectDescriptionrepository;
            chartofAccountsService = _chartofAccountsService; salesmanrepository = _salesmanrepository;
            currencyrepository = _currencyrepository; _customerMasterRepository = customerMasterRepository;
            bankGuaranteerepository = _bankGuaranteerepository;
            fileService = _fileService; _jobmasterrepository = jobmasterrepository;
            costcanterrepository = _costcentermasterrepository;
        }
        [HttpGet]
        [Route("GetJobMaster")]
        //public ApiResponse<List<JobMaster>> GetAllJob()
        //{
        //    ApiResponse<List<JobMaster>> apiResponse = new ApiResponse<List<JobMaster>>
        //    {
        //        Valid = true,
        //        Result = _mapper.Map<List<JobMaster>>(jobMasterService.GetAllJob()),
        //        Message = ""
        //    };
        //    return apiResponse;
        //}

        public ApiResponse<List<JobMaster>> GetAllJob()
        {
            var jobs = jobMasterService.GetAllJob()
                .OrderBy(job => job.JobMasterJobName) // Assuming 'Name' is the property you want to order by
                .OrderBy(x => x.JobMasterJobName).ToList();

            ApiResponse<List<JobMaster>> apiResponse = new ApiResponse<List<JobMaster>>
            {
                Valid = true,
                Result = _mapper.Map<List<JobMaster>>(jobs),
                Message = ""
            };
            return apiResponse;
        }


        [HttpGet]
        [Route("GetAllMasterById/{id}")]
        public ApiResponse<JobMaster> GetAllJobById(string id)
        {
            JobMaster jobMaster = jobMasterService.GetSavedJobMasterDetails(id);
            if (jobMaster != null)
            {
                ApiResponse<JobMaster> apiResponse = new ApiResponse<JobMaster>
                {
                    Valid = true,
                    Result = jobMaster,
                    Message = ""
                };
                return apiResponse;
            }
            return null;
        }
        [HttpPost]
        [Route("InsertJobMaster")]
        public ApiResponse<JobMaster> InsertJob([FromBody] JobMasterViewModel jobMaster)
        {
            ApiResponse<JobMaster> apiResponse = new ApiResponse<JobMaster>();
            var param1 = _mapper.Map<JobMaster>(jobMaster);
            var param2 = _mapper.Map<List<JobMasterBudgetDetails>>(jobMaster.JobMasterBudgetDetails);
            var param3 = _mapper.Map<List<JobMasterJobDetails>>(jobMaster.JobMasterJobDetails);
            var param4 = _mapper.Map<List<JobMasterJobWiseBankGuarantees>>(jobMaster.JobMasterJobWiseBankGuarantees);
            var param5 = _mapper.Map<List<JobStaff>>(jobMaster.JobStaff);
            var param6 = _mapper.Map<List<JobEquipment>>(jobMaster.JobEquipment);
            var param7 = _mapper.Map<List<JobExcutionDetails>>(jobMaster.JobExcutionDetails);
            var param8 = _mapper.Map<List<JobTimesheetDesc>>(jobMaster.JobTimesheetDesc);
            var param9 = _mapper.Map<List<JobTimeSheetEquip>>(jobMaster.JobTimeSheetEquip);
            var param10 = _mapper.Map<List<JobTimesheetMaster>>(jobMaster.JobTimesheetMaster);
            var param11 = _mapper.Map<List<JobTimeSheetStaff>>(jobMaster.JobTimeSheetStaff);
            var param12 = _mapper.Map<List<JobfilesStatus>>(jobMaster.JobfilesStatus);
            var respone = jobMasterService.InsertJobMaster(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);

            apiResponse = new ApiResponse<JobMaster>
            {
                Valid = true,
                Result = respone,
                Message = "Inserted"
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdateJobMaster")]
        public ApiResponse<JobMaster> UpdateJob([FromBody] JobMasterViewModel jobMaster)
        {

            //ApiResponse<JobMaster> apiResponse = new ApiResponse<JobMaster>();
            //var param1 = _mapper.Map<JobMaster>(jobMaster);
            //var param2 = _mapper.Map<List<JobMasterBudgetDetails>>(jobMaster.JobMasterBudgetDetails);
            //var param3 = _mapper.Map<List<JobMasterJobDetails>>(jobMaster.JobMasterJobDetails);
            //var param4 = _mapper.Map<List<JobMasterJobWiseBankGuarantees>>(jobMaster.JobMasterJobWiseBankGuarantees);

            //var xs = jobMasterService.UpdateJobMaster(param1, param2, param3, param4);
            ApiResponse<JobMaster> apiResponse = new ApiResponse<JobMaster>();
            var param1 = _mapper.Map<JobMaster>(jobMaster);
            var param2 = _mapper.Map<List<JobMasterBudgetDetails>>(jobMaster.JobMasterBudgetDetails);
            var param3 = _mapper.Map<List<JobMasterJobDetails>>(jobMaster.JobMasterJobDetails);
            var param4 = _mapper.Map<List<JobMasterJobWiseBankGuarantees>>(jobMaster.JobMasterJobWiseBankGuarantees);
            var param5 = _mapper.Map<List<JobStaff>>(jobMaster.JobStaff);
            var param6 = _mapper.Map<List<JobEquipment>>(jobMaster.JobEquipment);
            var param7 = _mapper.Map<List<JobExcutionDetails>>(jobMaster.JobExcutionDetails);
            var param8 = _mapper.Map<List<JobTimesheetDesc>>(jobMaster.JobTimesheetDesc);
            var param9 = _mapper.Map<List<JobTimeSheetEquip>>(jobMaster.JobTimeSheetEquip);
            var param10 = _mapper.Map<List<JobTimesheetMaster>>(jobMaster.JobTimesheetMaster);
            var param11 = _mapper.Map<List<JobTimeSheetStaff>>(jobMaster.JobTimeSheetStaff);
            var param12 = _mapper.Map<List<JobfilesStatus>>(jobMaster.JobfilesStatus);
            var respone = jobMasterService.UpdateJobMaster(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);

            apiResponse = new ApiResponse<JobMaster>
            {
                Valid = true,
                Result = respone,
                Message = "Updated"
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("DeleteJob")]
        public List<JobMasterViewModel> DeleteJob([FromBody] JobMasterViewModel jobMaster)
        {
            var result = _mapper.Map<JobMaster>(jobMaster);
            return jobMasterService.DeleteJob(result).Select(k => new JobMasterViewModel
            {
                JobMasterJobId = k.JobMasterJobId,
                JobMasterJobName = k.JobMasterJobName,
                JobMasterJobDate = k.JobMasterJobDate,
                //JobMasterJobValue = k.JobMasterJobValue,
                //JobMasterJobRetention = k.JobMasterJobRetention,
                //JobMasterJobBalance = k.JobMasterJobBalance,
                //JobMasterJobStatus = k.JobMasterJobStatus,
                //JobMasterJobConsultant = k.JobMasterJobConsultant,
                //JobMasterJobRemarks = k.JobMasterJobRemarks,
                //JobMasterJobCustomerId = k.JobMasterJobCustomerId,
                //JobMasterJobCurrencyId = k.JobMasterJobCurrencyId,
                //JobMasterJobCurVal = k.JobMasterJobCurVal,
                //JobMasterJobOpenInv = k.JobMasterJobOpenInv,
                //JobMasterJobPerInv = k.JobMasterJobPerInv,
                //JobMasterJobConsultantReff = k.JobMasterJobConsultantReff,
                //JobMasterJobStatusVal = k.JobMasterJobStatusVal,
                //JobMasterJobType = k.JobMasterJobType,
                //JobMasterJobRelativeNo = k.JobMasterJobRelativeNo,
                //JobMasterJobCode = k.JobMasterJobCode,
                //JobMasterJobSupplierId = k.JobMasterJobSupplierId,
                //JobMasterJobSalesManId = k.JobMasterJobSalesManId,
                //JobMasterJobNumber = k.JobMasterJobNumber,
                //JobMasterJobIsSubJob = k.JobMasterJobIsSubJob,
                //JobMasterJobRetentionPercentage = k.JobMasterJobRetentionPercentage,
                //JobMasterJobSalesMan = k.JobMasterJobSalesMan,
                //JobMasterJobBudgetMaterialCost = k.JobMasterJobBudgetMaterialCost,
                //JobMasterJobMaterialCost = k.JobMasterJobMaterialCost,
                //JobMasterJobMaterialCostVar = k.JobMasterJobMaterialCostVar,
                //JobMasterJobBugetLabourCost = k.JobMasterJobBugetLabourCost,
                //JobMasterJobLabourCost = k.JobMasterJobLabourCost,
                //JobMasterJobLabourCostVar = k.JobMasterJobLabourCostVar,
                //JobMasterJobBudgetOverHeadCost = k.JobMasterJobBudgetOverHeadCost,
                //JobMasterJobOverHeadCost = k.JobMasterJobOverHeadCost,
                //JobMasterJobOverHeadCostVar = k.JobMasterJobOverHeadCostVar,
                //JobMasterJobMiscCost = k.JobMasterJobMiscCost,
                //JobMasterJobTotalCost = k.JobMasterJobTotalCost,
                //JobMasterJobProfit = k.JobMasterJobProfit,
                //JobMasterJobTotalInvValue = k.JobMasterJobTotalInvValue,
                //JobMasterJobTotalPayValue = k.JobMasterJobTotalPayValue,
                //JobMasterJobTotalRecValue = k.JobMasterJobTotalRecValue,
                //JobMasterJobTotalExpValue = k.JobMasterJobTotalExpValue,
                //JobMasterJobContractorName = k.JobMasterJobContractorName,
                //JobMasterJobContractType = k.JobMasterJobContractType,
                //JobMasterJobCommenceDate = k.JobMasterJobCommenceDate,
                //JobMasterJobWorkingDays = k.JobMasterJobWorkingDays,
                //JobMasterJobOrgCompleteDate = k.JobMasterJobOrgCompleteDate,
                //JobMasterJobPayTermDays = k.JobMasterJobPayTermDays,
                //JobMasterJobAdvanceAmount = k.JobMasterJobAdvanceAmount,
                //JobMasterJobDelStatus = k.JobMasterJobDelStatus


            }).OrderBy(x => x.JobMasterJobName).ToList();
        }


        [HttpGet]
        [Route("GetJobCost/{id}")]
        public async Task<IActionResult> GeneratePdf(int id)
        {

            var html = await jobMasterService.GetJobHTML(id);
            var pdfRe = new AddPDFResponse();
            pdfRe.Html = html;
            var pdfBytes = await fileService.GeneratePdf(pdfRe);

            return File(pdfBytes.Result, "application/pdf", "generated.pdf");
        }
        [HttpPost]
        [Route("readpdffile/{Id}")]
        public async Task<IActionResult> PostPdfFile(int Id)
        {
            List<string> uploadedFiles = new List<string>();
            var files = Request.Form.Files;
            
            if (files.Count == 0)
                return Content("File Not Selected");

            foreach (var file in files)
            {
                Random rn = new Random();
                int count = rn.Next(1111, 9999);
                var fileName = count + file.FileName;

                //string fileExtension = Path.GetExtension(file.FileName);
                //var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Pdfs");

                //if (!Directory.Exists(pathBuilt))
                //{
                //    Directory.CreateDirectory(pathBuilt);
                //}

                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload/Pdfs/",
                   fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                JobDocuments jobDocuments = new JobDocuments();
                jobDocuments.JobDocumentName = "/Upload/Pdfs/" + fileName;

                jobDocuments.JobDocumentDelStatus = false;
                jobDocuments.JobDocumentJobId = Id;
               // jobDocuments.JobDocumentId = 1;
                jobMasterService.SaveJobDocument(jobDocuments);
            }


            return Ok(true);
        }

        [HttpGet]
        [Route("jobLoadDropdown")]
        public ResponseInfo jobLoadDropdown()
        {
            var objresponse = new ResponseInfo();

            var currencyMasters = currencyrepository.GetAsQueryable().Where(a => a.CurrencyMasterCurrencyDelStatus != true).Select(c => new
            {
                c.CurrencyMasterCurrencyId,
                c.CurrencyMasterCurrencyName,
                c.CurrencyMasterCurrencyRate
            }).OrderBy(x => x.CurrencyMasterCurrencyName).ToList();
            var masterAccountsTables = chartofAccountsService.GetAllAccounts().Where(a => a.MaDelStatus != true).Select(c => new
            {
                c.MaAccNo,
                c.MaAccName,
                c.MaRelativeNo,
                c.MaSno
            }).OrderBy(x => x.MaAccName).AsEnumerable().ToList();
            var customerMasters = _customerMasterRepository.GetAsQueryable().Where(k => k.CustomerMasterCustomerDelStatus != true).Select(a => new
            {
                a.CustomerMasterCustomerNo,
                a.CustomerMasterCustomerName,
                a.CustomerMasterCustomerVatNo,
                a.CustomerMasterCustomerContactPerson,
                a.CustomerMasterCustomerAddress,
                a.CustomerMasterCustomerReffAcNo
            }).OrderBy(x => x.CustomerMasterCustomerName).ToList();
            var SaleManList = salesmanrepository.GetAsQueryable().Where(a => a.SalesManMasterSalesManDelStatus != true).Select(c => new
            {
                c.SalesManMasterSalesManId,
                c.SalesManMasterSalesManName,
            }).OrderBy(x => x.SalesManMasterSalesManName).ToList();
            var projectDescriptionMaster = ProjectDescriptionrepository.GetAsQueryable().Where(a => a.ProjectDescriptionMasterProjectDescriptionDelStatus != true).Select(c => new
            {
                c.ProjectDescriptionMasterProjectDescriptionId,
                c.ProjectDescriptionMasterProjectDescriptionName
            }).OrderBy(x => x.ProjectDescriptionMasterProjectDescriptionName).ToList();

            var bankGuaranteeMasters = bankGuaranteerepository.GetAsQueryable().Where(a => a.BankGuaranteeMasterDeleted != true).Select(c => new
            {
                c.BankGuaranteeMasterBgid,
                BankGuaranteeMasterBgname = c.BankGuaranteeMasterBgname.Trim()
            }).OrderBy(x => x.BankGuaranteeMasterBgname).ToList();

            var costCenterMaster = costcanterrepository.GetAsQueryable().Where(a => a.CostCenterMasterCostCenterDelStatus != true).Select(c => new
            {
                c.CostCenterMasterCostCenterId,
                CostCenterMasterCostCenterName = c.CostCenterMasterCostCenterName.Trim()
            }).OrderBy(x => x.CostCenterMasterCostCenterName).ToList();
            var jobmaster = _jobmasterrepository.GetAsQueryable().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                c.JobMasterJobName
            }).OrderBy(x => x.JobMasterJobName).ToList();
            objresponse.ResultSet = new
            {
                currencyMasters = currencyMasters,
                masterAccountsTables = masterAccountsTables,
                SaleManList = SaleManList,
                customerMasters = customerMasters,
                bankGuaranteeMasters = bankGuaranteeMasters,
                projectDescriptionMaster = projectDescriptionMaster,
                costCenterMaster = costCenterMaster,
                jobmaster = jobmaster

            };
            return objresponse;
        }
        [HttpGet]
        [Route("GetAllJobDocumentById")]
        public IActionResult GetAllJobDocumentById(int id)
        {
            try
            {
                var item = jobMasterService.GetAllJobDocumentById(id);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("GetJobDocumentstById")]
        public IActionResult GetJobDocumentstById(int jobDocumentJobId,int jobDocumentId)
        {
            try
            {
                var item = jobMasterService.GetJobDocumentstById(jobDocumentJobId, jobDocumentId);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        

        [HttpGet]
        [Route("DeleteJobDocument")]
        public IActionResult DeleteJobDocument(int Id)
        {
            try
            {
                return Ok(jobMasterService.DeleteJobDocument(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
    }
}
