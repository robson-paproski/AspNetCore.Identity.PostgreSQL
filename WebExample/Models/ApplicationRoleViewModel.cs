using System;
using System.ComponentModel.DataAnnotations;
namespace WebExample.Models
{
    public class ApplicationRoleViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
