using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Mini
{
    /// <summary>
    /// 保存
    /// </summary>
    public class Order
    {
        [Required(ErrorMessage = "请填写用户")]
        public Guid member { get; set; }
        [Required(ErrorMessage = "请选择文章")]
        public Guid article { get; set; }
        [Required(ErrorMessage = "请填写金额")]
        public decimal money { get; set; }
    }

    /// <summary>
    /// 修改
    /// </summary>
    public class OrderUpdate {
        [Required(ErrorMessage = "请选择订单")]
        public Guid id { get; set; }

        [Required(ErrorMessage = "请设置状态")]
        [Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]
        public short status { get; set; }
    }

    /// <summary>
    /// 筛选
    /// </summary>
    public class ListOrder
    {
        public ListOrder()
        {

            pageIndex = 1;
            pageSize = 10;
        }

        public string author { get; set; }

        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }


        public short status { get; set; }


        public int pageIndex { get; set; }

        public int pageSize { get; set; }
    }

}