using AutoMapper;
using BetterExpenses.Common.DTO.User;
using UserSettings = BetterExpenses.Common.Models.User.UserSettings;

namespace BetterExpenses.Common.Mappings;

public class UserSettingsProfile : Profile
{
    public UserSettingsProfile()
    {
        CreateMap<UserSettings, UserSettings>();

        CreateMap<UserSettingsDto, UserSettings>().ReverseMap();
    }
}