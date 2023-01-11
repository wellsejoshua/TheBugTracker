using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
<<<<<<< HEAD
  //filter a list of only one item of btuser in order to change code in future if I want others to be able to send invites on the admins behalf. For now lock down for only admins and no project managers
  [Authorize(Roles = "Admin")]
  public class InvitesController : Controller
  {
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly UserManager<BTUser> _userManager;
    private readonly IBTCompanyInfoService _companyService;
    private readonly IEmailSender _emailSender;
    private readonly IBTInviteService _inviteService;
    private readonly IBTProjectService _projectService;

    #endregion

    #region Constructor
    public InvitesController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTCompanyInfoService companyService, IEmailSender emailSender, IBTInviteService inviteService, IBTProjectService projectService)
    {
      _context = context;
      _userManager = userManager;
      _companyService = companyService;
      _emailSender = emailSender;
      _inviteService = inviteService;
      _projectService = projectService;
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
      List<Project> projectList = await _projectService.GetAllProjectsByCompanyAsync(companyId);
      company.Add(await _companyService.GetCompanyInfoByIdAsync(companyId));
      invitor.Add((await _userManager.GetUserAsync(User)));
      

      ViewData["CompanyId"] = new SelectList(company, "Id", "Name");
      //ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "FullName");
      ViewData["InvitorId"] = new SelectList(invitor, "Id", "FullName");
      //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
      ViewData["ProjectId"] = new SelectList(projectList, "Id", "Name");
      return View();
    }
    #endregion

    #region Create Post
    // POST: Invites/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CompanyId,ProjectId,InvitorId,InviteeEmail,InviteeFirstName,InviteeLastName")] Invite invite)
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
        Company inviteCompany = await _companyService.GetCompanyInfoByIdAsync(invite.CompanyId);
        string url = CreateUrl(guid, invite.CompanyId, invite.InviteeEmail);
        string subject = "Welcome to the team!";
        string message = "Click the link to join our company " + inviteCompany.Name + " " + url;
        await _emailSender.SendEmailAsync(invite.InviteeEmail, subject, message);
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

    #region Private Create Url 
    private static string CreateUrl(Guid inviteId, int companyId, string email)
    {
      string url = string.Format("https://localhost:44371/Invites/ProcessInvite?inviteId={0}&companyId={1}&email={2}", inviteId.ToString(), "1", email);
      return url;
    }
    #endregion



    private static string StripUrlStartTag(string url)
    {
      var x = url[0];
      var striptUrl = url[1..];
      return striptUrl;

    }

    private static string[] ProcessUrl(string url)
    {
      string[] properties = url.Split("&");
      string inviteId = (properties[0].Split("="))[1];
      string companyId = (properties[1].Split("="))[1];
      string email = (properties[2].Split("="))[1];
      //Guid token = new Guid(inviteId);
      string[] array = new string[3];
      array[0] = inviteId;
      array[1] = email;
      array[2] = companyId;

      return array;

    }

    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> ProcessInvite()
    {
      try
      {
        //string url = HttpContext.Request.Host.Value;
        string url = HttpContext.Request.QueryString.Value;
        if (url.StartsWith("?"))
        {
          url = StripUrlStartTag(url);
        }
        var processed = ProcessUrl(url);

        //Guid token = new Guid("5f42f7dc-6b3a-4b7e-aae3-560ceeb556eb");
        ////bool tokenValid = false;
        //Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);
        string st = processed[0];
        Guid token = new Guid(processed[0]);
        int companyId = Int32.Parse(processed[2]);
        bool isInvite = await _inviteService.AnyInviteAsync(token, processed[1], companyId);
        Invite invite = new();
        Project project = new();
        Company company = new();

        invite = await _inviteService.GetInviteAsync(token, processed[1], companyId);
        company = await _companyService.GetCompanyInfoByIdAsync(invite.CompanyId);
        project = await _projectService.GetProjectByIdAsync(invite.ProjectId, invite.CompanyId);
        Project tryProject = _context.Projects.FirstOrDefault(p => p.CompanyId == companyId && p.Id == invite.ProjectId);
        ProcessInviteViewModel model = new();
        model.Invite = invite;
        //model.ProjectName = project.Name;
        model.CompanyName = company.Name;
        model.InvitorName = (_context.Users.FirstOrDefault(u => u.Id == invite.InvitorId)).FullName;
        model.ProjectName = project.Name;
        return View(model);
      }
      catch (Exception)
      {

        throw;
      }


      //await _companyService.GetCompanyInfoByIdAsync();


    }
  }
