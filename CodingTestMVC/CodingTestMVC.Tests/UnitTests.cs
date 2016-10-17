using System;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using CodingTestMVC.ServiceModel;
using CodingTestMVC.ServiceInterface;
using Modules.Business;

namespace CodingTestMVC.Tests
{
    [TestFixture]
    public class UnitTests
    {
        private readonly ServiceStackHost appHost;

        public UnitTests()
        {
            appHost = new BasicAppHost(typeof(EmailTemplateServices).Assembly)
            {
                ConfigureContainer = container =>
                {
                    //Add your IoC dependencies here
                }
            }
            .Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        [Test]
        public void get_emailtemplates()
        {
            var service = appHost.Container.Resolve<EmailTemplateServices>();

            var response = (EmailTemplatesResponse)service.Get(
                new GetEmailTemplates { PageSize = 10, SortBy = 0, Page = 1 });
            
            Assert.NotNull(response.templates[0]);
            Assert.NotNull(response.total);
        }
    }
}
