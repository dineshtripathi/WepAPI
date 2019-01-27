using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using Flex.Entity.AzureSqlRepository.DO;
using Flex.Entity.AzureSqlRepository.Mapper;
using Flex.Entity.AzureSqlRepository.Repository;
using NUnit.Framework;
using Moq;

namespace Flex.Entity.AzureSqlRepository.Tests
{
    [TestFixture]
    [Category("UnitTest")]
    public class EntityTypeRepositoryUnitTest
    {
        private readonly IMapper _mapper = new AutoMapper.Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new EntityTypeMapper())));

        [Test]
        public void Create_EntityTypes_Test()
        {
            List<EntityType> entityTypes;
            var repository = GetEntityTypeRepositoryMockForCreateMehtod(out entityTypes);
            var actual =  repository.CreateAsync(CreateModelEntityType("C","Client"),new CancellationToken());
            Assert.AreEqual(1, actual.Result);
            Assert.AreEqual(2, entityTypes.Count);
        }

        [Test]
        public void Create_Duplicate_Entry_EntityTypes_UniqueKeyViolation_Exception_Test()
        {
            List<EntityType> entityTypes;
            var repository = GetEntityTypeRepositoryMockForCreateMehtod(out entityTypes);
            Assert.That(() => repository.CreateAsync(CreateModelEntityType("L", "Load"), new CancellationToken()), Throws.TypeOf<System.Exception>());
        }

        [Test]
        public void Create_Duplicate_Prefix_EntityTypes_UniqueKeyVioloation_Exception_Test()
        {
            List<EntityType> entityTypes;
            var repository = GetEntityTypeRepositoryMockForCreateMehtod(out entityTypes);
            Assert.That(() => repository.CreateAsync(CreateModelEntityType("L", "Client"), new CancellationToken()), Throws.TypeOf<System.Exception>());
        }

        [Test]
        public void Create_Duplicate_Name_EntityTypes_UniqueKeyViolation_Exception_Test()
        {
            List<EntityType> entityTypes;
            var repository = GetEntityTypeRepositoryMockForCreateMehtod(out entityTypes);
            Assert.That(() => repository.CreateAsync(CreateModelEntityType("C", "Load"), new CancellationToken()), Throws.TypeOf<System.Exception>());
        }

        [Test]
        public void Get_EntityTypes_Found_Test()
        {
            var entityTypes = new List<EntityType>
            {
                new EntityType {Name = "Load", EntityTypeId = 1, Prefix = "L"}
            };

            var contextMock = new Mock<IDbContext>();
            var entitySetMock = new Mock<IDbSetWrapper<EntityType>>();
            entitySetMock.SetupDbSetAsync(contextMock,entityTypes);
            var repository = new EntityTypeRepository(contextMock.Object, _mapper);
            var actual = repository.GetAsync(new CancellationToken()).Result.FirstOrDefault();
            Assert.NotNull(actual);
            Assert.AreEqual("L", actual.prefix);
            Assert.AreEqual("Load", actual.name);
            Assert.AreEqual(false, actual.allow_in_asset_hierarchy);
            Assert.AreEqual(false, actual.allow_in_service_hierarchy);
            Assert.AreEqual(false, actual.allow_same_type_descendant);
        }

        [Test]
        public void Get_EntityTypes_NotFound_Test()
        {
            var entityTypes = new List<EntityType>();
            var contextMock = new Mock<IDbContext>();
            var entitySetMock = new Mock<IDbSetWrapper<EntityType>>();
            entitySetMock.SetupDbSetAsync(contextMock, entityTypes);
            var repository = new EntityTypeRepository(contextMock.Object, _mapper);
            var actual = repository.GetAsync(new CancellationToken()).Result.FirstOrDefault();
            Assert.Null(actual);
        }


        [Test]
        public void Delete_EntityTypes_Test()
        {
            List<EntityType> entityTypes;
            var repository = GetEntityTypeRepositoryMockForDeleteMehtod(out entityTypes);
            var actual = repository.DeleteAsync("C", new CancellationToken());
            Assert.AreEqual(1, actual.Result);
            Assert.AreEqual(1, entityTypes.Count);
        }

        [Test]
        public void Delete_NotExist_EntityTypes_Test()
        {
            List<EntityType> entityTypes;
            var repository = GetEntityTypeRepositoryMockForDeleteMehtod(out entityTypes);
            var actual = repository.DeleteAsync("NotExist", new CancellationToken());
            Assert.AreEqual(0, actual.Result);
            Assert.AreEqual(2, entityTypes.Count);
        }

        [Test]
        public void GetAsync_Prefix_Found_Test()
        {
            var repository = GetEntityTypeRepositoryForGetPrefixMehtod();
            var actual = repository.GetAsync("C", new CancellationToken()).Result;
            Assert.AreEqual("C", actual.prefix);
            Assert.AreEqual("Client", actual.name);
            Assert.AreEqual(false, actual.allow_in_asset_hierarchy);
            Assert.AreEqual(false, actual.allow_in_service_hierarchy);
            Assert.AreEqual(false, actual.allow_same_type_descendant);
        }

        [Test]
        public void GetAsync_Prefix_Not_Found_Null_Test()
        {
            var repository = GetEntityTypeRepositoryForGetPrefixMehtod();
            var actual = repository.GetAsync("D", new CancellationToken()).Result;
            Assert.IsNull(actual);
        }

        private EntityTypeRepository GetEntityTypeRepositoryForGetPrefixMehtod()
        {
            var entityTypes = new List<EntityType>()
            {
                new EntityType {Name = "Load", EntityTypeId = 1, Prefix = "L"},
                new EntityType {Name = "Client", EntityTypeId = 2, Prefix = "C"}
            };

            var contextMock = new Mock<IDbContext>();
            var entitySetMock = new Mock<IDbSetWrapper<EntityType>>();
            entitySetMock.SetupDbSetAsync(contextMock, entityTypes);

            return new EntityTypeRepository(contextMock.Object, _mapper);
        }

        private EntityTypeRepository GetEntityTypeRepositoryMockForCreateMehtod(out List<EntityType> entityTypes)
        {
            entityTypes = new List<EntityType>
            {
                new EntityType {Name = "Load", EntityTypeId = 1, Prefix = "L"}
            };

            var entityTypesBuffer = new List<EntityType>();
            var opertions = new List<MockExtensions.ChangesOperations>();
            var contextMock = new Mock<IDbContext>();
            var entitySetMock = new Mock<IDbSetWrapper<EntityType>>();
            entitySetMock.SetupDbSetAsync(contextMock, entityTypes);
            entitySetMock.SetupAddAsync(contextMock, entityTypesBuffer, opertions);
            contextMock.SetupSaveChangesAsync(entityTypes, entityTypesBuffer, opertions);

            return new EntityTypeRepository(contextMock.Object, _mapper);
        }

        private EntityTypeRepository GetEntityTypeRepositoryMockForDeleteMehtod(out List<EntityType> entityTypes)
        {
            entityTypes = new List<EntityType>
            {
                new EntityType {Name = "Load", EntityTypeId = 1, Prefix = "L"},
                new EntityType {Name = "Client", EntityTypeId = 2, Prefix = "C"}
            };

            var entityTypesBuffer = new List<EntityType>();
            var opertions = new List<MockExtensions.ChangesOperations>();

            var contextMock = new Mock<IDbContext>();
            var entitySetMock = new Mock<IDbSetWrapper<EntityType>>();
            entitySetMock.SetupDbSetAsync(contextMock, entityTypes);
            entitySetMock.SetupDeleteAsync(contextMock, entityTypesBuffer, opertions);
            contextMock.SetupSaveChangesAsync(entityTypes, entityTypesBuffer, opertions);

            return new EntityTypeRepository(contextMock.Object, _mapper);
        }

        private Api.Model.EntityType CreateModelEntityType(string prefix, string name)
        {
            return new Api.Model.EntityType
            {
                allow_in_asset_hierarchy = true,
                allow_in_service_hierarchy = true,
                allow_same_type_descendant = true,
                prefix = prefix,
                name = name
            };
        }
    }
}
