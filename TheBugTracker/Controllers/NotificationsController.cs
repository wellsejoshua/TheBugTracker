using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.Enums;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
  public class NotificationsController : Controller
  {
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly IBTCompanyInfoService _companyService;
    private readonly IBTTicketService _ticketService;
    private readonly UserManager<BTUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IBTNotificationService _notificationService;

    #endregion

    #region Constructor
    public NotificationsController(ApplicationDbContext context, IBTCompanyInfoService companyService, IBTTicketService ticketService, UserManager<BTUser> userManager, IEmailSender emailSender, IBTNotificationService notificationService)
    {
      _context = context;
      _companyService = companyService;
      _ticketService = ticketService;
      _userManager = userManager;
      _emailSender = emailSender;
      _notificationService = notificationService;
    }
    #endregion

    #region Index
    // GET: Notifications
    public async Task<IActionResult> Index()
    {
      //keep lists seperate for different displays add a filter by company as well before filter by user id
      BTUser user = await _userManager.GetUserAsync(User);
      List<Notification> sentNotifications = await _notificationService.GetReceivedNotificationsAsync(user.Id);
      List<Notification> recievedNotifications = await _notificationService.GetSentNotificationsAsync(user.Id);
      //come back after adding has been viewed button functionality 
      var notifications = sentNotifications.Union(recievedNotifications).OrderByDescending(n => n.Created);
      //var applicationDbContext = _context.Notifications.Include(n => n.Recipient).Include(n => n.Sender).Include(n => n.Ticket);
      //await applicationDbContext.ToListAsync()
      return View(notifications);
    }
    #endregion

    #region Details
    // GET: Notifications/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var notification = await _context.Notifications
          .Include(n => n.Recipient)
          .Include(n => n.Sender)
          .Include(n => n.Ticket)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (notification == null)
      {
        return NotFound();
      }

      return View(notification);
    }
    #endregion

    #region Create Get
    // GET: Notifications/Create
    public async Task<IActionResult> Create()
    {
      //Gave Admins and Project Managers the ability to send Notifications about any tickets in case one project manager is covering for another
      int companyId = User.Identity.GetCompanyId().Value;
      BTUser currentUser = await _userManager.GetUserAsync(User);
      List<BTUser> users = await _companyService.GetAllMembersAsync(companyId);
      List <BTUser> userNow = new();
      userNow.Add(currentUser);
      if (User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
      {
        List<Ticket> ticketNotification = await _ticketService.GetAllTicketsByCompanyAsync(companyId) ;
        ViewData["RecipientId"] = new SelectList(users, "Id", "FullName");
        ViewData["SenderId"] = new SelectList(userNow, "Id", "FullName");
        ViewData["TicketId"] = new SelectList(ticketNotification, "Id", "Description");
        return View();

      }
      else
      {
        List<Ticket> ticketNotification = await _ticketService.GetTicketsByUserIdAsync(currentUser.Id, companyId);
        ViewData["RecipientId"] = new SelectList(users, "Id", "FullName");
        ViewData["SenderId"] = new SelectList(userNow, "Id", "FullName");
        ViewData["TicketId"] = new SelectList(ticketNotification, "Id", "Description");
        return View();
      }
      


    }
    #endregion

    #region Create Post
    // POST: Notifications/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,TicketId,RecipientId,SenderId,Title,Message")] Notification notification)
    {
      if (ModelState.IsValid)
      {
        try
        {
          notification.Created = DateTimeOffset.Now;
          notification.Viewed = false;
          _context.Add(notification);
          await _context.SaveChangesAsync();
          //Get email of recipient from their Id
          BTUser recipient = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);
          await _emailSender.SendEmailAsync(recipient.Email, notification.Title, notification.Message);
          return RedirectToAction(nameof(Index));

        }
        catch (Exception)
        {

          throw;
        }
      }
      ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Name", notification.RecipientId);
      ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Name", notification.SenderId);
      ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);

      return View(notification);
    }

    #endregion

    #region Edit Get
    // GET: Notifications/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var notification = await _context.Notifications.FindAsync(id);
      if (notification == null)
      {
        return NotFound();
      }
      ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id", notification.RecipientId);
      ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", notification.SenderId);
      ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);
      return View(notification);
    }
    #endregion

    #region Edit Post
    // POST: Notifications/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,RecipientId,SenderId,Title,Message,Created,Viewed")] Notification notification)
    {
      if (id != notification.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(notification);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!NotificationExists(notification.Id))
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
      ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id", notification.RecipientId);
      ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", notification.SenderId);
      ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);
      return View(notification);
    }

    #endregion

    #region Delete Get
    // GET: Notifications/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var notification = await _context.Notifications
          .Include(n => n.Recipient)
          .Include(n => n.Sender)
          .Include(n => n.Ticket)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (notification == null)
      {
        return NotFound();
      }

      return View(notification);
    }
    #endregion

    #region Delete Post
    // POST: Notifications/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var notification = await _context.Notifications.FindAsync(id);
      _context.Notifications.Remove(notification);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }
    #endregion

    #region Private Notification Exists
    private bool NotificationExists(int id)
    {
      return _context.Notifications.Any(e => e.Id == id);
    }
    #endregion

  }
}
