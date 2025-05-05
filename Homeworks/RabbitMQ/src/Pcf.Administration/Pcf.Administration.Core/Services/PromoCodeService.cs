using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Administration.Core.Domain.Administration;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Services;
public class PromoCodeService : IPromoCodeService
{
    private readonly IRepository<Employee> _employeeRepository;

    public PromoCodeService(IRepository<Employee> employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<bool> UpdateAppliedPromocodesAsync(Guid id)
    {
       var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return false;

        employee.AppliedPromocodesCount++;

        await _employeeRepository.UpdateAsync(employee);

        return true;
    }
}
