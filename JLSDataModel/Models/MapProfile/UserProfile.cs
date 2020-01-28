using JLSDataModel.Models.Mapping;
using AutoMapper;
namespace JLSDataModel.Models.MapProfile
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRegistrationView, JLSDataModel.Models.User.User>();
        }
    }
}
