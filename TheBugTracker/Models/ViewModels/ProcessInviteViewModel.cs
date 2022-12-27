using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheBugTracker.Models.ViewModels
{
  public class ProcessInviteViewModel
  {
    public Invite Invite { get; set; }
    public string CompanyName { get; set; }
    public string ProjectName { get; set; }
    public string InvitorName { get; set; }
    
  }
}
