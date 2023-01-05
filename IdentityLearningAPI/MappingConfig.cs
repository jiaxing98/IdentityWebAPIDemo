using AutoMapper;
using IdentityLearningAPI.Dtos;
using IdentityLearningAPI.Models;

namespace IdentityLearningAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(ConfigurationBinder =>
            {
                ConfigurationBinder.CreateMap<UserSignUpDto, ApplicationUser>();
                ConfigurationBinder.CreateMap<UserLoginDto, ApplicationUser>();
            });

            return mappingConfig;
        }
    }
}
