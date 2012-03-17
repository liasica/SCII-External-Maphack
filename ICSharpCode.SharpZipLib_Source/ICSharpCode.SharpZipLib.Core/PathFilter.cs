namespace ICSharpCode.SharpZipLib.Core
{
    using System;
    using System.IO;

    public class PathFilter : IScanFilter
    {
        private NameFilter nameFilter;

        public PathFilter(string filter)
        {
            this.nameFilter = new NameFilter(filter);
        }

        public virtual bool IsMatch(string name)
        {
            return this.nameFilter.IsMatch(Path.GetFullPath(name));
        }
    }
}

