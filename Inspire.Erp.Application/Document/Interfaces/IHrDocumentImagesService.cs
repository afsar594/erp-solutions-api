using Inspire.Erp.Domain.Entities.documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Document.Interfaces
{
    public interface IHrDocumentImagesService
    {
        public IEnumerable<HrDocumentImages> InsertHrDocumentImages(DTOHrDocumentImages _DTOHrDocumentImages);
        public IEnumerable<HrDocumentImages> UpdateHrDocumentImages(HrDocumentImages hrDocumentImages);
        public IEnumerable<HrDocumentImages> DeleteHrDocumentImages(HrDocumentImages hrDocumentImages);
        public IEnumerable<HrDocumentImages> GetAllHrDocumentImages();
        HrDocumentImages GetByID(long docImageId);
    }
}
