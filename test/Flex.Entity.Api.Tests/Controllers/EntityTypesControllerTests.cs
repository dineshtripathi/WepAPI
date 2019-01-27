using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Flex.Entity.Api.Controllers;
using Flex.Entity.Api.Model;
using Flex.Entity.Repository;
using Flex.Logging.Container;
using Moq;
using NUnit.Framework;

namespace Flex.Entity.Api.Tests.Controllers
{
    [TestFixture]
    [Category("UnitTest")]
    public class EntityTypesControllerTests
    {
        [SetUp]
        public void Init()
        {
            _database = new List<EntityType>
            {
                new EntityType
                {
                    prefix = "C",
                    name = "Client",
                    allow_in_asset_hierarchy = true,
                    allow_in_service_hierarchy = false,
                    allow_same_type_descendant = false,
                }
                ,
                new EntityType
                {
                    prefix = "S",
                    name = "Site",
                    allow_in_asset_hierarchy = true,
                    allow_in_service_hierarchy = false,
                    allow_same_type_descendant = false,
                }
            };
        }

        private List<EntityType> _database;

        private static Mock<UrlHelper> MockUrlHelper(string locationUrl)
        {
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(locationUrl);
            return mockUrlHelper;
        }
        private static Mock<ILogger> MockLogger()
        {
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.LoggerName).Returns("MyTestLogger").Verifiable();
            mockLogger.Setup(x => x.Debug(It.IsAny<string>())).Verifiable();
            mockLogger.Setup(x => x.Trace(It.IsAny<string>())).Verifiable();
            mockLogger.Setup(x => x.Error(It.IsAny<string>())).Verifiable();
            mockLogger.Setup(x => x.Error(It.IsAny<string>())).Verifiable();
            return mockLogger;
        }


