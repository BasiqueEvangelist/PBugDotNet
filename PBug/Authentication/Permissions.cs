namespace PBug.Authentication;

public static class Permissions
{
    public const int MAX_DEPTH = 16;

    /// <summary>
    /// Check if provided <paramref name="permissions"/> (one or more) fulfill <paramref name="requirement"/>.
    /// </summary>
    /// <param name="permissions">One ore more permissions, separated by semicolons.</param>
    /// <param name="requirement">One permission requirement</param>
    public static bool CheckPermissions(ReadOnlySpan<char> permissions, ReadOnlySpan<char> requirement)
    {
        Span<Range> permRanges = stackalloc Range[MAX_DEPTH];
        Span<Range> reqRanges = stackalloc Range[MAX_DEPTH];
        int reqRangesCount = requirement.Split(reqRanges, '.');

        foreach (var range in permissions.Split(';'))
        {
            var perm = permissions[range];
            int permRangesCount = perm.Split(permRanges, '.');
            
            if (reqRangesCount < permRangesCount)
                continue;

            int permIndex = 0, reqIndex = 0;

            while (permIndex < permRangesCount)
            {
                var pi = perm[permRanges[permIndex]];
                var ri = requirement[reqRanges[reqIndex]];

                if (pi.SequenceEqual("**")) reqIndex = reqRangesCount - (permRangesCount - permIndex);
                else if (!pi.SequenceEqual(ri) && !pi.SequenceEqual("*")) break;
                permIndex++;
                reqIndex++;
            }
            if (reqIndex == reqRangesCount) return true;
        }
        return false;
    }
}
