using System.Threading.Tasks;
using TheBugTracker.Models;

namespace TheBugTracker.Services.Interfaces
{
    public interface IBTCompanyManagement
    {
        public Task CreateCompany(Company company);

        public Task ArchiveCompany();

        public Task UpdateCompany(Company company);
    }
}
