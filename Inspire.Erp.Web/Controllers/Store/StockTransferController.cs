using AutoMapper;
using Inspire.Erp.Application.Store.Interface;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.MODULE;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.Store;
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
        private IStockTransferService _st;
        private readonly IMapper _mapper;
        public StockTransferController(IStockTransferService st, IMapper mapper)
        {
            _st = st;
            _mapper = mapper;
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
                List<StockItemResponse> list =await _st.getStockItemResponse();
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
        //[HttpGet("getStockTransferReport")]
        //public async Task<ApiResponse<List<StockTransferResponseModel>>> getStockTransferReport()
        //{
        //    try
        //    {
        //        ApiResponse<List<StockTransferResponseModel>> response = new ApiResponse<List<StockTransferResponseModel>>()
        //        {
        //            Result = _st.getStockTransferReport().ToList(),
        //            Message = "Ok"
        //        };
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}




        [HttpPost]
        [Route("InsertStockTransfer")]
        public async Task<bool> InsertStockTransfer([FromBody] StockTransferViewModel stockTranfermodel)
        {
            try
            {
                
                
                
                var param1 = _mapper.Map<StockTransfer>(stockTranfermodel);
                var param2 = _mapper.Map<List<StockTransferDetails>>(stockTranfermodel.StockTransferDetails);
                var  param3 = new List<AccountsTransactions>();
                List<StockRegister> param4 = new List<StockRegister>();
                clsAccountAndStock.StockTransfer_Accounts_STOCK_Transactions("", "", param1, param2, ref param4);
                await _st.InsertStockTransfer(param1,param2, param4);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        [Route("DeleteStockTransfer")]
        public async Task<bool> DeleteStockTransfer([FromBody] StockTransferViewModel stockTranfermodel)
        {
            try
            {
                var param1 = _mapper.Map<StockTransfer>(stockTranfermodel);
            var param2 = _mapper.Map<List<StockTransferDetails>>(stockTranfermodel.StockTransferDetails);

   

                //==============
               var  param3 = new List<AccountsTransactions>();
                List<StockRegister> param4 = new List<StockRegister>();

                var result= await _st.DeleteStockTransfer(param1, param2
               , param4
               );
            return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //[HttpPost("deleteStockTransfer")]
        //public async Task<bool> deleteStockTransfer(StockTransferRequestModel obj)
        //{
        //    try
        //    {
        //        await _st.deleteStockTransfer(obj);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //[HttpPost("UpdateStockTransfer")]
        //public async Task<bool> UpdateStockTransfer(StockTransferRequestModel obj)
        //{
        //    try
        //    {
        //        await _st.UpdateStockTransfer(obj);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}




        [HttpPost]
        [Route("UpdateStockTransfer")]
        public async Task<bool> UpdateStockTransfer([FromBody] StockTransferViewModel stockTranfermodel)
        {

            try
            {
                var param1 = _mapper.Map<StockTransfer>(stockTranfermodel);
            var param2 = _mapper.Map<List<StockTransferDetails>>(stockTranfermodel.StockTransferDetails);
           // var param3 = _mapper.Map<List<AccountsTransactions>>(stockTranfermodel.AccountsTransactions);
            //List<StockRegister> param4 = new List<StockRegister>();
            //var xs = _issueVoucherService.UpdateIssueVoucher(param1, param3, param2
            //    , param4
            //    );

            //==============
            //param3 = new List<AccountsTransactions>();
            List<StockRegister> param4 = new List<StockRegister>();
            clsAccountAndStock.StockTransfer_Accounts_STOCK_Transactions("", "", param1, param2, ref param4);

            await _st.UpdateStockTransfer(param1, param2, param4);
                //========================

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }
}