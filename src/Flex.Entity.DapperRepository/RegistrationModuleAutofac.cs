using System.Collections.Generic;
using System.Linq;
using Autofac;
using AutoMapper;
using Flex.Entity.Api.Model;
using Flex.Entity.DapperRepository.Dapper;
using Flex.Entity.DapperRepository.Repository;
using Flex.Entity.JWTSecurity;
using Flex.Entity.Security;

namespace Flex.Entity.DapperRepository
{
    public class RegistrationModuleAutofac : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Server.MapPath("~/bin")
            builder.RegisterType<DapperRepositoryInitializer>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DbConnectionFactory>().AsImplementedInterfaces().SingleInstance();
            //builder.RegisterType<SecurityContextProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EntityTypeRepository>()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerRequest();
            builder.RegisterType<EntityRepository>()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerRequest();
            builder.RegisterType<TagTypeRepository>()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerRequest();
            builder.RegisterType<TagRepository>()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerRequest();

            //register automapper profiles

            var profileTypes = typeof(RegistrationModuleAutofac).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)).ToArray();
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
            
        }
    }
}