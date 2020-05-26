using DiffPlex.DiffBuilder.Model;
using DiffPlex.DiffBuilder;

namespace PBug.Utils
{
    public static class DiffHelper
    {
        public static SideBySideDiffModel DiffText(string t1, string t2)
        {
            return SideBySideDiffBuilder.Diff(t1, t2);
        }
    }
}