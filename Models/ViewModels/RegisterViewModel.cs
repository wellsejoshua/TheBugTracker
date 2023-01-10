namespace TheBugTracker.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string CompanyName { get; set; }
        public string ProjectName { get; set; }
        public string InvitorName { get; set; }
        public string InviteeFirstName { get; set; }
        public string InviteeLastName { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
    }
}
