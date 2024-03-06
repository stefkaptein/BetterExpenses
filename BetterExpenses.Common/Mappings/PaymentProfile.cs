using System.Globalization;
using AutoMapper;
using BetterExpenses.Common.Models.Expenses;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.Common.Mappings;

public class PaymentProfile : Profile
{
    private static readonly NumberFormatInfo AmountFormatInfo = new()
    {
        CurrencyDecimalSeparator = "."
    };
    
    public PaymentProfile()
    {
        CreateMap<Payment, UserExpense>()
            .ForMember(x => x.Created,
                y => y.MapFrom(z => DateTime.Parse(z.Created)))
            .ForMember(x => x.Updated,
                y => y.MapFrom(z => DateTime.Parse(z.Updated)))
            .ForMember(x => x.Amount,
                y => y.MapFrom(z => double.Parse(z.Amount.Value, AmountFormatInfo)));
    }
}