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

namespace Inspire.Erp.Web.Controllers.Administration
{
    [Route("api/master")]
    [Produces("application/json")]
    [ApiController]
    public class StationMasterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IStationMasterServices stationMasterService;
        public StationMasterController(IStationMasterServices _stationMasterService, IMapper mapper)
        {

            stationMasterService = _stationMasterService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAllStation")]
        public List<StationMasterViewModel> GetAllStation()
        {

            return stationMasterService.GetAllStation().Select(k => new StationMasterViewModel
            {
                StationMasterCode = k.StationMasterCode,
                StationMasterStationName = k.StationMasterStationName,
                StationMasterAddress = k.StationMasterAddress,
                StationMasterCity = k.StationMasterCity,
                StationMasterPostOffice = k.StationMasterPostOffice,
                StationMasterTele1 = k.StationMasterTele1,
                StationMasterTele2 = k.StationMasterTele2,
                StationMasterFax = k.StationMasterFax,
                StationMasterEmail = k.StationMasterEmail,
                StationMasterWebSite = k.StationMasterWebSite,
                StationMasterCountry = k.StationMasterCountry,
                StationMasterLogoPath = k.StationMasterLogoPath,
                StationMasterSignPath = k.StationMasterSignPath,
                StationMasterLogoImg = k.StationMasterLogoImg,
                StationMasterSignImg = k.StationMasterSignImg,
                StationMasterSealImg = k.StationMasterSealImg,
                StationMasterVatNo = k.StationMasterVatNo,
                StationMasterDelStatus = k.StationMasterDelStatus

            }).ToList();
        }
    }
}