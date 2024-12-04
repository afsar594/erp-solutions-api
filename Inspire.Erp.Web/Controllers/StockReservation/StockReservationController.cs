using AutoMapper;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.ViewModels.sales;
using Inspire.Erp.Web.ViewModels;

namespace Inspire.Erp.Web.Controllers
{

    [ApiController]
    [Route("api/StockReservation")]
    public class StockReservationController : ControllerBase
    {
        private IStockResversation _Stock;
        IRepository<StockReservation> _stockReservationrepository;
        IRepository<StockReservationDetails> _stockReservationDetailsrepository;

        private IMapper _mapper;
        public StockReservationController(IStockResversation Stock, IMapper mapper,IRepository<StockReservation> stockReservationrepository, IRepository<StockReservationDetails> stockReservationDetailsrepository)
        {
            _Stock = Stock;
            _mapper = mapper;
            _stockReservationrepository = stockReservationrepository;
            _stockReservationDetailsrepository = stockReservationDetailsrepository;
        }
        [HttpPost]
        [Route("InsertStockReservation")]
        public async Task<ApiResponse<StockReservationViewModel>> InsertStockReservation([FromBody] StockReservationViewModel stockReservationViewModel)
        {

            var param1 = _mapper.Map<StockReservation>(stockReservationViewModel);
            var param2 = _mapper.Map<List<StockReservationDetails>>(stockReservationViewModel.StockReservationDetails);
            var xs = await _Stock.InsertStockReservation(param1,param2);
            StockReservationViewModel StockReservation = new StockReservationViewModel
            {
                StockReservationId = (int)xs.StockReservation.StockReservationId,
                StockReservationRDate = xs.StockReservation.StockReservationRDate,
                StockReservationRelease = xs.StockReservation.StockReservationRelease,
                StockReservationDelStatus = xs.StockReservation.StockReservationDelStatus,   
                StockReservationRemarks = xs.StockReservation.StockReservationRemarks

            };
            ApiResponse<StockReservationViewModel> apiResponse = new ApiResponse<StockReservationViewModel>
            {
                Valid = true,
                Result = _mapper.Map<StockReservationViewModel>(StockReservation),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("DeleteStockReservation")]
        public IActionResult DeleteStockReservation(int Id)
        {
            try
            {
                return Ok(_Stock.DeleteStockReservation(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetStockReservationById")]
        public IActionResult GetStockReservationById(int id)
        {
            try
            {
                var item = _Stock.GetStockReservationById(id);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("UpdateStockReservation")]
        public async Task<ApiResponse<StockReservationViewModel>> UpdateStockReservation([FromBody] StockReservationViewModel stockReservationViewModel)
        {
            var param1 = _mapper.Map<StockReservation>(stockReservationViewModel);
            var param2 = _mapper.Map<List<StockReservationDetails>>(stockReservationViewModel.StockReservationDetails);
            var xs = await _Stock.UpdateStockReservation(param1, param2);

            StockReservationViewModel StockReservation = new StockReservationViewModel
            {
                StockReservationId = (int)xs.StockReservation.StockReservationId,
                StockReservationRDate = xs.StockReservation.StockReservationRDate,
                StockReservationRelease = xs.StockReservation.StockReservationRelease,
                StockReservationDelStatus = xs.StockReservation.StockReservationDelStatus,
                StockReservationRemarks = xs.StockReservation.StockReservationRemarks

            };
            ApiResponse<StockReservationViewModel> apiResponse = new ApiResponse<StockReservationViewModel>
            {
                Valid = true,
                Result = _mapper.Map<StockReservationViewModel>(StockReservation),
                Message = ""
            };
            return apiResponse;            
        }
    }
}
