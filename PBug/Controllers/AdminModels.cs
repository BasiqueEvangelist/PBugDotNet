using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PBug.Controllers
{
    public class CreateProjectRequest
    {
        [BindProperty(Name = "name"), BindRequired]
        [StringLength(maximumLength: 100, MinimumLength = 3)]
        public string Name { get; set; }

        [BindProperty(Name = "shortprojectid"), BindRequired]
        [StringLength(maximumLength: 3, MinimumLength = 3)]
        public string ShortID { get; set; }
    }

    public class SetRoleRequest
    {
        [BindProperty(Name = "id"), BindRequired]
        public uint UserID { get; set; }

        [BindProperty(Name = "roleid"), BindRequired]
        public uint RoleID { get; set; }
    }
}