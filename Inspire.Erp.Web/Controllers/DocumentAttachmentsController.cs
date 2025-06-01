using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Application.Master;
using Microsoft.EntityFrameworkCore.Storage;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Application.Common;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentAttachmentsController : ControllerBase
    {
        private IDocsAttachmentService docsAttachmentService;
        public DocumentAttachmentsController(IDocsAttachmentService _docsAttachmentService)
        {
            docsAttachmentService = _docsAttachmentService;
        }
        [HttpPost]
        [Route("readpdffile")]
        public async Task<IActionResult> PostPdfFile(int Id, string type)
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
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload/Pdfs/", fileName);


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                DocsAttachment Documents = new DocsAttachment();
                Documents.DocumentName = "/Upload/Pdfs/" + fileName;

                Documents.DocumentDelStatus = false;
                Documents.DocumentVocucherId = Id;
                Documents.DocumentType = type;
                docsAttachmentService.SaveDocument(Documents);
            }
            return Ok(true);
        }

        [HttpPost("import-selected-pr-documents-to-po")]
        public async Task<IActionResult> ImportSelectedPRDocumentsToPO([FromBody] ImportSelectedFileDto request)
        {
            return Ok( await docsAttachmentService.CopyAttachmentsFromPRToPO(request.PrId, request.PoId, request.PrFileIds));
        }



        [HttpGet]
        [Route("GetDocumentstById")]
        public IActionResult GetDocumentstById(int DocumentId, int DocumentTypeId, String Type)
        {
            try
            {
                var item = docsAttachmentService.GetDocumentstById(DocumentId, DocumentTypeId, Type);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("GetAllDocumentById")]
        public IActionResult GetAllDocumentById(int id, string type)
        {
            try
            {
                var item = docsAttachmentService.GetAllDocumentById(id, type);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("DeleteDocument")]
        public IActionResult DeleteDocument(int Id)
        {
            try
            {
                return Ok(docsAttachmentService.DeleteDocument(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet("GetPdfById/{id}")]
        public async Task<IActionResult> GetPdfById(int id)
        {
            var fileData = await docsAttachmentService.GetPdfById(id);

            if (fileData == null)
            {
                return NotFound("File not found.");
            }

            return File(fileData.FileBytes, fileData.ContentType, fileData.FileName);
        }


        [HttpGet]
        [Route("GetDocuments")]
        public async Task<IActionResult> GetDocuments(int id, string type)
        {
            return Ok(await docsAttachmentService.GetJobDocuments(id, type));
        }

        [HttpGet("DownloadAllFilesAsOnePdf/{id}/{type}")]
        public async Task<IActionResult> DownloadAllFilesAsOnePdf(int id, string type)
        {
            var result = await docsAttachmentService.DownloadAllFilesAsOnePdf(id, type);

            if (result.FileBytes == null)
                return null;
            
            return File(result.FileBytes, result.ContentType, "downloadedFile.pdf");
        }

    }

}
