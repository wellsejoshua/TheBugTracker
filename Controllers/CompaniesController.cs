using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.ViewModels;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
  public class CompaniesController : Controller
  {
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTCompanyManagement _companyManagement;

        #endregion

        #region Constructor
        public CompaniesController(ApplicationDbContext context, IBTCompanyInfoService companyInfoService, IBTCompanyManagement companyManagement)
        {
            _context = context;
            _companyInfoService = companyInfoService;
            _companyManagement = companyManagement;
        }

        #endregion

        #region Index
        // GET: Companies
        public async Task<IActionResult> Index()
    {

      // show number of members in company by status Dev/Admin/...
      //show the total number of projects; show total number of completed Projects
      //

      try
      {
        //int companyId = User.Identity.GetCompanyId().Value;
        ////var company = _companyInfoService.GetCompanyInfoByIdAsync(companyId);
        //var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
        DashboardViewModel model = new();
        int companyId = User.Identity.GetCompanyId().Value;

        model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
        model.Projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();
        model.Tickets = model.Projects.SelectMany(p => p.Tickets).Where(t => t.Archived == false).ToList();
        model.Members = model.Company.Members.ToList();

        return View(model);

      }
      catch (Exception)
      {

        throw;
      }

      //return View(await _context.Companies.ToListAsync());
    }
    #endregion

    #region Details
    // GET: Companies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var company = await _context.Companies
          .FirstOrDefaultAsync(m => m.Id == id);
      if (company == null)
      {
        return NotFound();
      }

      return View(company);
    }
    #endregion

    #region Create Get
    // GET: Companies/Create
    public IActionResult Create()
    {
      return View();
    }
    #endregion

    #region Create Post
    // POST: Companies/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description")] Company company)
    {
      if (ModelState.IsValid)
      {
        _context.Add(company);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(company);
    }
    #endregion

    #region Edit Get
    // GET: Companies/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      Company company = await _context.Companies.FindAsync(id);
      if (company == null)
      {
        return NotFound();
      }
      return View(company);
    }
    #endregion

    #region Edit Post
    // POST: Companies/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Company company)
    {
      if (id != company.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(company);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!CompanyExists(company.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      return View(company);
    }
    #endregion

    #region Delete Get
    // GET: Companies/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var company = await _context.Companies
          .FirstOrDefaultAsync(m => m.Id == id);
      if (company == null)
      {
        return NotFound();
      }

      return View(company);
    }
    #endregion

    #region Delete Post
    // POST: Companies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var company = await _context.Companies.FindAsync(id);
      _context.Companies.Remove(company);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }
    #endregion

    #region Private Company Exists

    private bool CompanyExists(int id)
    {
      return _context.Companies.Any(e => e.Id == id);
    }
    #endregion
  }
}
