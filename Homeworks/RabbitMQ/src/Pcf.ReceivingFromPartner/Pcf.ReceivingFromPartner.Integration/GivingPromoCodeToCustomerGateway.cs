using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Integration.Services;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration;

public class GivingPromoCodeToCustomerGateway(IPromocodeService promocodeService) : IGivingPromoCodeToCustomerGateway
{
    private readonly IPromocodeService _promocodeService = promocodeService;

    public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        => await _promocodeService.Create(promoCode);
}