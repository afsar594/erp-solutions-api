using AutoMapper;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DamageEntryController : ControllerBase
    {

        private IDamageEntry de;
        private readonly IMapper _mapper;
        public DamageEntryController(IDamageEntry _de, IMapper mapper)
        {
            _mapper = mapper;
            de = _de;
        }
        [HttpPost("submitDamageEntry")]
        public async Task<ApiResponse<DemageRequest>> submitDamageEntry(DemageRequest obj)
        {
            try
            {
                var response = await de.submitDamageEntry(obj);
                ApiResponse<DemageRequest> apiResponse = new ApiResponse<DemageRequest>
                {
                    Valid = true,
                    Result = response ? obj : null,
                    Message = "true"
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("deleteDamageEntry")]
        public async Task<ApiResponse<bool>> deleteDamageEntry(DamageMaster obj)
        {
            try
            {
                de.deleteDamageEntry(obj);
                ApiResponse<bool> apiResponse = new ApiResponse<bool>
                {
                    Valid = true,
                    Result = true,
                    Message = "true"
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("UpdateDamageEntry")]
        public async Task<ApiResponse<bool>> UpdateDamageEntry(DamageEntryResponse obj)
        {
            try
            {
                de.updateDamageEntry(obj);
                ApiResponse<bool> apiResponse = new ApiResponse<bool>
                {
                    Valid = true,
                    Result = true,
                    Message = "true"
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("DamageEntryResponse")]
        public ApiResponse<List<DamageEntryViewModal>> DamageEntryResponse()
        {
            try
            {
                List<DamageEntryViewModal> list = de.DamageEntryResponse().Select(k => new DamageEntryViewModal
                {
                    Damage_Details_Amount = k.DamageDetailsAmount,
                    Item_Master_Item_ID = k.ItemMasterItemId,
                    Item_Master_Item_Name = k.ItemMasterItemName,
                    Damage_Details_Price = k.DamageDetailsPrice,
                    Damage_Details_QTY = k.DamageDetailsQTY,
                    Damage_Details_Remarks = k.DamageDetailsRemarks,
                    Damage_Details_Unit_ID = k.DamageDetailsUnitID,
                    Damage_Details_Material_ID = k.DamageDetailsMaterialID,
                    Damage_Master_ID = k.DamageMasterID,
                    Damage_Master_Location_ID = k.DamageMasterLocationID,
                    Damage_Master_Narration = k.DamageMasterNarration,
                    Damage_Master_Vdate = k.DamageMasterVdat,
                    Location_Master_Location_Name = k.LocationMasterLocationName,
                    Damage_Details_Voucher_Number = k.DamageDetailsVoucherNumber
                }).ToList();
                ApiResponse<List<DamageEntryViewModal>> apiResponse = new ApiResponse<List<DamageEntryViewModal>>
                {
                    Valid = true,
                    Result = list,
                    Message = ""
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("DamageEntryPrefix")]
        public async Task<ApiResponse<string>> DamageEntryPrefix()
        {
            try
            {
                string prefix = await de.DamageEntryPrefix();
                ApiResponse<string> apiResponse = new ApiResponse<string>
                {
                    Valid = true,
                    Result = prefix,
                    Message = ""
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}