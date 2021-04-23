using System.Text;
using Microsoft.AspNetCore.Html;
using PBug.Data;

namespace PBug.Utils
{
    public static class RenderHelpers
    {
        public static HtmlString ShowUser(User user, bool withLink = true)
        {
            StringBuilder builder = new();
            if (user == null)
            {
                builder.Append("Anonymous");
            }
            else
            {
                if (withLink)
                {
                    builder.Append("<a class=\"userlink\" href=\"/user/");
                    builder.Append(user.Username);
                    builder.Append("\">");
                }
                builder.Append(user.FullName);
                builder.Append(" <span class=\"username\">@");
                builder.Append(user.Username);
                builder.Append("</span>");
                if (withLink)
                    builder.Append("</a>");
            }
            return new HtmlString(builder.ToString());
        }
    }
}