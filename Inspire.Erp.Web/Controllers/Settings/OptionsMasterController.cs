using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using Inspire.Erp.Web.ViewModels.Settings;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Settings
{
    [Route("api/[controller]")]
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
        [Route("options")]
        public IActionResult GetOptionSetting(string search)
        {
            return Ok(_optionsSvc.GetOptionsSettings(search));
        }


        [HttpGet]
        [Route("getAllOPtionSettings")]
        public async Task< IActionResult> getAllOPtionSettings()
        {
            return Ok(await _optionsSvc.GetAllOptionSettings());
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetOptionsSettingbyKey/{key}")]
        public ApiResponse<List<OptionsMasterViewModel>> GetOptionsSettingbyKey(string key)
        {
            List<OptionsMaster> listoptionsSettings = _optionsSvc.GetOptionsSettingsbyKey(key).ToList();
            if (listoptionsSettings != null)
            {
                var x = _mapper.Map<List<OptionsMasterViewModel>>(listoptionsSettings);

                ApiResponse<List<OptionsMasterViewModel>> apiResponse = new ApiResponse<List<OptionsMasterViewModel>>
                {
                    Valid = true,
                    Result = x,
                    Message = ""
                };

                return apiResponse;
            }
            return null;

        }

        [HttpPost]
        [Route("updateOptions")]
        public ApiResponse<List<OptionsMaster>> UpdateOptionsSetting([FromBody] List<OptionsMaster> optionsSettings)
        {
            bool result = _optionsSvc.updateOptionsSettings(optionsSettings);
            ApiResponse<List<OptionsMaster>> apiResponse = new ApiResponse<List<OptionsMaster>>
            {
                Valid = result,
                Result = _optionsSvc.GetOptionsSettings("").ToList(),
                Message = ""
            };
            return apiResponse;
        }



        [HttpPost]
        [Route("ImportOptionSettingsFromFile")]
        public IActionResult ImportOptionSettingsFromFile()
        {
            ReadingFiles<OptionsMaster> readingFiles = new ReadingFiles<OptionsMaster>();
            var options = readingFiles.ReadFile("OptionsSettings.json");
            bool result = _optionsSvc.ImportOptionsSettingsFromFile(options);
            return Ok(result);
        }
    }
}

