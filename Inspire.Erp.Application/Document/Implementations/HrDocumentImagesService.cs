using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrDocumentImagesService : IHrDocumentImagesService
    {
        private IRepository<HrDocumentImages> repository;
        public HrDocumentImagesService(IRepository<HrDocumentImages> repository)
        {
            this.repository = repository;
        }

        public IEnumerable<HrDocumentImages> DeleteHrDocumentImages(HrDocumentImages hrDocumentImages)
        {
            try
            {
                repository.Delete(hrDocumentImages);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentImages> GetAllHrDocumentImages()
        {
            try
            {
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HrDocumentImages GetByID(long docImageId)
        {
            try
            {
                var data = repository.GetAll().Where(o => o.DocImageId == docImageId).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentImages> InsertHrDocumentImages(DTOHrDocumentImages _DTOHrDocumentImages)
        {
            try
            {
                _DTOHrDocumentImages.DocImage = _DTOHrDocumentImages.DocImage.Substring(_DTOHrDocumentImages.DocImage.IndexOf(",") + 1);
                var objfiles = new HrDocumentImages()
                {
                    DocumentId = _DTOHrDocumentImages.DocumentId,
                    DocImage = _DTOHrDocumentImages.DocImage,
                    Extension = _DTOHrDocumentImages.Extension,
                    DocImageId = 0,
                    DocName = _DTOHrDocumentImages.DocName,
                };
                repository.Insert(objfiles);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<HrDocumentImages> UpdateHrDocumentImages(HrDocumentImages hrDocumentImages)
        {
            try
            {
                repository.Update(hrDocumentImages);
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
