using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Application.Settings.Interface;
using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Settings;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/settings")]
    [Produces("application/json")]
    [ApiController]
    public class OptionsMasterController : ControllerBase

    {
        private readonly IMapper _mapper;
        private IOptionsSettings _optionsSvc;
        public OptionsMasterController(IOptionsSettings optionsSvc, IMapper mapper)
        {
            _optionsSvc = optionsSvc;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetOptionSetting")]
        public async Task <IActionResult> GetOptionSetting(string search)
        {

            return Ok(await _optionsSvc.GetOptionsSettings(search));
            //var model = new List<OptionsMasterViewModel>();
            //var listoptionsSettings = await _optionsSvc.GetOptionsSettings(search);
            //if (listoptionsSettings != null)
            //{
            //    model = listoptionsSettings.Result.Select(x => new OptionsMasterViewModel
            //    {
            //        OptionsMasterDelStatus = x.OptionsMasterDelStatus,
            //        OptionsMasterDescription = x.OptionsMasterDescription,
            //        OptionsMasterFormName = x.OptionsMasterFormName,
            //        OptionsMasterFullDescription = x.OptionsMasterFullDescription,
            //        OptionsMasterId = x.OptionsMasterId,
            //        OptionsMasterTypeBoolean = x.OptionsMasterType == "Yes",
            //        OptionsMasterType= x.OptionsMasterType,
            //    }).ToList();

            //}
            //return Ok(model);
        }

        [HttpGet]
        [Route("GetOptionsSettingbyKey/{key}")]
        public async Task<IActionResult> GetOptionsSettingbyKey(string key)
        {
            return  Ok(await _optionsSvc.GetOptionsSettingsbyKey(key));
            //if (listoptionsSettings != null)
            //{

            //    //var x = _mapper.Map<List<OptionsMasterViewModel>>(listoptionsSettings);
            //    var x = listoptionsSettings.Select(x => new OptionsMasterViewModel
            //    {
            //        OptionsMasterDelStatus= x.OptionsMasterDelStatus,
            //        OptionsMasterDescription= x.OptionsMasterDescription,
            //        OptionsMasterFormName= x.OptionsMasterFormName,
            //        OptionsMasterFullDescription    = x.OptionsMasterFullDescription,
            //        OptionsMasterId = x.OptionsMasterId
            //        ,OptionsMasterType = x.OptionsMasterType,
            //        OptionsMasterTypeBoolean = x.OptionsMasterType == "Yes"?true:false,
            //    }).ToList();

            //    ApiResponse<List<OptionsMasterViewModel>> apiResponse = new ApiResponse<List<OptionsMasterViewModel>>
            //    {
            //        Valid = true,
            //        Result = x,
            //        Message = ""
            //    };

            //    return apiResponse;
            //}
            //return null;

        }

        [HttpPost]
        [Route("UpdateOptionsSetting")]
        public ApiResponse<bool> UpdateOptionsSetting([FromBody] List<OptionsMasterViewModel> optionsSettings)
        {
            var result = false;
            if (optionsSettings != null && optionsSettings.Count >0)
            {
              var options=  optionsSettings.Select(x => new OptionsMaster
                {
                  OptionsMasterDelStatus=x.OptionsMasterDelStatus,
                  OptionsMasterDescription=x.OptionsMasterDescription,
                  OptionsMasterFormName=x.OptionsMasterFormName,
                  OptionsMasterFullDescription=x.OptionsMasterFullDescription,
                  OptionsMasterId=x.OptionsMasterId,
                  OptionsMasterType=x.OptionsMasterType == "Yes" || x.OptionsMasterType=="No"?x.OptionsMasterTypeBoolean?"Yes":"No":x.OptionsMasterType,
                  OptionsMasterValueType = x.OptionsMasterValueType
              }).ToList();
                 result = _optionsSvc.updateOptionsSettings(options);
            }
            ApiResponse<bool> apiResponse = new ApiResponse<bool>
            {
                Valid = result,
                Result = result,
                Message = ""
            };
            return apiResponse;
        }
    }
}