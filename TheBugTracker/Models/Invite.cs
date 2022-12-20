using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheBugTracker.Models
{
  public class Invite
  {
    public int Id { get; set; }

    [DisplayName("Company")]
    public int CompanyId { get; set; }

    [DisplayName("Project")]
    public int ProjectId { get; set; }

    [DisplayName("Invitor")]
    public string InvitorId { get; set; }

    [DisplayName("Invitee")]
    public string InviteeId { get; set; }


    [DisplayName("Date Sent")]
    [DataType(DataType.Date)]
    public DateTimeOffset InviteDate { get; set; }

    [DataType(DataType.Date)]
    [DisplayName("Join Date")]
    public DateTimeOffset JoinDate { get; set; }

    [DisplayName("Code")]
    public Guid CompanyToken { get; set; }

    [DisplayName("Invitee Email")]
    public string InviteeEmail { get; set; }

    [DisplayName("Invitee First Name")]
    public string InviteeFirstName { get; set; }

    [DisplayName("Invitee Last Name")]
    public string InviteeLastName { get; set; }

    //for record keeping to determine if invite is still valid
    [DisplayName("Is Invite Valid")]
    public bool IsValid { get; set; }



    //Navigation Properties
    public virtual Company Company { get; set; }
    public virtual Project Project { get; set; }
    public virtual BTUser Invitor { get; set; }
    public virtual BTUser Invitee { get; set; }
  }
}
