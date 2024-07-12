using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.StoreWareHouse.Interface
{
    public interface IDamageEntry
    {
       // public VouchersNumbers DamageEntryPrefix();
        //public IEnumerable<DamageEntryModel> DamageEntryResponse();
        //public Task<bool> deleteDamageEntry(DamageMaster obj);
        //public Task<bool> updateDamageEntry(DamageEntryResponse Obj);
        //public Task<bool> submitDamageEntry(DemageRequest obj);


        public DamageMaster GetDamageById(int? id);
        public DamageMaster InsertDamageEntry(DamageMaster JobDamageMasterViewModel);
        public IEnumerable<DamageMaster> GetDamageEntry();
        public IEnumerable<DamageMaster> DeleteDamageEntry(string Id);

        

    }
}