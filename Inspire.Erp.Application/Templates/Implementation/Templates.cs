using Inspire.Erp.Application.Templates.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inspire.Erp.Application.Templates.Implementation
{
    public class Templates : ITemplates
    {
        private IRepository<Qtemplates> qtmplts;

        public Templates(IRepository<Qtemplates> qtmplts)
        {
            this.qtmplts = qtmplts;
        }

        public async Task<Response<bool>> DeleteTemplate(int qtemplateId)
        {
            try
            {
                var qtmp = await this.qtmplts.GetAsQueryable().SingleOrDefaultAsync(x => x.QtemplatesId == qtemplateId);
                qtmp.QtemplatesDelStatus = true;
                this.qtmplts.UpdateAsync(qtmp);
                return Response<bool>.Success(true, "Record Deleted");
            }
            catch(Exception ex)
            {
                return Response<bool>.Fail(false, ex.Message);
            }
        }

        public async Task<Response<List<Qtemplates>>> GetAllActiveTemplates()
        {
            try
            {
                await Task.Delay(1000);
                var data = this.qtmplts.GetAsQueryable().Where(x => x.QtemplatesDelStatus == false).ToList();
                return Response<List<Qtemplates>>.Success(data, "Data Found");
            }
            catch (Exception ex)
            {
                return Response<List<Qtemplates>>.Fail(new List<Qtemplates>(), ex.Message);
            }
        }

        public async Task<Response<List<Qtemplates>>> GetAllDeletedTemplates()
        {
            try
            {
                await Task.Delay(1000);
                var data = this.qtmplts.GetAsQueryable().Where(x => x.QtemplatesDelStatus == true).ToList();
                return Response<List<Qtemplates>>.Success(data, "Data Found");
            }
            catch (Exception ex)
            {
                return Response<List<Qtemplates>>.Fail(new List<Qtemplates>(), ex.Message);
            }
        }

        public async Task<Response<List<Qtemplates>>> GetAllTemplates()
        {
            try
            {
                await Task.Delay(1000);
                var data = this.qtmplts.GetAsQueryable().ToList();
                return Response<List<Qtemplates>>.Success(data, "Data Found");
            }
            catch (Exception ex)
            {
                return Response<List<Qtemplates>>.Fail(new List<Qtemplates>(), ex.Message);
            }
        }

        public async Task<Response<List<Qtemplates>>> GetTemplateById(int qtemplateId)
        {
            try
            {
                await Task.Delay(1000);
                var data = this.qtmplts.GetAsQueryable().Where(x=>x.QtemplatesId == qtemplateId).ToList();
                return Response<List<Qtemplates>>.Success(data, "Data Found");
            }
            catch (Exception ex)
            {
                return Response<List<Qtemplates>>.Fail(new List<Qtemplates>(), ex.Message);
            }
        }

        public async Task<Response<List<Qtemplates>>> GetTemplateBytype(string qtemplateType)
        {
            try
            {
                await Task.Delay(1000);
                var data = this.qtmplts.GetAsQueryable().Where(x => x.QtemplatesType.ToUpper() == qtemplateType.ToUpper()).ToList();
                return Response<List<Qtemplates>>.Success(data, "Data Found");
            }
            catch (Exception ex)
            {
                return Response<List<Qtemplates>>.Fail(new List<Qtemplates>(), ex.Message);
            }
        }

        public async Task<Response<List<Qtemplates>>> InsterTemplate(Qtemplates qtemplate)
        {
            try
            {
                qtemplate.QtemplatesDelStatus = false;
                this.qtmplts.Insert(qtemplate);
                return await  GetAllActiveTemplates();
            }
            catch (Exception ex)
            {
                return Response<List<Qtemplates>>.Fail(new List<Qtemplates>(), ex.Message);
            }
        }

     

        public async Task<Response<List<Qtemplates>>> UpdateTemplate(Qtemplates qtemplate)
        {
            try
            {
                qtemplate.QtemplatesDelStatus = false;
                this.qtmplts.Update(qtemplate);
                return await  GetAllActiveTemplates();
            }
            catch (Exception ex)
            {
                return Response<List<Qtemplates>>.Fail(new List<Qtemplates>(), ex.Message);
            }
        }
    }
}
