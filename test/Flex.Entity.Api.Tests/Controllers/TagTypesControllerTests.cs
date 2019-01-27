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
    public class TagTypesControllerTests
    {
        [SetUp]
        public void Init()
        {
            _database = new List<Model.TagType>
            {
                new Model.TagType
                {
                   key = "hostId"
                },
                new Model.TagType
                {
                   key = "ipaddress"
                },
                new Model.TagType
                {
                   key = "lannumber"
                },
                new Model.TagType
                {
                   key = "bmsno"
                },
            };
        }

        private List<Model.TagType> _database;

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


        [Test(Author = "Nooruddin Kapasi", Description = "Adding duplicate TagType(key/name) not allowed.")]
        public async Task Create_Duplicate_TagType_Causes_Constraint_Exception_Test()
        {
            var key = "controller";
            var tagTypeToAdd = new Model.TagType
            {
                key = key
            };
            var mockDbException = new Mock<DbException>(DbException.ErrorReason.Configuration, "", 0);
            mockDbException.SetupGet(p => p.Message).Returns("Constraint Violation");
            mockDbException.SetupGet(p => p.Reason).Returns(DbException.ErrorReason.ConstraintViolation);

            var mockTagTypeRepo = new Mock<ITagTypeRepository>();
            mockTagTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == key), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null);

            mockTagTypeRepo.Setup(s => s.CreateAsync(It.IsAny<Model.TagType>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(mockDbException.Object);

            var mockUrlHelper = MockUrlHelper($"http://localhost/entities/tags/{key}");

            var cut = new TagTypesController(mockTagTypeRepo.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration(),
                Url = mockUrlHelper.Object,
                Logger = MockLogger().Object
            };

            var actionResult = await cut.CreateAsync(new CancellationToken(), tagTypeToAdd);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.StatusCode, "Response Code");
            Assert.IsFalse(actual.Content.success, "Success Check");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }

        [Test(Author = "Nooruddin Kapasi", Description = "Adding duplicate TagType not allowed.")]
        public async Task Create_Duplicate_TagType_NotAllowed_Test()
        {
            var key = "controller";
            var tagTypeToAdd = new Model.TagType
            {
                key = key
            };
            var mockEntityTypeRepo = new Mock<ITagTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == key), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tagTypeToAdd);
            mockEntityTypeRepo.Setup(s => s.CreateAsync(It.IsAny<Model.TagType>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var mockUrlHelper = MockUrlHelper($"http://localhost/entities/tags/{key}");

            var cut = new TagTypesController(mockEntityTypeRepo.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration(),
                Url = mockUrlHelper.Object,
                Logger = MockLogger().Object
            };

            var actionResult = await cut.CreateAsync(new CancellationToken(), tagTypeToAdd);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.StatusCode, "Response Code");
            Assert.IsFalse(actual.Content.success, "Success Check");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }

        [Test(Author = "Nooruddin Kapasi", Description = "Add an TagType.")]
        public async Task Create_TagType_Succesfully_Test()
        {
            var key = "controller";
            var tagTypeToAdd = new Model.TagType
            {
                key = key
            };
            var mockEntityTypeRepo = new Mock<ITagTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == key), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null);
            mockEntityTypeRepo.Setup(s => s.CreateAsync(It.IsAny<Model.TagType>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var mockUrlHelper = MockUrlHelper($"http://localhost/entities/types/{key}");

            var cut = new TagTypesController(mockEntityTypeRepo.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration(),
                Url = mockUrlHelper.Object,
                Logger = MockLogger().Object
            };

            var actionResult = await cut.CreateAsync(new CancellationToken(), tagTypeToAdd);
            var actual = actionResult as CreatedNegotiatedContentResult<Model.TagType>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(tagTypeToAdd.key, actual.Content.key, "Prefix returned");
        }

        [Test(Author = "Nooruddin Kapasi",Description ="Dont allow deleting an EntityType with the specified prefix if it has entities associated with it.")]
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

        [Test(Author = "Nooruddin Kapasi", Description = "Deleting an TagType with the specified key raises an error if it is not found.")]
        public async Task Delete_EntityType_With_Supplied_Prefix_NotFound_Test()
        {
            var key = "controller";

            var mockEntityTypeRepo = new Mock<ITagTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.DeleteAsync(It.Is<string>(p => p == key), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            var cut = new TagTypesController(mockEntityTypeRepo.Object) {Logger = MockLogger().Object};

            var actionResult = await cut.DeleteAsync(new CancellationToken(), key);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(HttpStatusCode.NotFound, actual.StatusCode, "Response Code");
            Assert.IsFalse(actual.Content.success, "Success Check");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }


        [Test(Author = "Nooruddin Kapasi", Description = "Deleting an exisiting EntityType with the specified prefix succeeds.")]
        public async Task Delete_EntityType_With_Supplied_Prefix_Test()
        {
            var key = "D";
            var mockEntityTypeRepo = new Mock<ITagTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.DeleteAsync(It.Is<string>(p => p == key), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var cut = new TagTypesController(mockEntityTypeRepo.Object) {Logger = MockLogger().Object};
            var actionResult = await cut.DeleteAsync(new CancellationToken(), key);
            var actual = actionResult as OkNegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "ResponseCode OK");
            Assert.IsTrue(actual.Content.success, "Success Check");
            Assert.IsTrue(string.IsNullOrWhiteSpace(actual.Content.error), "Error Content Check");
        }

        [Test(Author = "Nooruddin Kapasi", Description = "Get method fails with empty database.")]
        public async Task Get_All_EmptyDb_Test()
        {
            var mockEntityTypeRepo = new Mock<ITagTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Model.TagType>().AsEnumerable());

            var cut = new TagTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var collection = await cut.GetAllAsync(new CancellationToken());
            var actual = collection as OkNegotiatedContentResult<IEnumerable<Model.TagType>>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.IsEmpty(actual.Content, "IsEmpty Collection");
        }

        [Test(Author = "Nooruddin Kapasi", Description = "Get method returns all the entitytypes defined in the database.")]
        public async Task Get_All_Test()
        {
            var expected = _database.AsEnumerable();
            var mockEntityTypeRepo = new Mock<ITagTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_database.AsEnumerable());

            var cut = new TagTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var actionResult = await cut.GetAllAsync(new CancellationToken());
            var actual = actionResult as OkNegotiatedContentResult<IEnumerable<Model.TagType>>;
            Assert.IsNotNull(actual, "Has Content");
            CollectionAssert.AreEquivalent(expected, actual.Content, "Returned Collections AreEquivalent");
        }

        [Test(Author = "Nooruddin Kapasi", Description = "Get method returns 404 when supplied key not found.")]
        public async Task Get_TagType_By_Key_Not_Found_Test()
        {
            var key = "S";
            var mockEntityTypeRepo = new Mock<ITagTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == key), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null);

            var cut = new TagTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var actionResult = await cut.GetSingleAsync(new CancellationToken(), key);
            var actual = actionResult as NegotiatedContentResult<ApiResult>;
            Assert.IsNotNull(actual, "ActionResult");
            Assert.AreEqual(HttpStatusCode.NotFound, actual.StatusCode, "NotFoundStatusCode");
            Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Content.error), "Error message present");
        }

        [Test(Author = "Nooruddin Kapasi", Description = "Get an tagtype that matches the supplied key."
            )]
        public async Task Get_TagType_By_Key_Test()
        {
            var expected = _database.Single(e => e.key == "hostId");
            var mockEntityTypeRepo = new Mock<ITagTypeRepository>();
            mockEntityTypeRepo.Setup(s => s.GetAsync(It.Is<string>(p => p == "hostId"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Model.TagType
                {
                   key = "hostId"
                }
                );

            var cut = new TagTypesController(mockEntityTypeRepo.Object) { Logger = MockLogger().Object };
            var actionResult = await cut.GetSingleAsync(new CancellationToken(), "hostId");
            var actual = actionResult as OkNegotiatedContentResult<Model.TagType>;
            Assert.IsNotNull(actual, "Has Content");
            Assert.AreEqual(expected.key, actual.Content.key, "key returned");
        }
    }
}