using Inspire.Erp.Application.Document.Interfaces;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Application.Document.Implementations
{
    public class HrStaffMasterService : IHrStaffMasterService

    {
        private IRepository<HrStaffMaster> hrStaffMasterrepository;
        //private IRepository<HrPassport> _hrpassportRepo;
        //private IRepository<HrDepartment> _HrDepartment;
        private readonly InspireErpDBContext _Context;
        public HrStaffMasterService(IRepository<HrStaffMaster> _hrStaffMasterrepository,  InspireErpDBContext Context)
        {
            hrStaffMasterrepository = _hrStaffMasterrepository;
            //_HrDepartment = HrDepartment;
            //_hrpassportRepo = hrpassportRepo;
            _Context = Context;
        }


        public IEnumerable<HrStaffMaster> GetAllHrStaffMaster()
        {
            IEnumerable<HrStaffMaster> hrStaffMaster;
            try
            {
                hrStaffMaster = hrStaffMasterrepository.GetAll();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //hrStaffTyperepository.Dispose();
            }
            return hrStaffMaster;
        }

        public IEnumerable<HrStaffMaster> InsertHrStaffMaster(HrStaffMaster hrStaffMaster)
        {
            bool valid = false;
            try
            {
                valid = true;
                hrStaffMasterrepository.Insert(hrStaffMaster);
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;
            }
            finally
            {
                //hrStaffTyperepository.Dispose();
            }
            return this.GetAllHrStaffMaster();
        }
        public IEnumerable<HrStaffMaster> UpdateHrStaffMaster(HrStaffMaster hrStaffMaster)
        {
            bool valid = false;
            try
            {
                hrStaffMasterrepository.Update(hrStaffMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //hrStaffTyperepository.Dispose();
            }
            return this.GetAllHrStaffMaster();
        }
        public IEnumerable<HrStaffMaster> DeleteHrStaffMaster(HrStaffMaster hrStaffMaster)
        {
            bool valid = false;
            try
            {
                hrStaffMasterrepository.Delete(hrStaffMaster);
                valid = true;

            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;

            }
            finally
            {
                //hrStaffTyperepository.Dispose();
            }
            return this.GetAllHrStaffMaster();
        }

        //public IQueryable GetDepartmentWiseStaff()
        //{
        //    try
        //    {
        //        var data = (from staff in hrStaffMasterrepository.GetAll()
        //                    join department in _HrDepartment.GetAll() on staff.DepartmentId equals department.DepartmentId
        //                    where (staff.FullName != "")
        //                    select new
        //                    {
        //                        FullName = staff.FullName,
        //                        StaffCode = staff.StaffCode,
        //                        DepartmentId = staff.DepartmentId,
        //                        Department = department.Department,
        //                    }).AsQueryable();
        //        return data;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IQueryable GetDesignationWiseStaff()
        //{
        //    try
        //    {
        //        var data = (from staff in hrStaffMasterrepository.GetAll()
        //                    join department in _Context.HrDesignation on staff.DesignationId equals department.DesignationId
        //                    where (staff.FullName != "" && staff.FullName != null)
        //                    select new
        //                    {
        //                        FullName = staff.FullName,
        //                        StaffCode = staff.StaffCode,
        //                        staffId = staff.StaffId,
        //                        DesignationId = staff.DesignationId,
        //                        Designation = department.Designation,
        //                    }).AsQueryable();
        //        return data;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IQueryable GetStaffReport(long? siteID, long? sponsorID, long? companyID, long? visaCompanyID, long? departmentID)
        //{
        //    try
        //    {
        //        var passportList = _hrpassportRepo.GetAll().ToList();
        //        var data = hrStaffMasterrepository.GetAll().Where(o => (siteID != null ? o.SiteId == siteID : true)
        //        && (sponsorID != null ? o.SponserId == sponsorID : true)
        //        && (companyID != null ? o.CompanyId == companyID : true)
        //        && (visaCompanyID != null ? o.VisaCompanyId == visaCompanyID : true)
        //        && (departmentID != null ? o.DepartmentId == departmentID : true)).Select(o => new
        //        {
        //            StaffCode = o.StaffCode,
        //            FullName = o.FullName,
        //            Doj = o.Doj,
        //            DesignationId = o.DesignationId,
        //            DepartmentId = o.DepartmentId,
        //            NationalityId = o.NationalityId,
        //            passPort = passportList.FirstOrDefault(x => x.StaffID == o.StaffId),
        //            EmpQid = o.EmpQid,
        //            Status = o.ActiveStatusId == 0 ? "InActive" : "Active",
        //        }).AsQueryable();
        //        return data;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public IQueryable GetStaffsBithday(string monthName)
        {
            try
            {
                Console.WriteLine(monthName);
                var data = hrStaffMasterrepository.GetAll().Where(o => o.Dob != null && o.Dob.Value.ToString("MMMM") == monthName).Select(o => new
                {
                    StaffCode = o.StaffCode,
                    StaffId = o.StaffId,
                    DOB = o.Dob,
                    FullName = o.FullName,
                    Day = o.Dob != null && o.Dob.Value != null ? o.Dob.Value.Day : 0,
                    DobmonthName = o.Dob != null && o.Dob.Value != null ? o.Dob.Value.ToString("MMMM") : "",
                }).OrderBy(o => o.Day).AsQueryable();
                return data;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //public IQueryable GetStaffOTRate()
        //{
        //    try
        //    {
        //        var data = (from staff in _Context.HrStaffMaster
        //                    join desi in _Context.HrDesignation on staff.DesignationId equals desi.DesignationId
        //                    join dept in _Context.HrDepartment on staff.DepartmentId equals dept.DepartmentId
        //                    select new
        //                    {
        //                        DepartmentName = dept.Department,
        //                        DesignationName = desi.Designation,
        //                        Staff = staff.StaffCode + staff.FullName,
        //                        NorOtrate = staff.NorOtrate,
        //                        HolOtrate = staff.HolOtrate,
        //                        FriOtrate = staff.FriOtrate,
        //                        StaffId = staff.StaffId
        //                    }).AsQueryable();
        //        return data;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public List<HrStaffMaster> InsertStaffOTRate(List<HrStaffMaster> dataList)
        {
            try
            {
                var staffIds = dataList.Select(o => o.StaffId).ToList();
                var StaffList = _Context.HrStaffMaster.Where(o => staffIds.Contains(o.StaffId)).ToList();
                List<HrStaffMaster> AddedData = new List<HrStaffMaster>();
                foreach (var item in dataList)
                {
                    var staffObj = StaffList.FirstOrDefault(o => o.StaffId == item.StaffId);
                    staffObj.NorOtrate = item.NorOtrate;
                    staffObj.HolOtrate = item.HolOtrate;
                    staffObj.FriOtrate = item.FriOtrate;
                    AddedData.Add(staffObj);
                }
                _Context.HrStaffMaster.UpdateRange(AddedData);
                _Context.SaveChanges();
                return dataList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //public IQueryable GetActiveStatusStaff()
        //{
        //    try
        //    {
        //        var data = (from hdr in _Context.HrStaffMaster
        //                    join act in _Context.HrActiveStatus on hdr.ActiveStatusId equals act.ActiveStatusId into left
        //                    from dtl in left.DefaultIfEmpty()
        //                    select new
        //                    {
        //                        Staff = hdr.StaffCode + " " + hdr.FullName,
        //                        ActiveStatus = dtl != null ? dtl.ActiveStatus : "",
        //                        ActiveStatusId = hdr.ActiveStatusId,
        //                        LastWorkingDay = hdr.LastWorkingDay,
        //                        DesignationId = hdr.DesignationId,
        //                        SiteId = hdr.SiteId,
        //                        StaffId = hdr.StaffId,
        //                    }).AsQueryable();
        //        return data;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public List<HrStaffMaster> UpdateActiveStatus(List<HrStaffMaster> dataList)
        {
            try
            {
                var staffIds = dataList.Select(o => o.StaffId).ToList();
                var StaffList = _Context.HrStaffMaster.Where(o => staffIds.Contains(o.StaffId)).ToList();
                List<HrStaffMaster> AddedData = new List<HrStaffMaster>();
                foreach (var item in dataList)
                {
                    var staffObj = StaffList.FirstOrDefault(o => o.StaffId == item.StaffId);
                    staffObj.ActiveStatusId = item.ActiveStatusId;
                    staffObj.LastWorkingDay = item.LastWorkingDay;
                    AddedData.Add(staffObj);
                }
                _Context.HrStaffMaster.UpdateRange(AddedData);
                _Context.SaveChanges();
                return dataList;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public HrStaffMaster getStaffByID(long staffID)
        {
            try
            {
                return _Context.HrStaffMaster.FirstOrDefault(o => o.StaffId == staffID);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public IQueryable GetDepartmentWiseStaff()
        {
            throw new NotImplementedException();
        }

        public IQueryable GetStaffReport(long? siteID, long? sponsorID, long? companyID, long? visaCompanyID, long? departmentID)
        {
            throw new NotImplementedException();
        }

        public IQueryable GetStaffOTRate()
        {
            throw new NotImplementedException();
        }

        public IQueryable GetActiveStatusStaff()
        {
            throw new NotImplementedException();
        }

        public IQueryable GetDesignationWiseStaff()
        {
            throw new NotImplementedException();
        }
    }
}
