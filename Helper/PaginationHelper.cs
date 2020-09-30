using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 继承List泛型
    /// 添加4个属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginationHelper<T> : List<T>
    {
        /// <summary>
        /// 总条目数
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 当前单页条数
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; private set; }

        public PaginationHelper(List<T> result, int total, int pageIndex, int pageSize)
        {
            Total = total;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            AddRange(result);
        }

        /// <summary>
        /// 创建分页结果
        /// 返回分页对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="PI"></param>
        /// <param name="PS"></param>
        /// <returns></returns>
        public static PaginationHelper<T> Paging(IQueryable<T> source, int pi = 1, int ps = 10)
        {
            if (source.Any())
            {
                List<T> result = source.Skip((pi - 1) * ps).Take(ps).ToList();
                return new PaginationHelper<T>(result, source.Count(), pi, ps);
            }
            return null;
        }
    }
}
