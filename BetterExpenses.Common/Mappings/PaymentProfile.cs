using System.Globalization;
using AutoMapper;
using BetterExpenses.Common.Models.Expenses;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.Common.Mappings;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, UserExpense>()
            .ForMember(x => x.Created,
                y => y.MapFrom(z => DateTime.Parse(z.Created)))
            .ForMember(x => x.Updated,
                y => y.MapFrom(z => DateTime.Parse(z.Updated)));
    }
}