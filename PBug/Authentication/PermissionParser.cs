using System;

namespace PBug.Authentication;

public static class PermissionParser
{
    public static int MAX_DEPTH = 16;

    public static bool ProvePermission(ReadOnlySpan<char> permissions, ReadOnlySpan<char> requirement)
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