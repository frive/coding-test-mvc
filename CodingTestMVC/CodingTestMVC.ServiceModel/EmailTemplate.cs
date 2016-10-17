using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Hosting;
using System.Web.Caching;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using Modules.Config;
using Modules.Business;
using System.Xml.Serialization;
using System.Threading;

namespace Modules.Business
{

    /// <summary>
    /// This class represents the actual template and its associated properties.  
    /// Some of the properties are simple types, and some complex.  
    /// This class takes care of initializing properties, loading individual templates, 
    /// and performing basic tasks, such as saving new versions.  
    /// Each EmailTemplate has a parent that acts as the primary record, and versions to view an audit trail and revision history.
    /// </summary>
    [Serializable]
    public class EmailTemplate
    {

        #region Object Constructors

        /// <summary>
        /// Default Constructor, performs basic initialization
        /// </summary>
        public EmailTemplate()
        {
            Init();
        }

        /// <summary>
        /// Initializes the EmailTemplate based on provided Guid, loads EmailTemplate versions
        /// </summary>
        /// <param name="emailTemplateId"></param>
        /// <param name="loadDrafts"></param>
        public EmailTemplate(Guid emailTemplateId)
        {
            Init();
            Id = emailTemplateId;
            Load();
        }

        #endregion

        #region Support Methods
        /// <summary>
        /// Basic initialization of necessary variables.
        /// </summary>
        private void Init()
        {
            Id = Guid.Empty;
        }
        #endregion

        #region Load Methods
        /// <summary>
        /// Performs a basic load with nothing specified, will pull the data from the DB
        /// </summary>
        private void Load()
        {
            EmailTemplate template = EmailTemplates.AllTemplates.Find(x => x.Id == Id);
            if (template != null)
            {
                Id = template.Id;
                VersionCount = template.VersionCount;
                versions = template.Versions;
                EmailLabel = template.EmailLabel;
                FromAddress = template.FromAddress;
                TemplateText = template.TemplateText;
                EmailType = template.EmailType;
                Active = template.Active;
                DateUpdated = template.DateUpdated;
                Subject = template.Subject;
                ParentId = template.ParentId;
                IsDefault = template.IsDefault;
                BccAddress = template.BccAddress;
            }
        }

