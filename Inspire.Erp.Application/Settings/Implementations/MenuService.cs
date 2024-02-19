using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.MIS.Interfaces;
using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.AccountStatement;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Settings;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Domain.Models.Settings;
using System.Data;

namespace Inspire.Erp.Application.Settings.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IRepository<Menu> _menu;
        private readonly IUtilityService _utilityService;
        public MenuService(IUtilityService utilityService, IRepository<Menu> menu)
        {
            _utilityService = utilityService;
            _menu = menu;
        }


        private List<GetMenusResponse> AddChildMenuItems(List<GetMenusResponse> allItems, GetMenusResponse parentMenu)
        {
            var childs = allItems.Where(y=>y.ParentId==parentMenu.Id).OrderBy(x => x.Order).ToList();
            foreach (GetMenusResponse child in childs)
            {
                child.Items = new List<GetMenusResponse>();
                child.Items=this.AddChildMenuItems(allItems, child).ToList();
            }
            return childs;
        }
        public async Task<Response<List<GetMenusResponse>>> GetMenusByWorkgroupId(int id = 0)
        {
            try
            {
                List<GetMenusResponse> model = new List<GetMenusResponse>();

               // var topMenu = await _menu.GetAsQueryable().Where(x=>x.ParentId==id).ToListAsync();
                //Include(x=>x.MenusWorkgroupDetails).Where(x => x.MenusWorkgroupDetails.Where(c => c.WorkgroupId == id).Count() > 0).ToListAsync();

                var topMenu = await _menu.GetAsQueryable().Where(x => x.ParentId == id).Select(x => new GetMenusResponse()
                {
                    Action = x.Action,
                    Controller=x.Controller,
                    CreatedAt = x.CreatedAt,
                    IconClass = x.IconClass,
                    Id = x.Id,
                    IsDeleted = x.IsDeleted,
                    ParentId = x.ParentId,
                    RouteUrl = x.RouteUrl,
                    Title=x.Title,
                    Page=x.Page,
                    Order=x.Order,
                 }).OrderBy(x=>x.Order).ToListAsync();

                var allItems = await _menu.GetAsQueryable().Where(x => x.ParentId != id).Select(x => new GetMenusResponse()
                {
                    Action = x.Action,
                    Controller = x.Controller,
                    CreatedAt = x.CreatedAt,
                    IconClass = x.IconClass,
                    Id = x.Id,
                    IsDeleted = x.IsDeleted,
                    ParentId = x.ParentId,
                    RouteUrl = x.RouteUrl,
                    Title = x.Title,
                    Page = x.Page,
                    Order = x.Order,
                }).OrderBy(x => x.Order).ToListAsync();

              
                foreach (var pageItem in topMenu)
                {
                    //var item = new GetMenusResponse()
                    //{
                    //    Action = pageItem.Action,
                    //    Controller = pageItem.Controller,
                    //    CreatedAt = pageItem.CreatedAt,
                    //    IconClass = pageItem.IconClass,
                    //    Id = pageItem.Id,
                    //    IsDeleted = pageItem.IsDeleted,
                    //    ParentId = pageItem.ParentId,
                    //    RouteUrl = pageItem.RouteUrl,
                    //    Title = pageItem.Title,
                    //    Page = pageItem.Page,
                    //    Order = pageItem.Order,
                    //};

                    //allItems.Add(item);

                    //var parent = allItems.Where(pi => pi.Id == item.ParentId).SingleOrDefault();
                    //if (parent == null)
                    //{
                    //    pageItems.Add(item);
                    //}
                    //else
                    //{
                    //    if (parent.Items == null)
                    //        parent.Items = new List<GetMenusResponse>();
                    //    parent.Items.Add(item);
                    //}

                    pageItem.Items=new List<GetMenusResponse>();
                    pageItem.Items = this.AddChildMenuItems(allItems, pageItem);
                }

                return Response<List<GetMenusResponse>>.Success(topMenu, "Records FOund.");
            }
            catch (Exception ex)
            {
                return Response<List<GetMenusResponse>>.Fail(new List<GetMenusResponse>(), ex.Message);
            }

            //public IEnumerable<Location> GetChild(int id)
            //{
            //    DBEntities db = new DBEntities();
            //    var locations = db.Locations.Where(x => x.ParentLocationID == id || x.LocationID == id).ToList();

            //    var child = locations.AsEnumerable().Union(
            //                                db.Locations.AsEnumerable().Where(x => x.ParentLocationID == id).SelectMany(y => GetChild(y.LocationId))).ToList();
            //    return child;
            //}
        }
    }
}
