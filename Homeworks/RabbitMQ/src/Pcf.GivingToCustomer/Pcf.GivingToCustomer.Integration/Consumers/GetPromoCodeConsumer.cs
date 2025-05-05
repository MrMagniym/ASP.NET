using MassTransit;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.Integration.Dto;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration.Consumers;
public class GetPromoCodeConsumer : IConsumer<GivePromoCodeToCustomerDto>
{
    private readonly IPromoCodeService _promoCodeService;

    public GetPromoCodeConsumer(IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    public Task Consume(ConsumeContext<GivePromoCodeToCustomerDto> context)
    {
        return _promoCodeService.Observe(context.Message);
    }
}