        /// <summary>
        /// Performs a reload of the versions
        /// </summary>
        private void ReloadDrafts()
        {
            if (Id != Guid.Empty && !IsDraft)
            {
                versions = new EmailTemplates();
                versions.AddRange(new EmailTemplate(Id).Versions);
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Provides a single blank EmailTemplate
        /// </summary>
        public static EmailTemplate BlankTemplate
        {
            get
            {
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Id = new Guid();
                emailTemplate.Subject = "Missing Default Template";
                emailTemplate.TemplateText = "Missing Default Template";
                emailTemplate.FromAddress = "webmaster@opm.gov";
                emailTemplate.EmailLabel = "Missing Default Template";
                emailTemplate.Active = true;
                return emailTemplate;
            }
        }
        /// <summary>
        /// Id of Template
        /// </summary>
        public Guid Id { get; set; }
        public string EmailLabel { get; set; }
        public string EmailLabelForDropdown
        {
            get
            {
                if (IsDefault)
                    return "Default Template";
                return EmailLabel;
            }
        }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string TemplateText { get; set; }
        public Config.EmailType EmailType { get; set; }
        public bool Active { get; set; }
        public string DateUpdated { get; set; }
        public bool LoadDrafts { get; set; }
        public Guid ParentId { get; set; }
        public int VersionCount { get; set; }
        public bool IsDefault { get; set; }
        public string BccAddress { get; set; }

        public bool IsDraft
        {
            get { return Id != ParentId; }
        }

        /// <summary>
        /// Determines if another EmailTemplate with the same name exists
        /// </summary>
        public bool IsNameValid
        {
            get
            {
                if (string.IsNullOrEmpty(EmailLabel))
                    return false;
                //Data.EmailTemplate dataTemplate = new Santantonio.Modules.Data.EmailTemplate();
                //return dataTemplate.IsNameValid(Id, (int)EmailType, EmailLabel);
                return true;
            }
        }

        private EmailTemplates versions;
        /// <summary>
        /// Collection of previous versions of the current EmailTemplate
        /// </summary>
        public EmailTemplates Versions
        {
            get
            {
                if (versions == null)
                {
                    versions = new EmailTemplates();
                    LoadDrafts = true;
                    ReloadDrafts();
                }
                return versions;
            }
            set { versions = value; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a collection of EmailTemplate
    /// </summary>
    public class EmailTemplates : List<EmailTemplate>
    {

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public EmailTemplates()
        {
            Init();
        }
        #endregion

        #region Support Methods
        /// <summary>
        /// Basic initialization of necessary variables.
        /// </summary>
        private void Init()
        {

        }
        #endregion

        #region Load Methods
        /// <summary>
        /// Loads the object from the database based on the inialized properties for the class
        /// </summary>
        public void Load()
        {

            EmailTemplates allTemplates = AllTemplates;

            foreach (EmailTemplate template in allTemplates)
            {
                this.Add(template);
            }

            if (SortBy != null)
            {
                switch (SortBy)
                {
                    case EmailSortBy.EmailLabelAscending:
                        this.Sort((x, y) => x.EmailLabel.CompareTo(y.EmailLabel));
                        break;
                    case EmailSortBy.EmailLabelDescending:
                        this.Sort((x, y) => y.EmailLabel.CompareTo(x.EmailLabel));
                        break;
                    case EmailSortBy.DateUpdatedAscending:
                        this.Sort((x, y) => x.DateUpdated.CompareTo(y.DateUpdated));
                        break;
                    case EmailSortBy.DateUpdatedDescending:
                        this.Sort((x, y) => y.DateUpdated.CompareTo(x.DateUpdated));
                        break;
                    case EmailSortBy.FromAddressAscending:
                        this.Sort((x, y) => x.FromAddress.CompareTo(y.FromAddress));
                        break;
                    case EmailSortBy.FromAddressDescending:
                        this.Sort((x, y) => y.FromAddress.CompareTo(x.FromAddress));
                        break;
                }
            }

            if (Active != null)
            {
                this.RemoveAll(x => x.Active != (bool)Active);
            }

            if (EmailType != null)
            {
                this.RemoveAll(x => x.EmailType != (Config.EmailType)EmailType);
            }

            this.TotalCount = this.Count;
            
            if (RecsPerPage != null && this.Count > RecsPerPage)
            {
                if (Page == null)
                    Page = 1;
                Pages = (int)Math.Ceiling((decimal)this.Count / (decimal)RecsPerPage);
                int startRecord = (int)RecsPerPage * ((int)Page - 1);
                int endRecord = startRecord + (int)RecsPerPage;
                if (endRecord < this.Count)
                    this.RemoveRange(endRecord, this.Count - endRecord);
                if (startRecord > 0)
                    this.RemoveRange(0, startRecord);

            }
            else
            {
                Pages = 1;
            }


        }

        /// <summary>
        /// Initializes properties based on a EmailTemplatePageCriteria object
        /// </summary>
        /// <param name="pageCriteria"></param>
        public void LoadCriteria(EmailTemplatePageCriteria pageCriteria)
        {
            Page = pageCriteria.Page;
            RecsPerPage = pageCriteria.PageSize;
            SortBy = pageCriteria.SortBy;
            EmailType = pageCriteria.EmailType;
            Active = pageCriteria.Active;
        }
        #endregion

        #region Properties

        public int? Page { get; set; }
        public int? RecsPerPage { get; set; }
        public EmailSortBy? SortBy { get; set; }
        public bool? Active { get; set; }
        public Config.EmailType? EmailType { get; set; }
        public bool? LoadDefault { get; set; }


        public int Pages { get; set; }
        public int TotalCount { get; set; }

        #endregion

        #region Static Methods
        /// <summary>
        /// Retrieves a single page based on provided search criteria
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="page"></param>
        /// <param name="recsPerPage"></param>
        /// <param name="active"></param>
        /// <param name="searchTerm"></param>
        /// <param name="userId"></param>
        /// <param name="groups"></param>
        /// <param name="emailType"></param>
        /// <returns></returns>
        public static EmailTemplates GetAll(EmailSortBy sortBy, int page, int recsPerPage, bool active, Config.EmailType emailType)
        {
            EmailTemplates emailTemplates = new EmailTemplates();
            emailTemplates.SortBy = sortBy;
            emailTemplates.Page = page;
            emailTemplates.RecsPerPage = recsPerPage;
            emailTemplates.Active = active;
            emailTemplates.EmailType = emailType;
            emailTemplates.Load();
            return emailTemplates;
        }

        /// <summary>
        /// Retrieves the default EmailTemplate for a specific email type
        /// </summary>
        /// <param name="emailType"></param>
        /// <returns></returns>
        public static EmailTemplate GetDefault(Config.EmailType emailType)
        {
            EmailTemplates emailTemplates = new EmailTemplates();
            emailTemplates.SortBy = EmailSortBy.DateUpdatedDescending;
            emailTemplates.Page = 1;
            emailTemplates.RecsPerPage = 1;
            emailTemplates.Active = true;
            emailTemplates.EmailType = emailType;
            emailTemplates.LoadDefault = true;
            emailTemplates.Load();
            if (emailTemplates.Count > 0)
                return emailTemplates[0];
            return EmailTemplate.BlankTemplate;
        }

        /// <summary>
        /// Retieve all associated templates based on provided criteria
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groups"></param>
        /// <param name="emailType"></param>
        /// <returns></returns>
        public static EmailTemplates GetAll(int userId, Config.EmailType emailType)
        {
            EmailTemplates emailTemplates = new EmailTemplates();
            emailTemplates.EmailType = emailType;
            emailTemplates.Load();
            return emailTemplates;
        }

        #endregion

        #region Deserialize

        public static EmailTemplates AllTemplates
        {
            get
            {
                EmailTemplates allTemplates = (EmailTemplates)HttpContext.Current.Cache["EmailTemplatesData"];
                if (allTemplates == null)
                {

                    //string fileName = ConfigurationManager.AppSettings["DataPath"].Replace("~", HostingEnvironment.ApplicationPhysicalPath).Replace("/", "\\");
                    string fileName = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["DataPath"]);
                    XmlSerializer serializer = new XmlSerializer(typeof(EmailTemplates));
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    allTemplates = (EmailTemplates)serializer.Deserialize(fs);
                    fs.Close();
                    HttpContext.Current.Cache.Insert("EmailTemplatesData", allTemplates, new CacheDependency(fileName));
                }
                return allTemplates;
            }
        }

        #endregion


    }

    /// <summary>
    /// Basic search criteria for use in managing search options on Presentation Layer
    /// </summary>
    [Serializable]
    public class EmailTemplatePageCriteria
    {

        #region Constructors 

        /// <summary>
        /// Default Constructor
        /// </summary>
        public EmailTemplatePageCriteria()
        {
            Init();
        }

        #endregion

        #region Support Methods
        /// <summary>
        /// Basic initialization of necessary variables.
        /// </summary>
        private void Init()
        {
            EmailType = null;
            Active = true;
            Page = 1;
            PageSize = 25;
            SortBy = EmailSortBy.EmailLabelAscending;
        }
        #endregion

        #region Properties
        public int PageSize { get; set; }
        public int Page { get; set; }
        public EmailSortBy SortBy { get; set; }
        public bool Active { get; set; }
        public EmailType? EmailType { get; set; }

        #endregion
    }
}

namespace Modules.Config
{

    public enum EmailSortBy
    {
        EmailLabelAscending,
        EmailLabelDescending,
        DateUpdatedAscending,
        DateUpdatedDescending,
        FromAddressAscending,
        FromAddressDescending
    }

    /// <summary>
    /// Represents the various EmailTypes
    /// </summary>
    public enum EmailType
    {
        WelcomeEmail,
        AccountActive,
        ConfirmAccount,
        ConfirmPasswordRequest,
        NewPassword,
        AdminApproval,
        AdminUnlockRequest,
        UnlockNotify,
        GeneralEmail
    };

}