=======
    //filter a list of only one item of btuser in order to change code in future if I want others to be able to send invites on the admins behalf. For now lock down for only admins and no project managers
    [Authorize(Roles = "Admin")]
    public class InvitesController : Controller
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyInfoService _companyService;
        private readonly IEmailSender _emailSender;
        private readonly IBTInviteService _inviteService;
        private readonly IBTProjectService _projectService;

        #endregion

        #region Constructor
        public InvitesController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTCompanyInfoService companyService, IEmailSender emailSender, IBTInviteService inviteService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _companyService = companyService;
            _emailSender = emailSender;
            _inviteService = inviteService;
            _projectService = projectService;
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
            List<Project> projectList = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            company.Add(await _companyService.GetCompanyInfoByIdAsync(companyId));
            invitor.Add((await _userManager.GetUserAsync(User)));


            ViewData["CompanyId"] = new SelectList(company, "Id", "Name");
            //ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["InvitorId"] = new SelectList(invitor, "Id", "FullName");
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["ProjectId"] = new SelectList(projectList, "Id", "Name");
            return View();
        }
        #endregion

        #region Create Post
        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,ProjectId,InvitorId,InviteeEmail,InviteeFirstName,InviteeLastName")] Invite invite)
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
                Company inviteCompany = await _companyService.GetCompanyInfoByIdAsync(invite.CompanyId);
                string url = CreateUrl(guid, invite.CompanyId, invite.InviteeEmail);
                string subject = "Welcome to the team!";
                string message = "Click the link to join our company " + inviteCompany.Name + " " + url;
                await _emailSender.SendEmailAsync(invite.InviteeEmail, subject, message);
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

        #region Private Create Url 
        private static string CreateUrl(Guid inviteId, int companyId, string email)
        {
            string url = string.Format("https://localhost:44371/Invites/ProcessInvite?inviteId={0}&companyId={1}&email={2}", inviteId.ToString(), "1", email);
            return url;
        }
        #endregion

        #region Strip URL Start Tag

        private static string StripUrlStartTag(string url)
        {
            var x = url[0];
            var striptUrl = url[1..];
            return striptUrl;

        }

        #endregion

        #region Process URL
        private static string[] ProcessUrl(string url)
        {
            string[] properties = url.Split("&");
            string inviteId = (properties[0].Split("="))[1];
            string companyId = (properties[1].Split("="))[1];
            string email = (properties[2].Split("="))[1];
            //Guid token = new Guid(inviteId);
            string[] array = new string[3];
            array[0] = inviteId;
            array[1] = email;
            array[2] = companyId;

            return array;

        }

        #endregion

        #region Process Member Invite
        [AllowAnonymous]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> ProcessInvite()
        {
            try
            {
                //string url = HttpContext.Request.Host.Value;
                string url = HttpContext.Request.QueryString.Value;
                if (url.StartsWith("?"))
                {
                    url = StripUrlStartTag(url);
                }
                var processed = ProcessUrl(url);

                //Guid token = new Guid("5f42f7dc-6b3a-4b7e-aae3-560ceeb556eb");
                ////bool tokenValid = false;
                //Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);
                string st = processed[0];
                Guid token = new Guid(processed[0]);
                int companyId = Int32.Parse(processed[2]);
                bool isInvite = await _inviteService.AnyInviteAsync(token, processed[1], companyId);
                Invite invite = new();
                Project project = new();
                Company company = new();

                invite = await _inviteService.GetInviteAsync(token, processed[1], companyId);
                company = await _companyService.GetCompanyInfoByIdAsync(invite.CompanyId);
                project = await _projectService.GetProjectByIdAsync(invite.ProjectId, invite.CompanyId);
                Project tryProject = _context.Projects.FirstOrDefault(p => p.CompanyId == companyId && p.Id == invite.ProjectId);
                ProcessInviteViewModel model = new();
                model.Invite = invite;
                //model.ProjectName = project.Name;
                model.CompanyName = company.Name;
                model.InvitorName = (_context.Users.FirstOrDefault(u => u.Id == invite.InvitorId)).FullName;
                model.ProjectName = project.Name;
                RegisterViewModel registerViewModel = new RegisterViewModel();
                registerViewModel.ProjectName = project.Name;
                registerViewModel.CompanyName = company.Name;
                registerViewModel.InvitorName = (_context.Users.FirstOrDefault(u => u.Id == invite.InvitorId)).FullName;
                registerViewModel.InviteeFirstName = invite.InviteeFirstName;
                registerViewModel.InviteeLastName = invite.InviteeLastName;
                registerViewModel.CompanyId = company.Id;



                return View(model);
            }
            catch (Exception)
            {

                throw;
            }


            //await _companyService.GetCompanyInfoByIdAsync();


        }

        #endregion
    }
>>>>>>> parent of a765b0d (Updated folder structure for railway)
}
