using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IJobMasterService jobMasterService;
        public JobController(IJobMasterService _jobMasterService, IMapper mapper)
        {
            jobMasterService = _jobMasterService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetJobMaster")]                       
        public ApiResponse<List<JobMaster>> GetAllJob()
        {
            ApiResponse<List<JobMaster>> apiResponse = new ApiResponse<List<JobMaster>>
            {
                Valid = true,
                Result = _mapper.Map<List<JobMaster>>(jobMasterService.GetAllJob()),
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

           var respone = jobMasterService.InsertJobMaster(param1, param2, param3,param4);

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

            ApiResponse<JobMaster> apiResponse = new ApiResponse<JobMaster>();
            var param1 = _mapper.Map<JobMaster>(jobMaster);
            var param2 = _mapper.Map<List<JobMasterBudgetDetails>>(jobMaster.JobMasterBudgetDetails);
            var param3 = _mapper.Map<List<JobMasterJobDetails>>(jobMaster.JobMasterJobDetails);
            var param4 = _mapper.Map<List<JobMasterJobWiseBankGuarantees>>(jobMaster.JobMasterJobWiseBankGuarantees);

            var xs = jobMasterService.UpdateJobMaster(param1, param2, param3, param4);

            apiResponse = new ApiResponse<JobMaster>
            {
                Valid = true,
                Result = xs,
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


            }).ToList();
        }

    }
}
