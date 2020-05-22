using System.Collections.Generic;
using PBug.Data;

namespace PBug.Models
{
    public class IssueCreateModel
    {
        public IEnumerable<Project> AllProjects;
        public IEnumerable<User> AllUsers;
    }
}