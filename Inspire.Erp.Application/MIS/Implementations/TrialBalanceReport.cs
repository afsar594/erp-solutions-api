using Inspire.Erp.Application.MIS.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.MIS.TrialBalance;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Models.MIS;

namespace Inspire.Erp.Application.MIS.Implementations
{
    public class TrialBalanceReport : ITrialBalanceReport
    {

        private readonly IRepository<SingleValueSPResult> _spResult;
        private readonly IRepository<Inspire.Erp.Domain.Entities.TrialBalance> _trialBalance;

        public TrialBalanceReport(IRepository<Domain.Entities.TrialBalance> trialBalance, IRepository<SingleValueSPResult> spResult)
        {
            _trialBalance = trialBalance;
            _spResult = spResult;
        }

        private async Task<int> ExecuteSP(ReportFilter query)
        {
            var spResult = await _spResult.GetBySPWithParameters<SingleValueSPResult>(@$"exec GetAccountTransactionTrialBalance  {query.fromDate},{query.toDate},{query.HideZeroTransactions},{query.CostCenter},{query.FinancialMindate},{query.Formate}", x => new SingleValueSPResult()
            {
                ReturnValue = x.ReturnValue
            });
            return (int)spResult.FirstOrDefault().ReturnValue;
        }
        public async Task<Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>> TrailBalanceSummary(ReportFilter query)
        {
            GridWrapperResponse<List<GetTrialBalanceReportModel>> aa = new GridWrapperResponse<List<GetTrialBalanceReportModel>>();
            List<GetTrialBalanceReportModel> bb = new List<GetTrialBalanceReportModel>();

            try
            {


                await ExecuteSP(query);

                var res = from tb in _trialBalance.GetAll()
                          group tb by new { tb.TrialBalanceMainHead, tb.TrialBalanceSubHead } into g
                          select new
                          {
                              MainHead = g.Key.TrialBalanceMainHead,
                              SubHead = g.Key.TrialBalanceSubHead,
                              OpenBalance = g.Sum(tb => tb.TrialBalanceOpenBalance),
                              TotalDebit = g.Sum(tb => tb.TrialBalanceTotalDebit),
                              TotalCredit = g.Sum(tb => tb.TrialBalanceTotalCredit)
                          }
                           into summary
                          orderby summary.MainHead
                          select summary;

                // Execute the query and get the result
                var result = res.ToList();


                var mainHeads = result.Select(x => x.MainHead).ToList().Distinct();
                if (mainHeads.Count() > 0)
                {
                    GetTrialBalanceReportModel trialBalanceReportModel;
                    foreach (var mainHead in mainHeads)
                    {
                        if (mainHead == null) continue;
                        trialBalanceReportModel = new GetTrialBalanceReportModel();
                        trialBalanceReportModel.__children = new List<SubHeadTrialBalance>();
                        trialBalanceReportModel.head = mainHead;



                        foreach (var subHead in result.Where(x => x.MainHead == mainHead).Distinct().ToList())
                        {
                            var t = new SubHeadTrialBalance();
                            t.subHead = subHead.SubHead;
                            t.totalDebit = Math.Round((decimal)subHead.TotalDebit, 2).ToString();
                            t.totalCredit = Math.Round((decimal)subHead.TotalCredit, 2).ToString();
                            t.openBalance = Math.Round((decimal)(subHead.TotalCredit - subHead.TotalDebit), 2).ToString();
                            trialBalanceReportModel.__children.Add(t);

                        }
                        bb.Add(trialBalanceReportModel);
                    }
                }

                aa.Data = bb;
                return Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>.Success(aa, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>.Fail(aa, "Data not found");
            }
        }


        public async Task<Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>> TrailBalanceDetailedView(ReportFilter query)
        {
            GridWrapperResponse<List<GetTrialBalanceReportModel>> aa = new GridWrapperResponse<List<GetTrialBalanceReportModel>>();
            List<GetTrialBalanceReportModel> bb = new List<GetTrialBalanceReportModel>();

            try
            {


                await ExecuteSP(query);
                var res = from tb in _trialBalance.GetAll()
                          group tb by new { tb.TrialBalanceMainHead, tb.TrialBalanceSubHead, tb.TrialBalanceRelativeName, tb.TrialBalanceAccName, tb.TrialBalanceAccNo } into g
                          select new
                          {
                              MainHead = g.Key.TrialBalanceMainHead,
                              SubHead = g.Key.TrialBalanceSubHead,
                              RelativeName = g.Key.TrialBalanceRelativeName,
                              AccName = g.Key.TrialBalanceAccName,
                              AccNo = g.Key.TrialBalanceAccNo,
                              OpenBalance = g.Sum(tb => tb.TrialBalanceOpenBalance),
                              TotalDebit = g.Sum(tb => tb.TrialBalanceTotalDebit),
                              TotalCredit = g.Sum(tb => tb.TrialBalanceTotalCredit)
                          }
                           into summary
                          orderby summary.MainHead
                          select summary;

                // Execute the query and get the result
                var result = res.ToList();


                var mainHeads = result.Select(x => x.MainHead).ToList().Distinct();
                if (mainHeads.Count() > 0)
                {
                    GetTrialBalanceReportModel trialBalanceReportModel;
                    foreach (var mainHead in mainHeads)
                    {
                        if (mainHead == null) continue;
                        trialBalanceReportModel = new GetTrialBalanceReportModel();
                        trialBalanceReportModel.__children = new List<SubHeadTrialBalance>();
                        trialBalanceReportModel.head = mainHead;



                        foreach (var subHead in result.Where(x => x.MainHead == mainHead).Distinct().ToList())
                        {
                            var t = new SubHeadTrialBalance();
                            t.subHead = subHead.SubHead;

                            t.__children = new List<AccNameTrialBalance>();
                            foreach (var item in result.Where(x => x.SubHead == subHead.SubHead).Distinct().ToList())
                            {
                                var acc = new AccNameTrialBalance();
                                //acc.subHead = item.SubHead;
                                acc.accName = item.AccName;
                                acc.totalCredit = Math.Round((decimal)item.TotalCredit, 2).ToString();
                                acc.totalDebit = Math.Round((decimal)item.TotalDebit, 2).ToString();
                                acc.openBalance = Math.Round((decimal)(item.TotalCredit - item.TotalDebit), 2).ToString(); //Math.Round((decimal)item.OpenBalance , 2).ToString();

                                t.__children.Add(acc);

                            }
                            trialBalanceReportModel.__children.Add(t);
                        }
                        bb.Add(trialBalanceReportModel);
                    }
                }
                aa.Data = bb;
                return Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>.Success(aa, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>.Fail(aa, "Data not found");
            }

        }

        public async Task<Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>> TrailBalanceGroupView(ReportFilter query)
        {
            GridWrapperResponse<List<GetTrialBalanceReportModel>> aa = new GridWrapperResponse<List<GetTrialBalanceReportModel>>();
            List<GetTrialBalanceReportModel> bb = new List<GetTrialBalanceReportModel>();
            try
            {


                await ExecuteSP(query);
                var res = (from tb in _trialBalance.GetAll()
                          group tb by new { tb.TrialBalanceMainHead, tb.TrialBalanceSubHead, tb.TrialBalanceRelativeName } into g
                          select new
                          {
                              MainHead = g.Key.TrialBalanceMainHead,
                              SubHead = g.Key.TrialBalanceSubHead,
                              RelativeName = g.Key.TrialBalanceRelativeName,
                              OpenBalance = g.Sum(tb => tb.TrialBalanceOpenBalance),
                              TotalDebit = g.Sum(tb => tb.TrialBalanceTotalDebit),
                              TotalCredit = g.Sum(tb => tb.TrialBalanceTotalCredit)
                          }
                            into summary
                          orderby summary.MainHead
                          select summary).ToList();



                var mainHeads = res.Select(x => x.MainHead).ToList().Distinct();
                if (mainHeads.Count() > 0)
                {
                    GetTrialBalanceReportModel trialBalanceReportModel;
                    foreach (var mainHead in mainHeads)
                    {
                        if (mainHead == null) continue;
                        trialBalanceReportModel = new GetTrialBalanceReportModel();
                        trialBalanceReportModel.__children = new List<SubHeadTrialBalance>();
                        trialBalanceReportModel.head = mainHead;



                        foreach (var subHead in res.Where(x => x.MainHead == mainHead).Distinct().ToList())
                        {
                            var t = new SubHeadTrialBalance();
                            t.subHead = subHead.SubHead;

                            t.__children = new List<AccNameTrialBalance>();
                            foreach (var item in res.Where(x => x.SubHead == subHead.SubHead).Distinct().ToList())
                            {
                                var acc = new AccNameTrialBalance();
                                if (query.HideZeroTransactions == true)
                                {
                                    if ((decimal)item.TotalCredit > 0 || (decimal)item.TotalDebit > 0 || (decimal)item.OpenBalance > 0)
                                    {
                                        acc.accName = item.RelativeName;
                                        acc.totalCredit = Math.Round((decimal)item.TotalCredit, 2).ToString();
                                        acc.totalDebit = Math.Round((decimal)item.TotalDebit, 2).ToString();
                                        acc.openBalance = Math.Round((decimal)(item.TotalCredit - item.TotalDebit), 2).ToString(); //Math.Round((decimal)item.OpenBalance, 2).ToString();

                                        t.__children.Add(acc);
                                    }
                                }
                                else
                                {
                                    acc.accName = item.RelativeName;
                                    acc.totalCredit = Math.Round((decimal)item.TotalCredit, 2).ToString();
                                    acc.totalDebit = Math.Round((decimal)item.TotalDebit, 2).ToString();
                                    acc.openBalance = Math.Round((decimal)(item.TotalCredit - item.TotalDebit), 2).ToString(); //Math.Round((decimal)item.OpenBalance, 2).ToString();

                                    t.__children.Add(acc);
                                }


                            }
                            trialBalanceReportModel.__children.Add(t);
                        }
                        bb.Add(trialBalanceReportModel);
                    }
                }

                aa.Data = bb;
                return Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>.Success(aa, "Data found");

            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetTrialBalanceReportModel>>>.Fail(aa, "Data not found");
            }
        }


    }
}
