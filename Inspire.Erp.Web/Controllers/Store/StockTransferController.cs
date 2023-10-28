using Inspire.Erp.Application.Store.Interface;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Web.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Store
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransferController : ControllerBase
    {
        private IStockTransfer _st;
        public StockTransferController(IStockTransfer st)
        {
            _st = st;
        }
        /// <summary>
        /// Need to replace SP getStockItemsList by LINQ see from the vb.code and load it according to the reposnse 
        /// </summary>
        /// <returns></returns>
        [HttpGet("getStockItemsList")]
        public async Task<ApiResponse<List<StockItemResponse>>> getStockItemsList()
        {
            try
            {
                List<StockItemResponse> list = _st.getStockItemResponse().ToList();
                ApiResponse<List<StockItemResponse>> response = new ApiResponse<List<StockItemResponse>>()
                {
                    Result = list,
                    Message = "Ok"
                };
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///  Need to replace SP getStockTransferReport by LINQ see from the vb.code and load it according to the reposnse  
        /// </summary>
        /// <returns></returns>
        [HttpGet("getStockTransferReport")]
        public async Task<ApiResponse<List<StockTransferResponseModel>>> getStockTransferReport()
        {
            try
            {
                ApiResponse<List<StockTransferResponseModel>> response = new ApiResponse<List<StockTransferResponseModel>>()
                {
                    Result = _st.getStockTransferReport().ToList(),
                    Message = "Ok"
                };
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("submitTransferReport")]
        public async Task<bool> submitTransferReport(List<StockTransferRequestModel> obj)
        {
            try
            {
                await _st.submitTransferReport(obj);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("deleteStockTransfer")]
        public async Task<bool> deleteStockTransfer(StockTransferRequestModel obj)
        {
            try
            {
                await _st.deleteStockTransfer(obj);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("UpdateStockTransfer")]
        public async Task<bool> UpdateStockTransfer(StockTransferRequestModel obj)
        {
            try
            {
                await _st.UpdateStockTransfer(obj);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}