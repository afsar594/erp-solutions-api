using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ICityMasterService cityMasterService;
        private ICountryMasterService _countryService;
        public CityController(ICityMasterService _countryMasterService, IMapper mapper, ICountryMasterService countryService)
        {

            cityMasterService = _countryMasterService;
            _mapper = mapper;
            _countryService = countryService;
        }
        [HttpGet]
        [Route("GetAllCity")]
        public List<CityMasterViewModel> GetAllCity()
        {
            var countries = _countryService.GetAllCountry().ToList();
            return cityMasterService.GetAllCity().Select(k => new CityMasterViewModel
            {

                CityMasterCityId = k.CityMasterCityId,
                CityMasterCityCountryId = k.CityMasterCityCountryId,
                CityMasterCityName = k.CityMasterCityName,
                CityMasterCityDeleted = k.CityMasterCityDeleted,
                CityMasterCityStatus = k.CityMasterCityStatus,
                CityMasterCityDelStatus= k.CityMasterCityDelStatus,
                CountryName = countries.FirstOrDefault(c => c.CountryMasterCountryId == k.CityMasterCityCountryId)?.CountryMasterCountryName
            }).OrderBy(x => x.CityMasterCityName).AsEnumerable().ToList();
        }


        [HttpGet]
        [Route("GetAllCityById/{id}")]
        public List<CityMasterViewModel> GetAllCityById(int id)
        {
            var countries = _countryService.GetAllCountry().ToList();
            return cityMasterService.GetAllCityById(id).Select(k => new CityMasterViewModel
            {

                CityMasterCityId = k.CityMasterCityId,
                CityMasterCityCountryId = k.CityMasterCityCountryId,
                CityMasterCityName = k.CityMasterCityName,
                CityMasterCityDeleted = k.CityMasterCityDeleted,
                CityMasterCityStatus = k.CityMasterCityStatus,
                CityMasterCityDelStatus = k.CityMasterCityDelStatus,
                CountryName = countries.FirstOrDefault(c => c.CountryMasterCountryId == k.CityMasterCityCountryId)?.CountryMasterCountryName

            }).OrderBy(x => x.CityMasterCityName).AsEnumerable().ToList();
        }

        [HttpPost]
        [Route("InsertCity")]
        public List<CityMasterViewModel> InsertCity([FromBody] CityMasterViewModel cityMaster)
        {
            
            var result = _mapper.Map<CityMaster>(cityMaster);
            var countries = _countryService.GetAllCountry().ToList();
            return cityMasterService.InsertCity(result).Select(k => new CityMasterViewModel
            {
                CityMasterCityId = k.CityMasterCityId,
                CityMasterCityCountryId = k.CityMasterCityCountryId,
                CityMasterCityName = k.CityMasterCityName,
                CityMasterCityDeleted = k.CityMasterCityDeleted,
                CityMasterCityStatus = k.CityMasterCityStatus,
                CityMasterCityDelStatus = k.CityMasterCityDelStatus,
                CountryName = countries.FirstOrDefault(c => c.CountryMasterCountryId == k.CityMasterCityCountryId)?.CountryMasterCountryName


            }).OrderBy(x => x.CityMasterCityName).AsEnumerable().ToList();
        }

        [HttpPost]
        [Route("UpdateCity")]
        public List<CityMasterViewModel> UpdateCity([FromBody] CityMasterViewModel cityMaster)
        {
            var result = _mapper.Map<CityMaster>(cityMaster);
            var countries = _countryService.GetAllCountry().ToList();
            return cityMasterService.UpdateCity(result).Select(k => new CityMasterViewModel
            {
                CityMasterCityId = k.CityMasterCityId,
                CityMasterCityCountryId = k.CityMasterCityCountryId,
                CityMasterCityName = k.CityMasterCityName,
                CityMasterCityDeleted = k.CityMasterCityDeleted,
                CityMasterCityStatus = k.CityMasterCityStatus,
                CityMasterCityDelStatus = k.CityMasterCityDelStatus,
                CountryName = countries.FirstOrDefault(c => c.CountryMasterCountryId == k.CityMasterCityCountryId)?.CountryMasterCountryName

            }).OrderBy(x => x.CityMasterCityName).AsEnumerable().ToList();
        }

        [HttpPost]
        [Route("DeleteCity")]
        public List<CityMasterViewModel> DeleteCity([FromBody] CityMasterViewModel cityMaster)
        {
            var result = _mapper.Map<CityMaster>(cityMaster);
            var countries = _countryService.GetAllCountry().ToList();
            return cityMasterService.DeleteCity(result).Select(k => new CityMasterViewModel
            {
                CityMasterCityId = k.CityMasterCityId,
                CityMasterCityCountryId = k.CityMasterCityCountryId,
                CityMasterCityName = k.CityMasterCityName,
                CityMasterCityDeleted = k.CityMasterCityDeleted,
                CityMasterCityStatus = k.CityMasterCityStatus,
                CityMasterCityDelStatus = k.CityMasterCityDelStatus,
                CountryName = countries.FirstOrDefault(c => c.CountryMasterCountryId == k.CityMasterCityCountryId)?.CountryMasterCountryName
            }).OrderBy(x => x.CityMasterCityName).AsEnumerable().ToList();
        }
    }
}