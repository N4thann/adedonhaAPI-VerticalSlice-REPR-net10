using AspNetCore.Identity.MongoDbCore.Models;

namespace AdedonhaAPI.Shared.Identity
{
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
