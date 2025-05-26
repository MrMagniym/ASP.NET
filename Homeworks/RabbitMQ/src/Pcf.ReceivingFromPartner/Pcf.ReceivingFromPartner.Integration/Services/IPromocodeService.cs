using Pcf.ReceivingFromPartner.Core.Domain;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration.Services;
public interface IPromocodeService
{
    public Task Create(PromoCode promoCode);
}
