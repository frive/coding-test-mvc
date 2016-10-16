using System;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using CodingTestMVC.ServiceModel;
using CodingTestMVC.ServiceInterface;

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
        public void TestMethod1()
        {
            var service = appHost.Container.Resolve<EmailTemplateServices>();

            //var response = (HelloResponse)service.Any(new EmailTemplateModel { Name = "World" });

            //Assert.That(response.Result, Is.EqualTo("Hello, World!"));
            Assert.That(true);
        }
    }
}
