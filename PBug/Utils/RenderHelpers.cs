using System.Text;
using Microsoft.AspNetCore.Html;
using PBug.Data;

namespace PBug.Utils
{
    public static class RenderHelpers
    {
        public static HtmlString ShowUser(User user)
        {
            StringBuilder builder = new();
            if (user == null)
            {
                builder.Append("Anonymous");
            }
            else
            {
                builder.Append(user.FullName);
                builder.Append(" <span class=\"username\">@");
                builder.Append(user.Username);
                builder.Append("</span>");
            }
            return new HtmlString(builder.ToString());
        }
    }
}