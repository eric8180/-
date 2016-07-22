using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCDemo.Models
{
    public static class MyHtmlHelperExt
    {
        #region 分页
        /// <summary>
        /// 创建分页链接
        /// </summary>
        /// <param name="helper">HtmlHelper类</param>
        /// <param name="startPage">开始页 (多数情况下是 1)</param>
        /// <param name="currentPage">当前页</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="pagesToShow">前后显示的页数</param>
        public static MvcHtmlString Pager(this HtmlHelper helper, int startPage,
            int currentPage, int totalPages, int pagesToShow)
        {
            RouteData routeData = helper.ViewContext.RouteData;
            //你可能还要获取action
            //routeData.Values["action"].ToString();
            string controller = routeData.Values["controller"].ToString();
            string action = routeData.Values["action"].ToString();
            StringBuilder html = new StringBuilder();
            //创建从第一页到最后一页的列表
            html = Enumerable.Range(startPage, totalPages)
            .Where(i => (currentPage - pagesToShow) < i & i < (currentPage + pagesToShow))
            .Aggregate(new StringBuilder(@"<div class=""pagination""><span>共" + totalPages + "页</span>"), (seed, page) =>
            {
                //当前页
                if (page == currentPage)
                    seed.AppendFormat("<span style=\"font-weight:bold;\">_{0}_</span>", page);
                else
                {
                    //第一页时显示：domain/archives
                    if (page == 1)
                    {
                        seed.AppendFormat("<a href=\"/{0}\"> {1}</a>", controller.ToLower() + "/" + action.ToLower(), page);
                    }
                    else
                    {
                        seed.AppendFormat("<a href=\"/{0}/{1}\"> {1}</a>", controller.ToLower() + "/" + action.ToLower(), page);
                    }
                }
                return seed;
            });
            html.Append(@"</div>");
            return MvcHtmlString.Create(html.ToString());
        }
        #endregion
    }
}