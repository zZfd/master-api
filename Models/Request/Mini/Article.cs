using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models.Request.Mini
{
    /// <summary>
    /// 添加，修改
    /// </summary>
    public class Article
    {
        // 修改用Id
        public Guid id { get; set; }

        public string author { get; set; }

        public DateTime time { get; set; }


        [Required(ErrorMessage = "请填写标题")]
        [StringLength(50)]
        public string title { get; set; }

        [Required(ErrorMessage = "请填写比赛")]
        [StringLength(100)]
        public string match { get; set; }

        [Required(ErrorMessage = "请填写推荐")]
        [StringLength(100)]
        public string recommend { get; set; }

        [Range(0.00,500.00)]
        // 金额 0.00代表免费
        public decimal money { get; set; }

        // 分析
        public string analysis { get; set; }

        // 结果
        public bool?  isTrue { get; set; }

        // 点赞
        public bool? preference { get; set; }

        // 收藏
        public bool? collection { get; set; }

        // 点赞
        public int preferenceCount { get; set; }

        // 收藏
        public int collectionCount { get; set; }

        public Guid attachment { get; set; }

        public string cover { get; set; }
        // 审核
        //[Required(ErrorMessage = "请选择状态")]
        //[Range(minimum: Models.Config.Status.deleted, maximum: Models.Config.Status.forbidden)]
        public short status { get; set; }

    }

    /// <summary>
    /// 查询
    /// </summary>
    public class ArtcileQuery
    {
        public ArtcileQuery()
        {
            pageIndex = 1;
            pageSize = 5;
        }

        public decimal money { get; set; }
        public Guid id { get; set; }

        public string key { get; set; }

        public Guid author { get; set; }

        public int pageIndex { get; set; }

        public int pageSize { get; set; }
    }
}