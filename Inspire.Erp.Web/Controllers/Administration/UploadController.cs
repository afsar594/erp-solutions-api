using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.ViewModels.Administration;
using Inspire.Erp.Application.Administration.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inspire.Erp.Domain.Modals.Common;
using System.ComponentModel.Design;
using System.IO;

namespace Inspire.Erp.Web.Controllers.Administration
{
    [Route("api/Upload")]
    [Produces("application/json")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UploadController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [ActionName("Temporary")]
        public ResponseInfo Temporary()
        {
            var objResponse = new ResponseInfo();
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.Request.HasFormContentType && httpContext.Request.Form.Files.Any())
            {
                List<string> FileNamesList = new List<string>();

                // Get the uploaded image from the Files collection
                var httpPostedFiles = httpContext.Request.Form.Files;

                string fileSavePath, fileName;
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Temp");
                var companyFolderPath = Path.Combine(folderPath);

                // Check if the folder exists, if not create it
                if (!Directory.Exists(companyFolderPath))
                {
                    Directory.CreateDirectory(companyFolderPath);
                }

                foreach (var file in httpPostedFiles)
                {
                    //HttpPostedFile UFD = httpPostedFiles[i] as HttpPostedFile;

                    if (file != null)
                    {
                        string fileExtension = Path.GetExtension(file.FileName).ToLower();
                        var baseFileName = Path.GetFileNameWithoutExtension(file.FileName);
                        if (baseFileName.Length > 50)
                        {
                            // Truncate the base file name to fit the max length
                            baseFileName = baseFileName.Substring(0, 50);
                        }
                        var timestamp = DateTime.Now.Ticks.ToString();
                        fileName = $"{baseFileName}_{timestamp}{fileExtension}";
                        string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".pdf" };
                        const int maxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB in bytes
                        if (allowedExtensions.Contains(fileExtension))
                        {
                            if (file.Length <= maxFileSizeInBytes)
                            {
                                FileNamesList.Add(fileName);
                                fileSavePath = Path.Combine(companyFolderPath, fileName);
                                using (var stream = new FileStream(fileSavePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                            }
                            else
                            {
                                objResponse.IsSuccess = false;
                                objResponse.ErrorMessage = ($"File {file.FileName} exceeds the 10 MB size limit.");
                            }
                        }
                        else
                        {
                            objResponse.IsSuccess = false;
                            objResponse.ErrorMessage = ($"File {file.FileName} has an invalid extension.");
                        }
                    }
                }

                objResponse.IsSuccess = true;
                objResponse.ResultSet = FileNamesList.ToArray();
            }
            else
            {
                objResponse.IsSuccess = false;
                objResponse.ErrorMessage = "No file selected";
            }
            return objResponse;
        }

    }
}