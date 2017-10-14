using System;
namespace WebExample.Models
{
    public class ApplicationRoleListViewModel
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int NumberOfUsers { get; set; }
    }
}
