using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using Modules.Business;
using Modules.Config;

namespace CodingTestMVC.ServiceModel
{
    /// <summary>
    /// Get email templates request
    /// </summary>
    [Route("/email/templates", "GET")]
    public class GetEmailTemplates : IReturn<EmailTemplatesResponse>
    {
        public int PageSize { get; set; }
        public EmailSortBy SortBy { get; set; }
        public int Page { get; set; }
    }

    /// <summary>
    /// Email templates response
    /// </summary>
    public class EmailTemplatesResponse
    {
        public EmailTemplates templates { get; set; }
        public int total { get; set; }
    }
}