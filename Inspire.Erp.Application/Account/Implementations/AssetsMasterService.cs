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

namespace Inspire.Erp.Application.Account.Implementations
{
    public class AssetsMasterService : IAssetsMasterService
    {
        private readonly IRepository<MasterAccountsTable> _masterAccountTable;
        private readonly IRepository<AssetMaster> _asetMaster;
        private readonly IRepository<AssetSubMaster> _asetSubMaster;

        public AssetsMasterService(IRepository<MasterAccountsTable> masterAccountTable, IRepository<AssetMaster> asetMaster, IRepository<AssetSubMaster> asetSubMaster)
        {
            _masterAccountTable = masterAccountTable;
            _asetMaster = asetMaster;
            _asetSubMaster = asetSubMaster;
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
            try 
            {
                //CREATING ASSET ACCOUNT IN CHART OF ACCOUNT
                var masterAccFixed = new MasterAccountsTable();
                masterAccFixed.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                             .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                             .DefaultIfEmpty()
                                             .Max(o => o == null ? 0 : o.MaSno)) + 1;

                masterAccFixed.MaRelativeNo = "01";
                var AccNo = _masterAccountTable.GetAsQueryable()
                                           .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == masterAccFixed.MaRelativeNo)
                                           .DefaultIfEmpty()
                                           .Max(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, 3))) + 1;

                masterAccFixed.MaAccNo = masterAccFixed.MaRelativeNo + String.Format("{0:00}", AccNo);
                masterAccFixed.MaAccName = model.GroupName;

                var mainHead = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == masterAccFixed.MaRelativeNo).MaMainHead;
                var subHead = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == masterAccFixed.MaRelativeNo).MaSubHead;

                masterAccFixed.MaMainHead = mainHead ?? "";
                masterAccFixed.MaSubHead = subHead ?? "";
                masterAccFixed.MaAccType = model.AccountType;

                masterAccFixed.MaStatus = AccountStatus.AccStatus;

                if (model.AccountType == ItemMasterStatus.Group || model.AccountType == AccountMasterStatus.SubItem)
                {

                    masterAccFixed.MaImageKey = ItemMasterStatus.GroupItem;
                }
                else
                {
                    masterAccFixed.MaImageKey = ItemMasterStatus.SubItem;
                }

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
                //masterAccFixed.MaManualCode = MaManualCode;
                //masterAcc.MaIsAirAcc = MaIsAirAcc;
                //masterAcc.MaIsSeaAcc = MaIsSeaAcc;
                //masterAcc.MaCostCenterId = MaCostCenterId;
                //MaCostCenterSub = MaCostCenterSub;
                //masterAcc.MaSortNo = MaSortNo;
                //masterAcc.MaShowSuminTb = MaShowSuminTb;
                //masterAcc.MaAssetValue = MaAssetValue;
                //masterAcc.MaAssetDepValue = MaAssetDepValue;
                //masterAcc.MaAssetQty = MaAssetQty;
                //masterAcc.MaLifeInYrs = MaLifeInYrs;
                //masterAcc.MaAssetDepMode = MaAssetDepMode;
                //masterAcc.MaAssetDate = MaAssetDate;
                //masterAcc.MaIsAsset = MaIsAsset;
                _masterAccountTable.Insert(masterAccFixed);

                //CREATING LIABILITIES ACCOUNT IN CHART OF ACCOUNT
                var masterAccLiabilities = new MasterAccountsTable();
                masterAccLiabilities.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                             .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                             .DefaultIfEmpty()
                                             .Max(o => o == null ? 0 : o.MaSno)) + 1;


                masterAccLiabilities.MaRelativeNo = "03";
                AccNo = _masterAccountTable.GetAsQueryable()
                                           .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == masterAccLiabilities.MaRelativeNo)
                                           .DefaultIfEmpty()
                                           .Max(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, 3))) + 1;

                masterAccLiabilities.MaAccNo = masterAccLiabilities.MaRelativeNo + String.Format("{0:00}", AccNo);
                masterAccLiabilities.MaAccName = model.GroupName;

                mainHead = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == masterAccLiabilities.MaRelativeNo).MaMainHead;
                subHead = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == masterAccLiabilities.MaRelativeNo).MaSubHead;

                masterAccLiabilities.MaMainHead = mainHead ?? "";
                masterAccLiabilities.MaSubHead = subHead ?? "";
                masterAccLiabilities.MaAccType = model.AccountType;

                masterAccLiabilities.MaStatus = AccountStatus.AccStatus;

                if (model.AccountType == ItemMasterStatus.Group || model.AccountType == AccountMasterStatus.SubItem)
                {

                    masterAccLiabilities.MaImageKey = ItemMasterStatus.GroupItem;
                }
                else
                {
                    masterAccLiabilities.MaImageKey = ItemMasterStatus.SubItem;
                }

                masterAccLiabilities.MaSystemAcc = "U";
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


                masterAccExpenses.MaRelativeNo = "04";
                AccNo = _masterAccountTable.GetAsQueryable()
                                           .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == masterAccExpenses.MaRelativeNo)
                                           .DefaultIfEmpty()
                                           .Max(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, 3))) + 1;

                masterAccExpenses.MaAccNo = masterAccExpenses.MaRelativeNo + String.Format("{0:00}", AccNo);
                masterAccExpenses.MaAccName = model.GroupName;

                mainHead = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == masterAccExpenses.MaRelativeNo).MaMainHead;
                subHead = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == masterAccExpenses.MaRelativeNo).MaSubHead;

                masterAccExpenses.MaMainHead = mainHead ?? "";
                masterAccExpenses.MaSubHead = subHead ?? "";
                masterAccExpenses.MaAccType = model.AccountType;

                masterAccExpenses.MaStatus = AccountStatus.AccStatus;

                if (model.AccountType == ItemMasterStatus.Group || model.AccountType == AccountMasterStatus.SubItem)
                {

                    masterAccExpenses.MaImageKey = ItemMasterStatus.GroupItem;
                }
                else
                {
                    masterAccExpenses.MaImageKey = ItemMasterStatus.SubItem;
                }

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
                _masterAccountTable.Insert(masterAccExpenses);


                //Assets here
                var assetMaster = new AssetMaster()
                {
                    AssetMasterAssetCode = model.GroupName.Substring(0, 2) + AccNo,
                    AssetMasterAssetName = model.GroupName,
                    AssetMasterAssetType = ItemMasterStatus.Group,
                    AssetMasterAssetCreatedDate = DateTime.Now,
                    AssetMasterAssetRelativeNo = Convert.ToInt32(model.ParentRelativeNo),
                    AssetMasterAssetAccountNo = masterAccFixed.MaAccNo,
                    AssetMasterAssetDepExpAccount = masterAccLiabilities.MaAccNo,
                    AssetMasterAssetDepLibAccount = masterAccExpenses.MaAccNo,
                };

                _asetMaster.Insert(assetMaster);
                return Response<bool>.Success(true, "Data found");
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(false, ex.Message);
            }
        }



        public async Task<Response<bool>> SaveNewAssetHead(AssetGroupViewModel model)
        {
            try
            {
                //CREATING ASSET ACCOUNT IN CHART OF ACCOUNT
                var masterAccFixed = new MasterAccountsTable();
                masterAccFixed.MaSno = Convert.ToInt32(_masterAccountTable.GetAsQueryable()
                                             .Where(x => Convert.ToInt32(x.MaSno) > 0)
                                             .DefaultIfEmpty()
                                             .Max(o => o == null ? 0 : o.MaSno)) + 1;

                masterAccFixed.MaRelativeNo = model.ParentRelativeNo;
                var AccNo = _masterAccountTable.GetAsQueryable()
                                           .Where(x => Convert.ToInt64(x.MaAccNo) > 0 && x.MaRelativeNo == masterAccFixed.MaRelativeNo)
                                           .DefaultIfEmpty()
                                           .Max(o => o == null ? 0 : Convert.ToInt64(o.MaAccNo.Substring(o.MaAccNo.Length - 3, 3))) + 1;

                masterAccFixed.MaAccNo = masterAccFixed.MaRelativeNo + String.Format("{0:00}", AccNo);
                masterAccFixed.MaAccName = model.GroupName;

                var mainHead = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == masterAccFixed.MaRelativeNo).MaMainHead;
                var subHead = _masterAccountTable.GetAsQueryable().FirstOrDefault(x => x.MaAccNo == masterAccFixed.MaRelativeNo).MaSubHead;

                masterAccFixed.MaMainHead = mainHead ?? "";
                masterAccFixed.MaSubHead = subHead ?? "";
                masterAccFixed.MaAccType = model.AccountType;

                masterAccFixed.MaStatus = AccountStatus.AccStatus;

                if (model.AccountType == ItemMasterStatus.Group || model.AccountType == AccountMasterStatus.SubItem)
                {

                    masterAccFixed.MaImageKey = ItemMasterStatus.GroupItem;
                }
                else
                {
                    masterAccFixed.MaImageKey = ItemMasterStatus.SubItem;
                }

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

                             }).ToList();
                return Response<List<AssetCodes>>.Success(result, "Data found");

            }
            catch (Exception ex)
            {
                return Response<List<AssetCodes>>.Fail(new List<AssetCodes>(), ex.Message);
            }
        }
    }
}
