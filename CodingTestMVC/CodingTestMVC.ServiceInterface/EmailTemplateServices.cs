using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using CodingTestMVC.ServiceModel;
using Modules.Business;
using Modules.Config;

namespace CodingTestMVC.ServiceInterface
{
    public class EmailTemplateServices : Service
    {
        /// <summary>
        /// Get email templates based on request parameters
        /// </summary>
        /// <param name="request">Request parameters</param>
        /// <returns>EmailTemplatesResponse</returns>
        public object Get(GetEmailTemplates request)
        {
            EmailTemplatePageCriteria pageCriteria = new EmailTemplatePageCriteria();
            pageCriteria.PageSize = request.PageSize == 0 ? 10 : request.PageSize;
            pageCriteria.SortBy = request.SortBy;
            pageCriteria.Page = request.Page == 0 ? 1 : request.Page;

            EmailTemplates templates = new EmailTemplates();
            templates.LoadCriteria(pageCriteria);
            templates.Load();

            return new EmailTemplatesResponse { templates = templates, total = templates.TotalCount };
        }

        public object Any(FallbackForClientRoutes request)
        {
            //Return default.cshtml for unmatched requests so routing is handled on the client
            return new HttpResult
            {
                View = "/default.cshtml"
            };
        }
    }

    [FallbackRoute("/{PathInfo*}")]
    public class FallbackForClientRoutes
    {
        public string PathInfo { get; set; }
    }
}