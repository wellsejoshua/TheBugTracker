using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
  //filter a list of only one item of btuser in order to change code in future if I want others to be able to send invites on the admins behalf. For now lock down for only admins and no project managers
  [Authorize(Roles ="Admin")]
  public class InvitesController : Controller
  {
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly UserManager<BTUser> _userManager;
    private readonly IBTCompanyInfoService _companyService;

    #endregion

    #region Constructor
    public InvitesController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTCompanyInfoService companyService)
    {
      _context = context;
      _userManager = userManager;
      _companyService = companyService;
    }
    #endregion

    #region Index
    // GET: Invites
    public async Task<IActionResult> Index()
    {
      var applicationDbContext = _context.Invites.Include(i => i.Company).Include(i => i.Invitee).Include(i => i.Invitor).Include(i => i.Project);
      return View(await applicationDbContext.ToListAsync());
    }
    #endregion

    #region Details
    // GET: Invites/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var invite = await _context.Invites
          .Include(i => i.Company)
          .Include(i => i.Invitee)
          .Include(i => i.Invitor)
          .Include(i => i.Project)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (invite == null)
      {
        return NotFound();
      }

      return View(invite);
    }
    #endregion

    #region Create Get
    // GET: Invites/Create
    public async Task<IActionResult> Create()
    {

      List<BTUser> invitor = new();
      int companyId = User.Identity.GetCompanyId().Value;
      List<Company> company = new();
      company.Add(await _companyService.GetCompanyInfoByIdAsync(companyId));
      
      invitor.Add((await _userManager.GetUserAsync(User)));
      ViewData["CompanyId"] = new SelectList(company, "Id", "Name");
      ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "FullName");
      ViewData["InvitorId"] = new SelectList(invitor, "Id", "FullName");
      ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
      return View();
    }
    #endregion

    #region Create Post
    // POST: Invites/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CompanyId,ProjectId,InvitorId,InviteeEmail,InviteeFirstName,InviteeLastName,IsValid")] Invite invite)
    {
      if (ModelState.IsValid)
      {
        //join date is set when the invite is accepted
        invite.InviteDate = DateTime.Now;
        invite.IsValid = true;
        System.Guid guid = System.Guid.NewGuid();
        invite.CompanyToken = guid;
        _context.Add(invite);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }

      List<BTUser> invitor = new();
      invitor.Add((await _userManager.GetUserAsync(User)));
      int companyId = User.Identity.GetCompanyId().Value;
      List<Company> company = new();
      company.Add(await _companyService.GetCompanyInfoByIdAsync(companyId));
      ViewData["CompanyId"] = new SelectList(company, "Id", "Name");
      //ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "FullName", invite.InviteeId);
      ViewData["InvitorId"] = new SelectList(invitor, "Id", "FullName");
      ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", invite.ProjectId);
      return View(invite);
    }

    #endregion

    #region Edit Get
    // GET: Invites/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var invite = await _context.Invites.FindAsync(id);
      if (invite == null)
      {
        return NotFound();
      }
      ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", invite.CompanyId);
      ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id", invite.InviteeId);
      ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id", invite.InvitorId);
      ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", invite.ProjectId);
      return View(invite);
    }
    #endregion

    #region Edit Post
    // POST: Invites/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,ProjectId,InvitorId,InviteeId,InviteDate,JoinDate,CompanyToken,InviteeEmail,InviteeFirstName,InviteeLastName")] Invite invite)
    {
      if (id != invite.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(invite);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!InviteExists(invite.Id))
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
      ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", invite.CompanyId);
      ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id", invite.InviteeId);
      ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id", invite.InvitorId);
      ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", invite.ProjectId);
      return View(invite);
    }

    #endregion

    #region Delete
    // GET: Invites/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var invite = await _context.Invites
          .Include(i => i.Company)
          .Include(i => i.Invitee)
          .Include(i => i.Invitor)
          .Include(i => i.Project)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (invite == null)
      {
        return NotFound();
      }

      return View(invite);
    }
    #endregion

    #region Delete Post
    // POST: Invites/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var invite = await _context.Invites.FindAsync(id);
      _context.Invites.Remove(invite);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }
    #endregion

    #region Private Invite Exists
    private bool InviteExists(int id)
    {
      return _context.Invites.Any(e => e.Id == id);
    }
    #endregion
  }
}
