using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Application.Store.Interfaces;

namespace Inspire.Erp.Application.Store.implementations
{
    public class ManufactureItems : IManufactureItems
    {
        private readonly IRepository<ProgramSettings> _programsettingsrepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly IRepository<ManufactureItemsMaster> _ManufactureItemsMastererepo;
        private readonly IRepository<ManufactureItemsMasterDetails> _ManufactureItemsMasterDetailsrepo;
        private static string prefix;
        public ManufactureItems(IRepository<ProgramSettings> programsettingsRepository,
                IRepository<VouchersNumbers> voucherNumbers,
                IRepository<ManufactureItemsMaster> ManufactureItemsMaster,
                IRepository<ManufactureItemsMasterDetails> ManufactureItemsMasterDetails
            )
        {
            
            _voucherNumbersRepository = voucherNumbers;
            _programsettingsrepository = programsettingsRepository;
            _ManufactureItemsMastererepo = ManufactureItemsMaster;
            _ManufactureItemsMasterDetailsrepo = ManufactureItemsMasterDetails;
        }

       public ManufactureItemsMaster InsertManufactureItems(ManufactureItemsMaster ManufactureItems)
        {
            try
            {
                

                string ManufactureItemsVoucherNumber = this.GenerateVoucherNo(System.DateTime.Now).VouchersNumbersVNo;
                ManufactureItems.MI_VocherNo = ManufactureItemsVoucherNumber;

                _ManufactureItemsMastererepo.BeginTransaction();
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _ManufactureItemsMastererepo.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.MI_Id_N) + 1);
                ManufactureItems.MI_Id_N = maxcount;

                _ManufactureItemsMastererepo.Insert(ManufactureItems);
                ManufactureItems.ManufactureItemsMasterDetails.ForEach(elemt =>
                {
                    elemt.MI_Id_N = maxcount;
                    elemt.MI_Details_VocherNo = ManufactureItems.MI_VocherNo;
                });
                _ManufactureItemsMasterDetailsrepo.InsertList(ManufactureItems.ManufactureItemsMasterDetails);
                _ManufactureItemsMasterDetailsrepo.TransactionCommit();
                return this.GetManufactureItemsById(ManufactureItems.MI_Id_N);
            }
            catch (Exception ex)
            {
                _ManufactureItemsMastererepo.TransactionRollback();
                throw ex;
            }
        }
        public ManufactureItemsMaster UpdateManufactureItems(ManufactureItemsMaster ManufactureItems)
        {
            try
            {
                _ManufactureItemsMastererepo.BeginTransaction();
                _ManufactureItemsMastererepo.Update(ManufactureItems);
                //_ManufactureItemsMastererepo.UpdateList(ManufactureItems.ManufactureItemsMasterDetails);
                _ManufactureItemsMastererepo.TransactionCommit();
                return this.GetManufactureItemsById(ManufactureItems.MI_Id_N);
            }
            catch (Exception ex)
            {
                _ManufactureItemsMastererepo.TransactionRollback();
                throw ex;
            }
        }
        public IEnumerable<ManufactureItemsMaster> DeleteManufactureItems(string Id)
        {
            try
            {
                var dataObj = _ManufactureItemsMastererepo.GetAsQueryable().Where(o => o.MI_VocherNo == Id).FirstOrDefault();
                dataObj.ManufactureItemsMasterStatus = true;
                var detailData = _ManufactureItemsMastererepo.GetAsQueryable().Where(o => o.MI_VocherNo == Id).ToList();
                detailData.ForEach(elemt =>
                {
                    elemt.ManufactureItemsMasterStatus = true;
                });
                _ManufactureItemsMastererepo.Update(dataObj);
                _ManufactureItemsMastererepo.UpdateList(detailData);
                 return _ManufactureItemsMastererepo.GetAll();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public IEnumerable<ManufactureItemsMaster> GetManufactureItemsMaster()
        {
            try
            {
                var detailList = _ManufactureItemsMasterDetailsrepo.GetAll().ToList();
                var data = _ManufactureItemsMastererepo.GetAll().Select(o => new
                {
                    JobInvoice = o,
                    JobInvoiceDetails = detailList.Where(a => Convert.ToInt32(a.MID_Id_N) == Convert.ToInt32(o.MI_Id_N)).ToList()
                }).ToList();
                return _ManufactureItemsMastererepo.GetAll();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ManufactureItemsMaster GetManufactureItemsById(int? id)
        {
            try
            {
                var ManufactureItemsMaster = new ManufactureItemsMaster();
                ManufactureItemsMaster = _ManufactureItemsMastererepo.GetAsQueryable().FirstOrDefault(x => x.MI_Id_N == Convert.ToInt32(id));
                ManufactureItemsMaster.ManufactureItemsMasterDetails = _ManufactureItemsMasterDetailsrepo.GetAsQueryable().Where(x => x.MID_Id_N == Convert.ToInt32(id)).ToList();
                return ManufactureItemsMaster;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ManufactureItemsMaster GetManufactureItemVoucherNo(string VNo)
        {
            try
            {
             
                ManufactureItemsMaster ManufactureItem = new ManufactureItemsMaster();
                ManufactureItem = _ManufactureItemsMastererepo.GetAsQueryable().Where(k => k.MI_VocherNo == VNo).SingleOrDefault();
                ManufactureItem.ManufactureItemsMasterDetails =_ManufactureItemsMasterDetailsrepo.GetAsQueryable().Where(x => x.MI_Details_VocherNo == VNo).ToList();
                return ManufactureItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsrepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.ManufactureItem_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.ManufactureItem_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                //var prefix = "CN";
                //int vnoMaxVal = 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.ManufactureItem_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
}
