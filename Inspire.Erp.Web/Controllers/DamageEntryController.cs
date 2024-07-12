using AutoMapper;
using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Application.StoreWareHouse.Implementation;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
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

        private IDamageEntry DamageEntry;
        private readonly IMapper _mapper;
        public DamageEntryController(IDamageEntry _DamageEntry, IMapper mapper)
        {
            _mapper = mapper;
            DamageEntry = _DamageEntry;
  
        }
        [HttpPost]
        [Route("InsertDamageEntry")]       
        public ApiResponse<DamageMaster> InsertDamageEntry([FromBody] DamageMaster DamageViewModel)
        {

            var param1 = _mapper.Map<DamageMaster>(DamageViewModel);
            var InsertDamageViewModelViewModel = DamageEntry.InsertDamageEntry(param1);

            ApiResponse<DamageMaster> apiResponse = new ApiResponse<DamageMaster>
            {
                Valid = true,
                Result = _mapper.Map<DamageMaster>(InsertDamageViewModelViewModel),
                Message = DamageMessage.SaveDamage
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetDamageById")]
        public IActionResult GetJobDamageId(int id)
        {
            try
            {
                var item = DamageEntry.GetDamageById(id);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("GetDamageEntry")]
        public ApiResponse<List<DamageMaster>> GetDamageEntry()
        {
            ApiResponse<List<DamageMaster>> apiResponse = new ApiResponse<List<DamageMaster>>
            {
                Valid = true,
                Result = _mapper.Map<List<DamageMaster>>(DamageEntry.GetDamageEntry()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpDelete]
        [Route("DeleteDamageEntry")]
        public IActionResult DeleteDamageEntry(string Id)
        {
            try
            {
                return Ok(DamageEntry.DeleteDamageEntry(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        //[HttpPost("deleteDamageEntry")]
        //public async Task<ApiResponse<bool>> deleteDamageEntry(DamageMaster obj)
        //{
        //    try
        //    {
        //        de.deleteDamageEntry(obj);
        //        ApiResponse<bool> apiResponse = new ApiResponse<bool>
        //        {
        //            Valid = true,
        //            Result = true,
        //            Message = "true"
        //        };
        //        return apiResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //[HttpPost("UpdateDamageEntry")]
        //public async Task<ApiResponse<bool>> UpdateDamageEntry(DamageEntryResponse obj)
        //{
        //    try
        //    {
        //        de.updateDamageEntry(obj);
        //        ApiResponse<bool> apiResponse = new ApiResponse<bool>
        //        {
        //            Valid = true,
        //            Result = true,
        //            Message = "true"
        //        };
        //        return apiResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //[HttpGet("DamageEntryResponse")]
        //public ApiResponse<List<DamageEntryViewModal>> DamageEntryResponse()
        //{
        //    try
        //    {
        //        List<DamageEntryViewModal> list = de.DamageEntryResponse().Select(k => new DamageEntryViewModal
        //        {
        //            Damage_Details_Amount = k.DamageDetailsAmount,
        //            Item_Master_Item_ID = k.ItemMasterItemId,
        //            Item_Master_Item_Name = k.ItemMasterItemName,
        //            Damage_Details_Price = k.DamageDetailsPrice,
        //            Damage_Details_QTY = k.DamageDetailsQTY,
        //            Damage_Details_Remarks = k.DamageDetailsRemarks,
        //            Damage_Details_Unit_ID = k.DamageDetailsUnitID,
        //            Damage_Details_Material_ID = k.DamageDetailsMaterialID,
        //            Damage_Master_ID = k.DamageMasterID,
        //            Damage_Master_Location_ID = k.DamageMasterLocationID,
        //            Damage_Master_Narration = k.DamageMasterNarration,
        //            Damage_Master_Vdate = k.DamageMasterVdat,
        //            Location_Master_Location_Name = k.LocationMasterLocationName,
        //            Damage_Details_Voucher_Number = k.DamageDetailsVoucherNumber
        //        }).ToList();
        //        ApiResponse<List<DamageEntryViewModal>> apiResponse = new ApiResponse<List<DamageEntryViewModal>>
        //        {
        //            Valid = true,
        //            Result = list,
        //            Message = ""
        //        };
        //        return apiResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //[HttpGet("DamageEntryPrefix")]
        //public async Task<ApiResponse<VouchersNumbers>> DamageEntryPrefix()
        //{
        //    try
        //    {
        //        var prefix =  de.DamageEntryPrefix();
        //        ApiResponse<VouchersNumbers> apiResponse = new ApiResponse<VouchersNumbers>
        //        {
        //            Valid = true,
        //            Result = prefix,
        //            Message = ""
        //        };
        //        return apiResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}