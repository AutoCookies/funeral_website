using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DietDoHongTran.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public List<RoleSelection> Roles { get; set; }
    }

    public class RoleSelection
    {
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
