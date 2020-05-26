using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PBug.Data;

namespace PBug.Controllers
{
    public class CreatePageRequest
    {
        [BindProperty(Name = "name"), BindRequired]
        [StringLength(100)]
        public string Name { get; set; }

        [BindProperty(Name = "tags"), BindRequired]
        public string Tags { get; set; }

        [BindProperty(Name = "text"), BindRequired]
        public string Text { get; set; }

        [BindProperty(Name = "secrecy"), BindRequired]
        public KBSecrecy Secrecy { get; set; }
    }
}