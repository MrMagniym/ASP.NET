using Pcf.ReceivingFromPartner.Core.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Services;
public interface IBusService
{
    Task Publish(PromoCode promoCode, CancellationToken cancellationToken = default);
}
