using AutoMapper;
using Inspire.Erp.Application.Authentication.Inerfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.Procurement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Document.Interfaces;

namespace Inspire.Erp.Web.Controllers.Document
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class HrDocumentImagesController : ControllerBase
    {
        private IMapper mapper;
        private IHrDocumentImagesService service;
        public HrDocumentImagesController(IHrDocumentImagesService hrDocumentImagesService, IMapper mapper)
        {
            this.service = hrDocumentImagesService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllHrDocumentImages")]
        public List<HrDocumentImagesViewModel> GetAllDocumentImages()
        {
            try
            {
                return service.GetAllHrDocumentImages().Select(x => new HrDocumentImagesViewModel()
                {
                    DocImage = x.DocImage,
                    DocImageId = x.DocImageId,
                    DocumentId = x.DocumentId,
                    Extension = x.Extension,
                   // DocName = x.DocName,
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentImagesViewModel>();
            }
        }

        [HttpGet]
        [Route("GetByID")]
        public IActionResult GetByID(long docImageId)
        {
            try
            {
                var item = service.GetByID(docImageId);
                // byte[] bytes = System.IO.File.ReadAllBytes(item.DocImage.ToString());
                return File(item.DocImage, item.Extension, item.DocName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("InsertHrDocumentImage")]
        public List<HrDocumentImagesViewModel> InserHrDocumentImage([FromBody] DTOHrDocumentImages _DTOHrDocumentImages)
        {
            try
            {

                return service.InsertHrDocumentImages(_DTOHrDocumentImages).Select(x => new HrDocumentImagesViewModel()
                {
                    DocImage = x.DocImage,
                    DocImageId = x.DocImageId,
                    DocumentId = x.DocumentId,
                    Extension = x.Extension
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        [HttpPost]
        [Route("UpdateHrDocumentImage")]
        public List<HrDocumentImagesViewModel> UpdateHrDocumentImage([FromBody] HrDocumentImagesViewModel hrDocumentImagesViewModel)
        {
            try
            {
                var docImage = mapper.Map<HrDocumentImages>(hrDocumentImagesViewModel);
                return service.UpdateHrDocumentImages(docImage).Select(x => new HrDocumentImagesViewModel()
                {
                    DocImage = x.DocImage,
                    DocImageId = x.DocImageId,
                    DocumentId = x.DocumentId,
                    Extension = x.Extension
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentImagesViewModel>();
            }
        }

        [HttpPost]
        [Route("DeleteHrDocumentImage")]
        public List<HrDocumentImagesViewModel> DeleteHrDocumentImage([FromBody] HrDocumentImagesViewModel hrDocumentImagesViewModel)
        {
            try
            {
                var docImage = mapper.Map<HrDocumentImages>(hrDocumentImagesViewModel);
                return service.DeleteHrDocumentImages(docImage).Select(x => new HrDocumentImagesViewModel()
                {
                    DocImage = x.DocImage,
                    DocImageId = x.DocImageId,
                    DocumentId = x.DocumentId,
                    Extension = x.Extension
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentImagesViewModel>();
            }
        }

    }
}