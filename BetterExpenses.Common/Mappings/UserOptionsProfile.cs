using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace BetterExpenses.Common.Mappings;

public class UserOptionsProfile : Profile
{
    public UserOptionsProfile()
    {
        CreateMap<UserOptions, UserOptions>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}