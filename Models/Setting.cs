using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public static class Setting
    {
       public enum NormalStauts
        {
            正常 = 0,
            禁用 = 1,
            已删除 = 2
        }

        public static List<SettingModel> normalStatus = new List<SettingModel>
        {
            new SettingModel{key = "0",val = "正常"},
            new SettingModel{key = "1",val = "禁用"},
            new SettingModel {key = "-1",val = "已删除"}
        };
        /// <summary>
        /// 菜单类型
        /// </summary>
        public static List<SettingModel> menuType = new List<SettingModel>
        {
            new SettingModel { key = "ROOT",val = "根节点"  },
            new SettingModel { key = "MENU-1",val = "一级菜单"  },
            new SettingModel { key = "MENU-2",val = "二级菜单"  },
            new SettingModel { key = "PAGE",val = "页面"  },
            new SettingModel { key = "BUTTON",val = "按钮"  }
        };
    }
    public class SettingModel
    {
        public string key { get; set; }

        public string val { get; set; }
    }
}
