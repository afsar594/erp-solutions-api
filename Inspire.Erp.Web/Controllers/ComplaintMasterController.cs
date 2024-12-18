using Inspire.Erp.Application.Complaint;
using Inspire.Erp.Domain.DTO;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintMasterController : ControllerBase
    {
        private readonly InspireErpDBContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IComplaint complaintSerivce;

        public ComplaintMasterController(InspireErpDBContext context, IWebHostEnvironment webHostEnvironment, IComplaint complaintSerivce)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
            this.complaintSerivce = complaintSerivce;
        }
        [HttpGet]
        [Route("GetAllComplaints")]
        public ActionResult GetAllComplaints()
        {
            try
            {

                var Complaints = complaintSerivce.GetAllComplaints();

                return Ok(Complaints);
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = $"An Exception occured {ex.Message}" });
            }

        }
        [HttpGet]
        [Route("GetComplaintById")]
        public ActionResult GetComplaintById(int Id)
        {

            try
            {
                var Complaint = complaintSerivce.GetComplaintById(Id);

                return Ok(Complaint);
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = $"An Exception occured {ex.Message}" });
            }
        }
        [HttpPost]
        [Route("InsertComplaint")]
        public ActionResult InsertComplaint([FromForm] ComplainMasterDto dto)
        {
            try
            {
                bool IsInserted = complaintSerivce.InsertComplaint(dto);
                if (!IsInserted)
                {
                    return BadRequest(new { Message = "Data not Inserted.Something Went Wrong!" });
                }
                return Ok(new { Message = "Complaint Sent Successfully!" });
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = $"An Exception occured {ex.Message}" });
            }

        }
        [HttpPut]
        [Route("UpdateComplaintStatus")]
        public ActionResult UpdateComplaintStatus(int Id, string NewStatus)
        {
            var isUpdated = complaintSerivce.UpdateComplaintStatus(Id, NewStatus);
            if (!isUpdated)
            {
                return BadRequest(new { Message = "An Exception occured" });
            }
            return Ok(new { Message = "Update Status Successfully." });
        }
        [HttpDelete]
        [Route("DeleteComplaint")]
        public ActionResult DeleteComplaint(int Id)
        {

            try
            {
                // complainService.DeleteComplaint(Id)

                return Ok(new { message = "Complaint deleted Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An Exception occured {ex.Message}" });

            }
        }


    }
}
