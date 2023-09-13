using Inspire.Erp.Application.Dashboard.Interfaces;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Dashboard;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Dashboard.Implementations
{
    public class DashboardService:IDashboardService
    {
        private readonly IRepository<GetSalesResponse> _salesResponse;
        private readonly IRepository<GetSummaryResponse> _summaryService;
        private readonly IRepository<GetDashboardResponse> _dashboardService;
        private readonly IRepository<GetPDCResponse> _PDCService;
        private readonly IRepository<GetIncomeExpenseResponse> _incomeExpService;
        private readonly IRepository<GetCustomerSupplierResponse> _customerSupplierService;
        public DashboardService(IRepository<GetDashboardResponse> dashboardService, IRepository<GetSummaryResponse> summaryService,
            IRepository<GetSalesResponse> salesResponse, IRepository<GetCustomerSupplierResponse> customerSupplierService,
             IRepository<GetPDCResponse> PDCService, IRepository<GetIncomeExpenseResponse> incomeExpService)
        {
            _dashboardService = dashboardService;
            _salesResponse = salesResponse;
            _summaryService = summaryService;
            _customerSupplierService = customerSupplierService;
            _PDCService = PDCService;
            _incomeExpService = incomeExpService;
        }

        public async Task<Response<List<GetSummaryResponse>>> GetSummaryChart(DashboardFilterModel model)
        {
            try
            {
                Nullable<DateTime> startDate = null;
                Nullable<DateTime> endDate = null;
                if (model.StartDate != null)
                {
                    startDate = Convert.ToDateTime(model.StartDate);
                }
                if (model.EndDate != null)
                {
                    endDate = Convert.ToDateTime(model.EndDate);
                }
                var result = await _summaryService.GetBySPWithParameters<GetSummaryResponse>(@$" exec Sp_GetDashboardData {model.Type},{startDate},{endDate}",x=>new GetSummaryResponse { 
                Description=x.Description,
                Value=x.Value,
                });
                return Response<List<GetSummaryResponse>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetSummaryResponse>>.Fail(new List<GetSummaryResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<GetCustomerSupplierResponse>>> GetCustomerSupplier(DashboardFilterModel model)
        {
            try
            {
                List<GetCustomerSupplierResponse> resultModel = new List<GetCustomerSupplierResponse>();
                Nullable<DateTime> startDate = null;
                Nullable<DateTime> endDate = null;
                if (model.StartDate != null)
                {
                    startDate = Convert.ToDateTime(model.StartDate);
                }
                if (model.EndDate != null)
                {
                    endDate = Convert.ToDateTime(model.EndDate);
                }
                    resultModel = await _customerSupplierService.GetBySPWithParameters<GetCustomerSupplierResponse>(@$" exec Sp_GetDashboardData {model.Type},{startDate},{endDate}", x => new GetCustomerSupplierResponse
                    {
                        Amount = x.Amount,
                        MA_AccName = x.MA_AccName,
                        MA_AccNo = x.MA_AccNo
                    });

                return Response<List<GetCustomerSupplierResponse>>.Success(resultModel, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetCustomerSupplierResponse>>.Fail(new List<GetCustomerSupplierResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<GetSalesResponse>>> GetDasboardPurchaseGroupAccount(DashboardFilterModel model)
        {
            try
            {
                Nullable<DateTime> startDate = null;
                Nullable<DateTime> endDate = null;
                if (model.StartDate != null)
                {
                    startDate = Convert.ToDateTime(model.StartDate);
                }
                if (model.EndDate != null)
                {
                    endDate = Convert.ToDateTime(model.EndDate);
                }
                var result = await _salesResponse.GetBySPWithParameters<GetSalesResponse>(@$" exec Sp_GetDashboardData {model.Type},{startDate},{endDate}", x => new GetSalesResponse
                {
                    MonthYear = x.MonthYear,
                    NetAmount = x.NetAmount,
                });
                return Response<List<GetSalesResponse>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetSalesResponse>>.Fail(new List<GetSalesResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<GetPDCResponse>>> GetDashboardPDCIssueRecieved(DashboardFilterModel model)
        {
            try
            {
                Nullable<DateTime> startDate = null;
                Nullable<DateTime> endDate = null;
                if (model.StartDate != null)
                {
                    startDate = Convert.ToDateTime(model.StartDate);
                }
                if (model.EndDate != null)
                {
                    endDate = Convert.ToDateTime(model.EndDate);
                }
                var result = await _PDCService.GetBySPWithParameters<GetPDCResponse>(@$" exec Sp_GetDashboardData {model.Type},{startDate},{endDate}", x => new GetPDCResponse
                {
                    AccName = x.AccName,
                    CAmount = x.CAmount,
                    BAccNo=x.BAccNo,
                    CBName=x.CBName,
                    CNO=x.CNO,
                    PartyName=x.PartyName,
                    PDate=x.PDate,
                    PID=x.PID,
                    TDate=x.TDate,
                    VNO=x.VNO,
                    VType=x.VType
                });
                return Response<List<GetPDCResponse>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetPDCResponse>>.Fail(new List<GetPDCResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<GetIncomeExpenseResponse>>> GetDasboardIncomeExpense(DashboardFilterModel model)
        {
            try
            {
                Nullable<DateTime> startDate = null;
                Nullable<DateTime> endDate = null;
                if (model.StartDate != null)
                {
                    startDate = Convert.ToDateTime(model.StartDate);
                }
                if (model.EndDate != null)
                {
                    endDate = Convert.ToDateTime(model.EndDate);
                }
                var result = await _incomeExpService.GetBySPWithParameters<GetIncomeExpenseResponse>(@$" exec Sp_GetDashboardData {model.Type},{startDate},{endDate}", x => new GetIncomeExpenseResponse
                {
                    Profit = x.Profit,
                    Income = x.Income,
                    Expenses=x.Expenses,
                    TransDate=x.TransDate
                });
                return Response<List<GetIncomeExpenseResponse>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetIncomeExpenseResponse>>.Fail(new List<GetIncomeExpenseResponse>(), ex.Message);
            }
        }
    }
}
