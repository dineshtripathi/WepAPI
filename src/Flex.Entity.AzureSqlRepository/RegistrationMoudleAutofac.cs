using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Autofac;
using AutoMapper;
using Flex.Entity.AzureSqlRepository.Mapper;
using Flex.Entity.AzureSqlRepository.Repository;

namespace Flex.Entity.AzureSqlRepository
{
    public class RegistrationMoudleAutofac : Module

    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MetaSchemaContext>().AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterType<EntityTypeRepository>()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerRequest();
            builder.RegisterType<RepositoryInitializer>().AsImplementedInterfaces().InstancePerLifetimeScope();

            //register automapper profiles
            
            //builder.RegisterAssemblyTypes().AssignableTo(typeof(Profile)).As<Profile>();
            var profileTypes = typeof(RegistrationMoudleAutofac).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)).ToArray();
            builder.RegisterTypes(profileTypes).As<Profile>();

            //register configuration as a single instance
            builder.Register(c => new MapperConfiguration(cfg =>
            {
                //add your profiles (either resolve from container or however else you acquire them)
                var profiles = c.Resolve<IEnumerable<Profile>>();
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();

            //register mapper
            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>().InstancePerLifetimeScope();

            //builder.RegisterType<AutoMapperConfig>()
            //    .AsSelf()
            //    .SingleInstance();
            //builder.Register(c => c.Resolve<AutoMapperConfig>().RegisterMappings()).As<IConfigurationProvider>().SingleInstance();

            //builder.Register(c => c.Resolve<IConfigurationProvider>().CreateMapper()).As<IMapper>();
        }
    }
}