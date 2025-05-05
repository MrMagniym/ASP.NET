using Pcf.Integration.Dto;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Services;
public interface IPromoCodeService
{
    Task<bool> Observe(GivePromoCodeToCustomerDto request);
}
