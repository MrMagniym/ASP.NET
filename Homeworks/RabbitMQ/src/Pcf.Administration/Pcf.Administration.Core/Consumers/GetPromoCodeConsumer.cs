using MassTransit;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Integration.Dto;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Consumers;
public class GetPromoCodeConsumer : IConsumer<GivePromoCodeToCustomerDto>
{
    private readonly IPromoCodeService _promoCodeService;

    public GetPromoCodeConsumer(IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    public Task Consume(ConsumeContext<GivePromoCodeToCustomerDto> context)
    {
        if (context.Message.PartnerManagerId.HasValue)
            return _promoCodeService.UpdateAppliedPromocodesAsync(context.Message.PartnerManagerId.Value);

        return Task.CompletedTask;
    }
}
