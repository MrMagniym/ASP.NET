using Grpc.Net.Client;
using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration.Services;
public class PromocodeService : IPromocodeService, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly PromocodeCreator.PromocodeCreatorClient _client;
    public PromocodeService(Uri uri)
    {
        _channel = GrpcChannel.ForAddress(uri);
        _client = new PromocodeCreator.PromocodeCreatorClient(_channel);
    }

    public async Task Create(PromoCode promoCode)
    {
        var request = new Request()
        {
            PartnerId = new UUID() { Value = promoCode.Partner.Id.ToString() },
            BeginDate = promoCode.BeginDate.ToShortDateString(),
            EndDate = promoCode.EndDate.ToShortDateString(),
            PreferenceId = new UUID() { Value = promoCode.PreferenceId.ToString() },
            PromoCode = promoCode.Code,
            ServiceInfo = promoCode.ServiceInfo,
            PromoCodeId = new UUID() { Value = promoCode.Id.ToString() }
        };

        var result = await _client.CreateAsync(request);
        if (result?.Result != true)
            throw new HttpRequestException(result?.Message);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}
