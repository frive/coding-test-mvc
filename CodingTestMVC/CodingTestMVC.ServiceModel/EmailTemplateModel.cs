using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using Modules.Business;
using Modules.Config;

namespace CodingTestMVC.ServiceModel
{
    [Route("/email/templates", "GET")]
    public class GetEmailTemplates : IReturn<EmailTemplatesResponse>
    {
        public int PageSize { get; set; }
        public EmailSortBy SortBy { get; set; }
        public int Page { get; set; }
    }

    public class EmailTemplatesResponse
    {
        public EmailTemplates templates { get; set; }
        public int total { get; set; }
    }
}