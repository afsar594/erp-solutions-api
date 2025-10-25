using AutoMapper;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class OtherChargesMasterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IOtherChargesMasterService _service;
        public OtherChargesMasterController(IMapper mapper, IOtherChargesMasterService service)
        {
            _mapper = mapper;
            _service = service;
        }
        [HttpGet]
        [Route("GetAllOtherChargesMaster")]
        public List<OtherChargesMasterViewModel> GetAllOtherChargesMaster()
        {

            return _service.GetAllOtherChargesMaster().Select(k => new OtherChargesMasterViewModel
            {
                ServiceID = k.ServiceID,
                ServiceName = k.ServiceName,
                AccountNo = k.AccountNo,
                VatPer = k.VatPer,
                ServiceType = k.ServiceType
            }).OrderBy(o => o.ServiceName).ToList();
        }
        [HttpGet]
        [Route("GetAllOtherChargesMasterById/{id}")]
        public List<OtherChargesMasterViewModel> GetAllOtherChargesMasterById(int id)
        {
            return _service.GetAllOtherChargesMasterById(id).Select(k => new OtherChargesMasterViewModel
            {
                ServiceID = k.ServiceID,
                ServiceName = k.ServiceName,
                AccountNo = k.AccountNo,
                VatPer = k.VatPer,
                ServiceType = k.ServiceType
            }).OrderBy(x => x.ServiceName).ToList();
        }
        [HttpPost]
        [Route("InsertOtherChargesMaster")]
        public List<OtherChargesMasterViewModel> InsertSalesman([FromBody] OtherChargesMasterViewModel vm)
        {
            var result = _mapper.Map<OtherChargesMaster>(vm);
            return _service.InsertOtherChargesMaster(result).Select(k => new OtherChargesMasterViewModel
            {
                ServiceID = k.ServiceID,
                ServiceName = k.ServiceName,
                AccountNo = k.AccountNo,
                VatPer = k.VatPer,
                ServiceType = k.ServiceType
            }).OrderBy(x => x.ServiceName).AsEnumerable().ToList();
        }
        [HttpPost]
        [Route("UpdateOtherChargesMaster")]
        public List<OtherChargesMasterViewModel> UpdateSalesman([FromBody] OtherChargesMasterViewModel vm)
        {
            var result = _mapper.Map<OtherChargesMaster>(vm);
            return _service.UpdateOtherChargesMaster(result).Select(k => new OtherChargesMasterViewModel
            {
                ServiceID = k.ServiceID,
                ServiceName = k.ServiceName,
                AccountNo = k.AccountNo,
                VatPer = k.VatPer,
                ServiceType = k.ServiceType
            }).OrderBy(x => x.ServiceName).ToList();
        }
        [HttpPost]
        [Route("DeleteOtherChargesMaster")]
        public List<OtherChargesMasterViewModel> DeleteSalesman([FromBody] OtherChargesMasterViewModel vm)
        {
            var result = _mapper.Map<OtherChargesMaster>(vm);
            return _service.DeleteOtherChargesMaster(result).Select(k => new OtherChargesMasterViewModel
            {
                ServiceID = k.ServiceID,
                ServiceName = k.ServiceName,
                AccountNo = k.AccountNo,
                VatPer = k.VatPer,
                ServiceType = k.ServiceType
            }).OrderBy(x => x.ServiceName).ToList();
        }
    }

}
