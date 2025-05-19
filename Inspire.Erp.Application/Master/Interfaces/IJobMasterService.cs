using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.File;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master
{
    public interface IJobMasterService
    {
        public JobMaster InsertJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails , List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees,List<JobStaff> jobStaff, List<JobEquipment> jobEquipment, List<JobExcutionDetails> jobExcutionDetails, List<JobTimesheetDesc> JobTimesheetDesc, List<JobTimeSheetEquip> JobTimeSheetEquip,List<JobTimesheetMaster> JobTimesheetMaster,List<JobTimeSheetStaff> JobTimeSheetStaff, List<JobfilesStatus> JobfilesStatus, List<JobMasterControl> JobMasterControl);
        //public JobMaster UpdateJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails , List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees);
        public JobMaster UpdateJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees, List<JobStaff> jobStaff, List<JobEquipment> jobEquipment, List<JobExcutionDetails> jobExcutionDetails, List<JobTimesheetDesc> JobTimesheetDesc, List<JobTimeSheetEquip> JobTimeSheetEquip, List<JobTimesheetMaster> JobTimesheetMaster, List<JobTimeSheetStaff> JobTimeSheetStaff, List<JobfilesStatus> JobfilesStatus,List<JobMasterControl> JobMasterControl);

        public JobMaster GetSavedJobMasterDetails(string jbno);

        public IEnumerable<JobMaster> DeleteJob(JobMaster jobMaster);
        public IEnumerable<JobMaster> GetAllJob();
        public IEnumerable<JobMaster> GetAllJobById(int id);

        public IEnumerable<JobDocuments> GetJobDocumentstById(int jobDocumentJobId, int jobDocumentId);


        Task<FileResultDto> GetJobPdfById(int id);
        Task<FileResultDto> GetJobAttachedFiles(int jobId);


        public Task<string>  GetJobHTML(int id);

        public void SaveJobDocument(JobDocuments jobDocumnet);

        public IEnumerable<JobDocuments> GetAllJobDocumentById(int id);
        public IEnumerable<JobDocuments> DeleteJobDocument(int id);
        public Task<Response<string>> GetUserLevelForJob(int userId, int joId);
        public Task<Response<List<JobDocumentsDto>>> GetJobDocuments(int joId);
    }
}

