using Inspire.Erp.Domain.DTO;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Application.Complaint
{
    public class ComplaintImplementation : IComplaint
    {
        private readonly InspireErpDBContext context;
        private readonly IRepository<ComplaintMaster> repo;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ComplaintImplementation(InspireErpDBContext context, IRepository<ComplaintMaster> repo, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.repo = repo;
            this.webHostEnvironment = webHostEnvironment;
        }
        public void DeleteComplaint(int Id)
        {
            var dataToDelete = context.ComplaintMasters.FirstOrDefault(u => u.Id == Id);
            if (!string.IsNullOrEmpty(dataToDelete.PhotoPath))
            {
                var imagePath = dataToDelete.PhotoPath;
                if (System.IO.File.Exists(imagePath))
                {

                    System.IO.File.Delete(imagePath);
                }
                repo.Delete(dataToDelete);
                repo.SaveChangesAsync();

            }
        }

        public IEnumerable<ComplaintMaster> GetAllComplaints()
        {
            //var Complaints = context.ComplaintMasters.ToList();
            var Complaints = repo.GetAll();
            return Complaints;
        }

        public ComplaintMaster GetComplaintById(int Id)
        {
            var Complaint = context.ComplaintMasters.FirstOrDefault(u => u.Id == Id);

            return Complaint;
        }

        public bool InsertComplaint(ComplainMasterDto dto)
        {
            if (dto == null)
            {
                return false;
            }

            string photoPath = null;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(dto.Photo.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return false;
            }
            string wwwRootPath = webHostEnvironment.WebRootPath;

            string imagesFolderPath = Path.Combine(wwwRootPath, "Images");


            if (dto.Photo != null)
            {

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Photo.FileName);
                string fullPath = Path.Combine(imagesFolderPath, uniqueFileName);


                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    dto.Photo.CopyToAsync(stream);
                }


                photoPath = fullPath;
            }


            var dataToInsert = new ComplaintMaster()
            {
                BuildingId = dto.BuildingId,
                FlatId = dto.FlatId,
                CompanyId = dto.CompanyId,
                FullName = dto.FullName,

                ApartmentNo = dto.ApartmentNo,
                BuildingName = dto.BuildingName,
                PhoneNo = dto.PhoneNo,
                Email = dto.Email,

                Address = dto.Address,
                ComplaintType = dto.ComplaintType,
                Status = "UnChecked Complaint",
                Comments = dto.Comments,
                PhotoPath = photoPath,
                ComplaintDate = DateTime.UtcNow
            };
            repo.Insert(dataToInsert);
            repo.SaveChangesAsync();

            return true;
        }

        public bool UpdateComplaintStatus(int Id, string NewStatus)
        {
            var data = context.ComplaintMasters.FirstOrDefault(u => u.Id == Id);
            if (data == null)
            {
                return false;
            }

            data.Status = NewStatus;
            repo.Update(data);
            //context.ComplaintMasters.Update(data);
            repo.SaveChangesAsync();
            return true;
        }
    }
}
