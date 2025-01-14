using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Models.Master;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.VisualBasic;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class AssetsMasterService : IAssetsMasterService
    {
        private readonly IRepository<MasterAccountsTable> _masterAccountTable;
        private readonly IRepository<AssetMaster> _asetMaster;
        private readonly IRepository<AssetSubMaster> _asetSubMaster;
        public InspireErpDBContext _Context;
        public AssetsMasterService(IRepository<MasterAccountsTable> masterAccountTable, IRepository<AssetMaster> asetMaster, IRepository<AssetSubMaster> asetSubMaster,
            InspireErpDBContext Context)
        {
            _masterAccountTable = masterAccountTable;
            _asetMaster = asetMaster;
            _asetSubMaster = asetSubMaster;
            _Context = Context;
        }
        public async Task<Response<List<MasterAccountsTable>>> GetFixedAccountForAssetMaster(GenericGridViewModel model)
        {
            try
            {
                var response = new List<MasterAccountsTable>();
                var fixedAccount = await _masterAccountTable.GetAsQueryable().Where(x => x.MaAccName == "Fixed Asset").FirstOrDefaultAsync();
                var result = await _masterAccountTable.GetAsQueryable().Where(x => x.MaRelativeNo == fixedAccount.MaRelativeNo).ToListAsync();
                //var data = RecursionMasterAccountTable(result);
                return Response<List<MasterAccountsTable>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<MasterAccountsTable>>.Fail(new List<MasterAccountsTable>(), ex.Message);
            }
        }

        public async Task<Response<List<AssetMaster>>> GetFixedAssets(int relativeNo)
        {
            try
            {
                var result = await _asetMaster.GetAsQueryable().Where(x => x.AssetMasterAssetRelativeNo == relativeNo).ToListAsync();
                return Response<List<AssetMaster>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<AssetMaster>>.Fail(new List<AssetMaster>(), ex.Message);
            }
        }
        public async Task<Response<List<MasterAccountsTable>>> GetRelativeAccountAssetMaster(string accNo)
        {
            try
            {
                var result = await _masterAccountTable.GetAsQueryable().Where(x => x.MaAccNo == accNo).ToListAsync();

                return Response<List<MasterAccountsTable>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<MasterAccountsTable>>.Fail(new List<MasterAccountsTable>(), ex.Message);
            }
        }
        private List<MasterAccountsTable> RecursionMasterAccountTable(List<MasterAccountsTable> model)
        {
            var result = new List<MasterAccountsTable>();
            foreach (var account in model)
            {
                var masteraccount = _masterAccountTable.GetAsQueryable().Where(x => x.MaRelativeNo == account.MaAccNo).ToList();
                if (masteraccount.Count > 0)
                {
                    result.AddRange(RecursionMasterAccountTable(masteraccount));
                }
                result.Add(account);
            }
            return result;
        }

        public async Task<Response<AssetMaster>> GetAssetMaster(int id)
        {
            try
            {
                var result = await _asetMaster.GetAsQueryable().Where(x => x.AssetMasterAssetId == id).FirstOrDefaultAsync();
                return Response<AssetMaster>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<AssetMaster>.Fail(new AssetMaster(), ex.Message);
            }
        }

        public async Task<Response<List<AssetMaster>>> GetRelativeAssetMasterRecord(int accNo)
        {
            try
            {
                var result = await _asetMaster.GetAsQueryable().Where(x => x.AssetMasterAssetRelativeNo == accNo).ToListAsync();

                return Response<List<AssetMaster>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<AssetMaster>>.Fail(new List<AssetMaster>(), ex.Message);
            }
        }


        public async Task<Response<bool>> SaveNewAssetGroup(AssetGroupViewModel model)
        {
           
                using (var trans = _Context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                try
                {
                    //CREATING ASSET ACCOUNT IN CHART OF ACCOUNT
                    var masterAccFixed = new MasterAccountsTable();
                    masterAccFixed.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                                 .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                                 .DefaultIfEmpty()
                                                 .Max(o => o == null ? 0 : o.MaSno)) + 1;

                    string topAccNo = "01002";
                    var topAcc = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo.Trim() == topAccNo);

                    var AccNo = await _masterAccountTable.GetAsQueryable()
                                               .Where(x => Convert.ToInt64(x.MaAccNo.Trim()) > 0 && x.MaRelativeNo.Trim() == topAcc.MaAccNo.Trim())
                                               .DefaultIfEmpty()
                                               .MaxAsync(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Trim().Substring(o.MaAccNo.Trim().Length - 3, 3))) + 1;


                    masterAccFixed.MaAccNo = topAcc.MaRelativeNo.Trim() + String.Format("{0:00}", AccNo);
                    masterAccFixed.MaAccName = model.GroupName + " Assets";
                    masterAccFixed.MaMainHead = topAcc.MaMainHead ?? "";
                    masterAccFixed.MaSubHead = topAcc.MaSubHead ?? "";
                    masterAccFixed.MaAccType = "S";
                    masterAccFixed.MaRelativeNo = topAcc.MaAccNo.Trim();
                    masterAccFixed.MaStatus = AccountStatus.AccStatus;
                    masterAccFixed.MaFsno = 1;
                    masterAccFixed.MaStatus = "R";
                    masterAccFixed.MaUserId = 1;
                    masterAccFixed.MaDateCreated = DateTime.Now;
                    masterAccFixed.MaCurrencyId = 1;
                    masterAccFixed.MaGpAcc = "Y";
                    masterAccFixed.MaAcAcc = "Y";
                    masterAccFixed.MaEdAcc = "Y";
                    masterAccFixed.MaOpenBalance = 0;
                    masterAccFixed.MaTotalDebit = 0;
                    masterAccFixed.MaTotalCredit = 0;
                    _masterAccountTable.Insert(masterAccFixed);

                    //CREATING LIABILITIES ACCOUNT IN CHART OF ACCOUNT
                    var masterAccLiabilities = new MasterAccountsTable();
                    masterAccLiabilities.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                                 .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                                 .DefaultIfEmpty()
                                                 .Max(o => o == null ? 0 : o.MaSno)) + 1;


                    topAccNo = "03002";
                    topAcc = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo.Trim() == topAccNo);
                    AccNo = _masterAccountTable.GetAsQueryable()
                                               .Where(x => Convert.ToInt64(x.MaAccNo.Trim()) > 0 && x.MaRelativeNo.Trim() == topAcc.MaAccNo.Trim())
                                               .DefaultIfEmpty()
                                               .Max(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Trim().Substring(o.MaAccNo.Trim().Length - 3, 3))) + 1;


                    masterAccLiabilities.MaAccNo = topAcc.MaRelativeNo.Trim() + String.Format("{0:00}", AccNo);
                    masterAccLiabilities.MaAccName = model.GroupName + " Liabilities";
                    masterAccLiabilities.MaMainHead = topAcc.MaMainHead ?? "";
                    masterAccLiabilities.MaSubHead = topAcc.MaSubHead ?? "";
                    masterAccLiabilities.MaAccType = "S";
                    masterAccLiabilities.MaRelativeNo = topAcc.MaAccNo.Trim();
                    masterAccLiabilities.MaStatus = AccountStatus.AccStatus;
                    masterAccLiabilities.MaFsno = 1;
                    masterAccLiabilities.MaStatus = "R";
                    masterAccLiabilities.MaUserId = 1;
                    masterAccLiabilities.MaDateCreated = DateTime.Now;
                    masterAccLiabilities.MaCurrencyId = 1;
                    masterAccLiabilities.MaGpAcc = "Y";
                    masterAccLiabilities.MaAcAcc = "Y";
                    masterAccLiabilities.MaEdAcc = "Y";
                    masterAccLiabilities.MaOpenBalance = 0;
                    masterAccLiabilities.MaTotalDebit = 0;
                    masterAccLiabilities.MaTotalCredit = 0;
                    _masterAccountTable.Insert(masterAccLiabilities);


                    //CREATING LIABILITIES ACCOUNT IN CHART OF ACCOUNT
                    var masterAccExpenses = new MasterAccountsTable();
                    masterAccExpenses.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                                 .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                                 .DefaultIfEmpty()
                                                 .Max(o => o == null ? 0 : o.MaSno)) + 1;


                    topAccNo = "04002";
                    topAcc = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo.Trim() == topAccNo);
                    AccNo = _masterAccountTable.GetAsQueryable()
                                               .Where(x => Convert.ToInt64(x.MaAccNo.Trim()) > 0 && x.MaRelativeNo.Trim() == topAcc.MaAccNo.Trim())
                                               .DefaultIfEmpty()
                                               .Max(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Trim().Substring(o.MaAccNo.Trim().Length - 3, 3))) + 1;


                    masterAccExpenses.MaAccNo = topAcc.MaRelativeNo + String.Format("{0:00}", AccNo);
                    masterAccExpenses.MaAccName = model.GroupName + " Expenses";
                    masterAccExpenses.MaMainHead = topAcc.MaMainHead ?? "";
                    masterAccExpenses.MaSubHead = topAcc.MaSubHead ?? "";
                    masterAccExpenses.MaAccType = "S";
                    masterAccExpenses.MaRelativeNo = topAcc.MaAccNo.Trim();
                    masterAccExpenses.MaStatus = AccountStatus.AccStatus;
                  
                    masterAccExpenses.MaFsno = 1;
                    masterAccExpenses.MaStatus = "R";
                    masterAccExpenses.MaUserId = 1;
                    masterAccExpenses.MaDateCreated = DateTime.Now;
                    masterAccExpenses.MaCurrencyId = 1;
                    masterAccExpenses.MaGpAcc = "Y";
                    masterAccExpenses.MaAcAcc = "Y";
                    masterAccExpenses.MaEdAcc = "Y";
                    masterAccExpenses.MaOpenBalance = 0;
                    masterAccExpenses.MaTotalDebit = 0;
                    masterAccExpenses.MaTotalCredit = 0;
                    _masterAccountTable.Insert(masterAccExpenses);

                    //Assets here
                    int maxAssetId = Convert.ToInt32(_asetMaster.GetAsQueryable()
                                                .Where(x => Convert.ToInt32(x.AssetMasterAssetId) > 0)
                                                .DefaultIfEmpty()
                                                .Max(o => o == null ? 0 : o.AssetMasterAssetId)) + 1;


                    var assetMaster = new AssetMaster()
                    {

                        AssetMasterAssetCode = model.GroupName.Substring(0, 2) + maxAssetId,
                        AssetMasterAssetName = model.GroupName,
                        AssetMasterAssetType = ItemMasterStatus.Group,
                        AssetMasterAssetCreatedDate = DateTime.Now,
                        AssetMasterAssetRelativeNo = model.AssetGroupId,
                        AssetMasterAssetAccountNo = masterAccFixed.MaAccNo,
                        AssetMasterAssetDepExpAccount = masterAccExpenses.MaAccNo,
                        AssetMasterAssetDepLibAccount = masterAccLiabilities.MaAccNo,
                    };
                    _asetMaster.Insert(assetMaster);


                    await trans.CommitAsync();
                    return Response<bool>.Success(true, "Data found");

                }
            catch (Exception ex)
            {
                    await trans.RollbackAsync();
                    return Response<bool>.Fail(false, ex.Message);
            }
        }
    }
           


        public async Task<Response<bool>> EditAssetGroup(AssetGroupEditViewModel model)
        {
            try
            {
                //CREATING ASSET ACCOUNT IN CHART OF ACCOUNT
                var asset = await _asetMaster.GetAsQueryable().FirstOrDefaultAsync(x => x.AssetMasterAssetId == model.AssetId);
                var masterAcc = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == asset.AssetMasterAssetAccountNo);
                masterAcc.MaAccName = model.GroupName + " Assets";
                _masterAccountTable.Update(masterAcc);
                var masterAccLiab = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == asset.AssetMasterAssetDepLibAccount);
                masterAccLiab.MaAccName = model.GroupName + " Liabilities";
                _masterAccountTable.Update(masterAccLiab);
                var masterAccExp = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == asset.AssetMasterAssetDepExpAccount);
                masterAccExp.MaAccName = model.GroupName + " Expenses";
                _masterAccountTable.Update(masterAccExp);

                asset.AssetMasterAssetName = model.GroupName;
                asset.AssetMasterAssetCode = model.GroupName.Substring(0, 2) + asset.AssetMasterAssetId;
                _asetMaster.Update(asset);
                return Response<bool>.Success(true, "Data Saved");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(false, ex.Message);
            }
        }

        public async Task<Response<bool>> EditAssetHead(AssetGroupEditViewModel model)
        {
            try
            {
                //CREATING ASSET ACCOUNT IN CHART OF ACCOUNT
                var asset = await _asetMaster.GetAsQueryable().FirstOrDefaultAsync(x => x.AssetMasterAssetId == model.AssetId);
                var masterAcc = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == asset.AssetMasterAssetAccountNo);
                masterAcc.MaAccName = model.GroupName +" " + "[ASSET]";
                masterAcc.MaManualCode = model.GroupName.Substring(0, 2) + asset.AssetMasterAssetId;
                _masterAccountTable.Update(masterAcc);

                var masterAccLiab = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == asset.AssetMasterAssetDepLibAccount);
                masterAccLiab.MaAccName = model.GroupName + " " + "[LIABILITY]";
                _masterAccountTable.Update(masterAccLiab);

                var masterAccExp = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == asset.AssetMasterAssetDepExpAccount);
                masterAccExp.MaAccName = model.GroupName + " [EXPENSE]";
                _masterAccountTable.Update(masterAccExp);

                asset.AssetMasterAssetName = model.GroupName;
                asset.AssetMasterAssetCode = model.GroupName.Substring(0, 2) + asset.AssetMasterAssetId;
                _asetMaster.Update(asset);
                return Response<bool>.Success(true, "Data Saved");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(false, ex.Message);
            }
        }


        public async Task<Response<bool>> SaveNewAssetHead(AssetHeaderViewModel model)
        {
            try
            {

                var asset=await _asetMaster.GetAsQueryable().FirstOrDefaultAsync(x => x.AssetMasterAssetId == model.AssetGroupId);
                var masterAcc = await _masterAccountTable.GetAsQueryable().FirstOrDefaultAsync(x => x.MaAccNo == asset.AssetMasterAssetAccountNo);
                //CREATING ASSET ACCOUNT IN CHART OF ACCOUNT
                var masterAccFixed = new MasterAccountsTable();
                masterAccFixed.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                             .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                             .DefaultIfEmpty()
                                             .Max(o => o == null ? 0 : o.MaSno)) + 1;

                masterAccFixed.MaRelativeNo = masterAcc.MaAccNo;
                var AccNo = await _masterAccountTable.GetAsQueryable()
                                           .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == masterAccFixed.MaRelativeNo)
                                           .DefaultIfEmpty()
                                           .MaxAsync(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, 3))) + 1;

                masterAccFixed.MaAccNo = masterAccFixed.MaRelativeNo + String.Format("{0:00}", AccNo);
                masterAccFixed.MaAccName = model.HeadName + " " + "[ASSET]";
                masterAccFixed.MaMainHead = masterAcc.MaMainHead?? "";
                masterAccFixed.MaSubHead = masterAcc.MaMainHead ?? "";
                masterAccFixed.MaAccType = "A";
                masterAccFixed.MaStatus = AccountStatus.AccStatus;
                masterAccFixed.MaImageKey = ItemMasterStatus.ItemKey;
                masterAccFixed.MaManualCode = model.HeadName.Substring(0, 2) + AccNo;
                //if (model.AccountType == ItemMasterStatus.Group || model.AccountType == AccountMasterStatus.SubItem)
                //{

                //    masterAccFixed.MaImageKey = ItemMasterStatus.GroupItem;
                //}
                ////else
                //{
                //    masterAccFixed.MaImageKey = ItemMasterStatus.SubItem;
                //}

                masterAccFixed.MaSystemAcc = "U";
                masterAccFixed.MaFsno = 1;
                masterAccFixed.MaStatus = "R";
                masterAccFixed.MaUserId = 1;
                masterAccFixed.MaDateCreated = DateTime.Now;
                masterAccFixed.MaCurrencyId = 1;
                masterAccFixed.MaGpAcc = "Y";
                masterAccFixed.MaAcAcc = "Y";
                masterAccFixed.MaEdAcc = "Y";
                masterAccFixed.MaOpenBalance = 0;
                masterAccFixed.MaTotalDebit = 0;
                masterAccFixed.MaTotalCredit = 0;
                _masterAccountTable.Insert(masterAccFixed);

                var masterAccLiab = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == asset.AssetMasterAssetDepLibAccount);
                //CREATING LIABILITIES ACCOUNT IN CHART OF ACCOUNT
                var masterAccLiabilities = new MasterAccountsTable();
                masterAccLiabilities.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                             .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                             .DefaultIfEmpty()
                                             .Max(o => o == null ? 0 : o.MaSno)) + 1;


                masterAccLiabilities.MaRelativeNo =masterAccLiab.MaAccNo;
                AccNo = await _masterAccountTable.GetAsQueryable()
                                           .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == masterAccLiabilities.MaRelativeNo)
                                           .DefaultIfEmpty()
                                           .MaxAsync(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, 3))) + 1;

                masterAccLiabilities.MaAccNo = masterAccLiabilities.MaRelativeNo + String.Format("{0:00}", AccNo);
                masterAccLiabilities.MaAccName = model.HeadName + " " + "[LIABILITY]";
                masterAccLiabilities.MaMainHead = masterAcc.MaMainHead ?? "";
                masterAccLiabilities.MaSubHead = masterAcc.MaMainHead ?? "";
                masterAccLiabilities.MaAccType = "A";
                masterAccLiabilities.MaStatus = AccountStatus.AccStatus;
                masterAccLiabilities.MaImageKey = ItemMasterStatus.ItemKey;
                masterAccLiabilities.MaSystemAcc = "U";
                masterAccLiabilities.MaFsno = 1;
                masterAccLiabilities.MaStatus = "R";
                masterAccLiabilities.MaUserId = 1;
                masterAccLiabilities.MaDateCreated = DateTime.Now;
                masterAccLiabilities.MaCurrencyId = 1;
                masterAccLiabilities.MaGpAcc = "Y";
                masterAccLiabilities.MaAcAcc = "Y"; 
                masterAccLiabilities.MaEdAcc = "Y";
                masterAccLiabilities.MaOpenBalance =0;
                masterAccLiabilities.MaTotalDebit = 0;
                masterAccLiabilities.MaTotalCredit = 0;
                masterAccLiabilities.MaManualCode = model.HeadName.Substring(0, 2) + AccNo;
                _masterAccountTable.Insert(masterAccLiabilities);


                var masterAccExp= _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == asset.AssetMasterAssetDepExpAccount);
                //CREATING LIABILITIES ACCOUNT IN CHART OF ACCOUNT
                var masterAccExpenses = new MasterAccountsTable();
                masterAccExpenses.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                             .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                             .DefaultIfEmpty()
                                             .Max(o => o == null ? 0 : o.MaSno)) + 1;


                masterAccExpenses.MaRelativeNo = masterAccExp.MaAccNo;
                AccNo = await _masterAccountTable.GetAsQueryable()
                                           .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == masterAccExpenses.MaRelativeNo)
                                           .DefaultIfEmpty()
                                           .MaxAsync(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, 3))) + 1;

                masterAccExpenses.MaAccNo = masterAccExpenses.MaRelativeNo + String.Format("{0:00}", AccNo);
                masterAccExpenses.MaAccName = model.HeadName + " " + "[EXPENSE]";
                masterAccExpenses.MaMainHead = masterAcc.MaMainHead ?? "";
                masterAccExpenses.MaSubHead = masterAcc.MaMainHead ?? "";
                masterAccExpenses.MaAccType = "A";
                masterAccExpenses.MaStatus = AccountStatus.AccStatus;
                masterAccExpenses.MaImageKey = ItemMasterStatus.ItemKey;
                masterAccExpenses.MaSystemAcc = "U";
                masterAccExpenses.MaSystemAcc = "U";
                masterAccExpenses.MaFsno = 1;
                masterAccExpenses.MaStatus = "R";
                masterAccExpenses.MaUserId = 1;
                masterAccExpenses.MaDateCreated = DateTime.Now;
                masterAccExpenses.MaCurrencyId = 1;
                masterAccExpenses.MaGpAcc = "Y";
                masterAccExpenses.MaAcAcc = "Y";
                masterAccExpenses.MaEdAcc = "Y";
                masterAccExpenses.MaOpenBalance = 0;
                masterAccExpenses.MaTotalDebit = 0;
                masterAccExpenses.MaTotalCredit = 0;
                masterAccExpenses.MaManualCode = model.HeadName.Substring(0, 2) + AccNo;
                _masterAccountTable.Insert(masterAccExpenses);


                //Assets here
                var assetMaster = new AssetMaster()
                {
                    AssetMasterAssetCode = model.HeadName.Substring(0, 2) + AccNo,
                    AssetMasterAssetName = model.HeadName,
                    AssetMasterAssetType = ItemMasterStatus.SubItem,
                    AssetMasterAssetCreatedDate = DateTime.Now,
                    AssetMasterAssetRelativeNo = Convert.ToInt32(model.AssetGroupId),
                    AssetMasterAssetAccountNo = masterAccFixed.MaAccNo,
                    AssetMasterAssetDepExpAccount = masterAccExpenses.MaAccNo,
                    AssetMasterAssetDepLibAccount = masterAccLiabilities.MaAccNo,
                };

                _asetMaster.Insert(assetMaster);
                return Response<bool>.Success(true, "Data found");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(false, ex.Message);
            }
        }

        public async Task<Response<bool>> AddEditAssetMaster(AssetMasterViewModel model)
        {
            try
            {
                if (model.AssetMasterAssetId == 0)
                {
                    var result = new AssetMaster()
                    {
                        AssetMasterAssetCode = model.AssetMasterAssetCode,
                        AssetMasterAssetName = model.AssetMasterAssetName,
                        AssetMasterAssetType = model.AssetMasterAssetType,
                        AssetMasterAssetCreatedDate = DateTime.Now,
                        AssetMasterAssetRelativeNo = model.AssetMasterAssetRelativeNo,
                        AssetMasterAssetAccountNo="",
                        AssetMasterAssetDepExpAccount= "",
                        AssetMasterAssetDepLibAccount= "",
                    };

                    _asetMaster.Insert(result);
                }
                else
                {
                    var result = await _asetMaster.GetAsQueryable().Where(x => x.AssetMasterAssetId == model.AssetMasterAssetId).FirstOrDefaultAsync();
                    result.AssetMasterAssetCode = model.AssetMasterAssetCode;
                    result.AssetMasterAssetName = model.AssetMasterAssetName;
                    result.AssetMasterAssetType = model.AssetMasterAssetType;
                    result.AssetMasterAssetRelativeNo =model.AssetMasterAssetRelativeNo;
                    _asetMaster.Update(result);
                }
                
                return Response<bool>.Success(true, "Data found");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(false, ex.Message);
            }
        }

        public async Task<Response<List<AssetCodes>>> GetAssetsCodes()
        {
            try
            {
                var result = (from assetSubMaster in _asetSubMaster.GetAsQueryable() 
                             join assetMaster in _asetMaster.GetAsQueryable() 
                             on assetSubMaster.GroupID equals assetMaster.AssetMasterAssetId
                             select new AssetCodes
                             {
                                GroupID = (long)assetSubMaster.GroupID,
                                AssetMasterAssetId = assetMaster.AssetMasterAssetId,
                                AssetCode = assetSubMaster.AssetCode,
                                AssetDescription = assetSubMaster.AssetDescription,
                                SubAssetID = assetSubMaster.SubAssetID,
                                AssetMasterAssetName = assetMaster.AssetMasterAssetName

                             }).OrderBy(x => x.AssetMasterAssetName).ToList();
                return Response<List<AssetCodes>>.Success(result, "Data found");

            }
            catch (Exception ex)
            {
                return Response<List<AssetCodes>>.Fail(new List<AssetCodes>(), ex.Message);
            }
        }

        public async Task<Response<List<AssetMaster>>> GetAllAssets()
        {
            try
            {
                var result =  this._asetMaster.GetAsQueryable()
                                                                       .Where(x => x.AssetMasterAssetType == ItemMasterStatus.SubItem)
                                                                       .OrderBy(x => x.AssetMasterAssetName)
                                                                       .ToList();
                return Response<List<AssetMaster>>.Success(result, "Data found");

            }
            catch (Exception ex)
            {
                return Response<List<AssetMaster>>.Fail(new List<AssetMaster>(), ex.Message);
            }
        }
    }
}
