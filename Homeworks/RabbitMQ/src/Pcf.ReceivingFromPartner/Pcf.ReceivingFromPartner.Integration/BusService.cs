using MassTransit;
using Pcf.ReceivingFromPartner.Core.Abstractions.Services;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.Integration.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration;
public class BusService : IBusService
{
    private readonly IBusControl _busControl;

    public BusService(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public Task Publish(PromoCode promoCode, CancellationToken cancellationToken = default)
    {
        var dto = new GivePromoCodeToCustomerDto()
        {
            PartnerId = promoCode.Partner.Id,
            BeginDate = promoCode.BeginDate.ToShortDateString(),
            EndDate = promoCode.EndDate.ToShortDateString(),
            PreferenceId = promoCode.PreferenceId,
            PromoCode = promoCode.Code,
            ServiceInfo = promoCode.ServiceInfo,
            PartnerManagerId = promoCode.PartnerManagerId
        };

        return _busControl.Publish(dto, cancellationToken);
    }
}
