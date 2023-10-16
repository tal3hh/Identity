namespace AspNetCoreIdentity.Models.Account
{
    public class RoleAssignSendModel
    {
        public List<RoleAssingListModel>? Roles { get; set; }
        public string? UserId { get; set; }
    }
}
