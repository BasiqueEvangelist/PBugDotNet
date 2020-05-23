namespace PBug.Authentication
{
    public static class PermissionParser
    {
        public static bool ProvePermission(string permstring, string required)
        {
            foreach (var match in permstring.Split(";"))
            {
                if (MatchPermission(match, required))
                    return true;
            }
            return false;

        }

        private static bool MatchPermission(string match, string required)
        {
            string[] matchwords = match.Split('.');
            string[] reqwords = required.Split('.');

            if (reqwords.Length < matchwords.Length)
                return false;

            int matchindex = 0, reqindex = 0;

            while (matchindex < matchwords.Length)
            {
                if (matchwords[matchindex] == "**") reqindex = reqwords.Length - (matchwords.Length - matchindex);
                else if (matchwords[matchindex] != reqwords[reqindex] && matchwords[matchindex] != "*") return false;
                matchindex++;
                reqindex++;
            }

            return reqindex == reqwords.Length;
        }
    }
}