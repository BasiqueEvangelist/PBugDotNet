using System.Collections.Generic;
using PBug.Data;

namespace PBug.Models
{
    public class IssueEditIssueModel
    {
        public Issue Issue { get; set; }
        public IEnumerable<Project> AllProjects;
        public IEnumerable<User> AllUsers;
    }
}