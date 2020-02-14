using AutoMapper;
using JLSDataModel.Models.Adress;
using JLSDataModel.Models.User;
using JLSMobileApplication.Resources;

namespace JLSMobileApplication.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationView, User>();// 将UserRegistrationView 映射到  user中
            CreateMap<UserRegistrationView, Adress>();// 将UserRegistrationView 映射到  adress 中
            CreateMap<User, Auth.Auth>();
        }
    }
}