        [Test(Author = "Nooruddin Kapasi"
            , Description = "Adding duplicate EntityType(prefix/name) not allowed."
            )]
        public async Task Create_Duplicate_EntityType_Causes_Constraint_Exception_Test()
        {
            var prefix = "D";
            var entityTypeToAdd = new EntityType
            {
                prefix = prefix,
                name = "Device",
                allow_in_asset_hierarchy = true,
                allow_in_service_hierarchy = false,
                allow_same_type_descendant = false,
            };
            var mockDbException = new Mock<DbException>(DbException.ErrorReason.Configuration, "", 0);
            mockDbException.SetupGet(p => p.Message).Returns("Constraint Violation");
            mockDbException.SetupGet(p => p.Reason).Returns(DbException.ErrorReason.ConstraintViolation);
            

            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == prefix), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null);

            mockEntityTypeRepo.Setup(s => s.CreateAsync(It.IsAny<EntityType>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(mockDbException.Object);

            var mockUrlHelper = MockUrlHelper($"http://localhost/entities/types/{prefix}");



            var cut = new EntityTypesController(mockEntityTypeRepo.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration(),
                Url = mockUrlHelper.Object,
                Logger = MockLogger().Object
            };

            var actionResult = await cut.CreateAsync(new CancellationToken(), entityTypeToAdd);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.StatusCode, "Response Code");
            Assert.IsFalse(actual.Content.success, "Success Check");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }

        [Test(Author = "Nooruddin Kapasi"
            , Description = "Adding duplicate EntityType not allowed."
            )]
        public async Task Create_Duplicate_EntityType_NotAllowed_Test()
        {
            var prefix = "D";
            var entityTypeToAdd = new EntityType
            {
                prefix = prefix,
                name = "Device",
                allow_in_asset_hierarchy = true,
                allow_in_service_hierarchy = false,
                allow_same_type_descendant = false,
            };
            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == prefix), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entityTypeToAdd);
            mockEntityTypeRepo.Setup(s => s.CreateAsync(It.IsAny<EntityType>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var mockUrlHelper = MockUrlHelper($"http://localhost/entities/types/{prefix}");

            var cut = new EntityTypesController(mockEntityTypeRepo.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration(),
                Url = mockUrlHelper.Object,
                Logger = MockLogger().Object
            };

            var actionResult = await cut.CreateAsync(new CancellationToken(), entityTypeToAdd);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.StatusCode, "Response Code");
            Assert.IsFalse(actual.Content.success, "Success Check");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }

        [Test(Author = "Nooruddin Kapasi"
            , Description = "Add an EntityType."
            )]
        public async Task Create_EntityType_Succesfully_Test()
        {
            var prefix = "D";
            var entityTypeToAdd = new EntityType
            {
                prefix = prefix,
                name = "Device",
                allow_in_asset_hierarchy = true,
                allow_in_service_hierarchy = false,
                allow_same_type_descendant = false,
            };
            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == prefix), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null);
            mockEntityTypeRepo.Setup(s => s.CreateAsync(It.IsAny<EntityType>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var mockUrlHelper = MockUrlHelper($"http://localhost/entities/types/{prefix}");

            var cut = new EntityTypesController(mockEntityTypeRepo.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration(),
                Url = mockUrlHelper.Object,
                Logger = MockLogger().Object
            };

            var actionResult = await cut.CreateAsync(new CancellationToken(), entityTypeToAdd);
            var actual = actionResult as CreatedNegotiatedContentResult<EntityType>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(entityTypeToAdd.prefix, actual.Content.prefix, "Prefix returned");
        }

        [Test(Author = "Nooruddin Kapasi"
            ,
            Description =
                "Dont allow deleting an EntityType with the specified prefix if it has entities associated with it."
            )]
        public async Task Delete_EntityType_With_Supplied_Prefix_Has_Entities_Test()
        {
            var prefix = "D";

            var mockDbException = new Mock<DbException>(DbException.ErrorReason.Configuration, "", 0);
            mockDbException.SetupGet(p => p.Message).Returns("Constraint Violation");
            mockDbException.SetupGet(p => p.Reason).Returns(DbException.ErrorReason.ConstraintViolation);
            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();

            mockEntityTypeRepo.Setup(s => s.DeleteAsync(It.Is<string>(p => p == prefix), It.IsAny<CancellationToken>()))
                .ThrowsAsync(mockDbException.Object);
            var cut = new EntityTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var actionResult = await cut.DeleteAsync(new CancellationToken(), prefix);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.StatusCode, "Response Code");
            Assert.IsFalse(actual.Content.success, "Success Check");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }

        [Test(Author = "Nooruddin Kapasi"
            , Description = "Deleting an EntityType with the specified prefix raises an error if it is not found."
            )]
        public async Task Delete_EntityType_With_Supplied_Prefix_NotFound_Test()
        {
            var prefix = "D";

            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.DeleteAsync(It.Is<string>(p => p == prefix), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            var cut = new EntityTypesController(mockEntityTypeRepo.Object) {Logger = MockLogger().Object};

            var actionResult = await cut.DeleteAsync(new CancellationToken(), prefix);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(HttpStatusCode.NotFound, actual.StatusCode, "Response Code");
            Assert.IsFalse(actual.Content.success, "Success Check");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }


        [Test(Author = "Nooruddin Kapasi"
            , Description = "Deleting an exisiting EntityType with the specified prefix succeeds."
            )]
        public async Task Delete_EntityType_With_Supplied_Prefix_Test()
        {
            var prefix = "D";
            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.DeleteAsync(It.Is<string>(p => p == prefix), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var cut = new EntityTypesController(mockEntityTypeRepo.Object) {Logger = MockLogger().Object};
            var actionResult = await cut.DeleteAsync(new CancellationToken(), prefix);
            var actual = actionResult as OkNegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "ResponseCode OK");
            Assert.IsTrue(actual.Content.success, "Success Check");
            Assert.IsTrue(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }

        [Test(Author = "Nooruddin Kapasi"
            , Description = "Get method fails with empty database."
            )]
        public async Task Get_All_EmptyDb_Test()
        {
            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EntityType>().AsEnumerable());

            var cut = new EntityTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var collection = await cut.GetAllAsync(new CancellationToken());
            var actual = collection as OkNegotiatedContentResult<IEnumerable<EntityType>>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.IsEmpty(actual.Content, "IsEmpty Collection");
        }

        [Test(Author = "Nooruddin Kapasi"
            , Description = "Get method returns all the entitytypes defined in the database."
            )]
        public async Task Get_All_Test()
        {
            var expected = _database.AsEnumerable();
            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_database.AsEnumerable());

            var cut = new EntityTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var actionResult = await cut.GetAllAsync(new CancellationToken());
            var actual = actionResult as OkNegotiatedContentResult<IEnumerable<EntityType>>;
            Assert.IsNotNull(actual, "Has Content");
            CollectionAssert.AreEquivalent(expected, actual.Content, "Returned Collections AreEquivalent");
        }

        [Test(Author = "Nooruddin Kapasi"
            , Description = "Get method returns 404 when supplied prefix not found."
            )]
        public async Task Get_EntityType_By_Prefix_Not_Found_Test()
        {
            var prefix = "S";
            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == "S"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null);
        
            var cut = new EntityTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var actionResult = await cut.GetSingleAsync(new CancellationToken(), prefix);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "ActionResult");
            Assert.AreEqual(HttpStatusCode.NotFound, actual.StatusCode, "NotFoundStatusCode");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error message present");
        }

        [Test(Author = "Nooruddin Kapasi"
            , Description = "Get an entitytype that matches the supplied prefix."
            )]
        public async Task Get_EntityType_By_Prefix_Test()
        {
            var expected = _database.Single(e => e.prefix == "C");
            var mockEntityTypeRepo = new Mock<IEntityTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == "C"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityType
                {
                    prefix = "C",
                    name = "Client",
                    allow_in_asset_hierarchy = true,
                    allow_in_service_hierarchy = false,
                    allow_same_type_descendant = false,
                }
                );

            var cut = new EntityTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var actionResult = await cut.GetSingleAsync(new CancellationToken(), "C");
            var actual = actionResult as OkNegotiatedContentResult<EntityType>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(expected.prefix, actual.Content.prefix, "Prefix returned");
        }
    }
}