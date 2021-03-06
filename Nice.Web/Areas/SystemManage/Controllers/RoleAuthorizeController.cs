﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nice.Common.Web.TreeView;
using Nice.Domain.Entity.SystemManage;
using Nice.Service.SystemManage;


namespace Nice.WebPc.Areas.SystemManage.Controllers
{
    public class RoleAuthorizeController : Nice.WebPc.Handler.ControllerBase
    {
        private RoleAuthorizeService _roleAuthorizeService = new RoleAuthorizeService();
        private ModuleService _moduleService = new ModuleService();
        private ModuleButtonService _moduleButtonService = new ModuleButtonService();

        public ActionResult GetPermissionTree(string roleId)
        {
            var moduledata = _moduleService.GetList();
            var buttondata = _moduleButtonService.GetList();
            var authorizedata = new List<RoleAuthorizeBaseEntity>();
            if (!string.IsNullOrEmpty(roleId))
            {
                authorizedata = _roleAuthorizeService.GetList(roleId);
            }
            var treeList = new List<TreeViewModel>();
            foreach (ModuleBaseEntity item in moduledata)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = moduledata.Count(t => t.F_ParentId == item.F_Id) == 0 ? false : true;
                tree.id = item.F_Id;
                tree.text = item.F_FullName;
                tree.value = item.F_EnCode;
                tree.parentId = item.F_ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.showcheck = true;
                tree.checkstate = authorizedata.Count(t => t.F_ItemId == item.F_Id);
                tree.hasChildren = true;
                tree.img = item.F_Icon == "" ? "" : item.F_Icon;
                treeList.Add(tree);
            }
            foreach (ModuleButtonBaseEntity item in buttondata)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = buttondata.Count(t => t.F_ParentId == item.F_Id) == 0 ? false : true;
                tree.id = item.F_Id;
                tree.text = item.F_FullName;
                tree.value = item.F_EnCode;
                tree.parentId = item.F_ParentId == "0" ? item.F_ModuleId : item.F_ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.showcheck = true;
                tree.checkstate = authorizedata.Count(t => t.F_ItemId == item.F_Id);
                tree.hasChildren = hasChildren;
                tree.img = item.F_Icon == "" ? "" : item.F_Icon;
                treeList.Add(tree);
            }
            return Content(treeList.TreeViewJson());
        }
    }
}
