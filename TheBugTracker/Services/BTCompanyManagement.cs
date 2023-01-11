using System.Threading.Tasks;
using TheBugTracker.Data;
using TheBugTracker.Services.Interfaces;
using TheBugTracker.Models;

namespace TheBugTracker.Services
{
    public class BTCompanyManagement : IBTCompanyManagement
    {
        private readonly ApplicationDbContext _context;

        public BTCompanyManagement(ApplicationDbContext context)
        {
            _context = context;
            
        }

        public Task ArchiveCompany()
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateCompany(Company company)
        {
            try
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task UpdateCompany(Company company)
        {
            _context.Add(company);
            await _context.SaveChangesAsync();
        }
    }
}
