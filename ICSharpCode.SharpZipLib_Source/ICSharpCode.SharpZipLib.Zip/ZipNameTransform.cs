namespace ICSharpCode.SharpZipLib.Zip
{
    using ICSharpCode.SharpZipLib.Core;
    using System;
    using System.IO;

    public class ZipNameTransform : INameTransform
    {
        private bool relativePath;
        private string trimPrefix;

        public ZipNameTransform()
        {
            this.relativePath = true;
        }

        public ZipNameTransform(bool useRelativePaths)
        {
            this.relativePath = useRelativePaths;
        }

        public ZipNameTransform(bool useRelativePaths, string trimPrefix)
        {
            this.trimPrefix = trimPrefix;
            this.relativePath = useRelativePaths;
        }

        public string TransformDirectory(string name)
        {
            name = this.TransformFile(name);
            if (name.Length > 0)
            {
                if (!name.EndsWith("/"))
                {
                    name = name + "/";
                }
                return name;
            }
            name = "/";
            return name;
        }

        public string TransformFile(string name)
        {
            if (name != null)
            {
                if ((this.trimPrefix != null) && (name.IndexOf(this.trimPrefix) == 0))
                {
                    name = name.Substring(this.trimPrefix.Length);
                }
                if (Path.IsPathRooted(name))
                {
                    name = name.Substring(Path.GetPathRoot(name).Length);
                }
                if (this.relativePath)
                {
                    if ((name.Length > 0) && ((name[0] == Path.AltDirectorySeparatorChar) || (name[0] == Path.DirectorySeparatorChar)))
                    {
                        name = name.Remove(0, 1);
                    }
                }
                else if (((name.Length > 0) && (name[0] != Path.AltDirectorySeparatorChar)) && (name[0] != Path.DirectorySeparatorChar))
                {
                    name = name.Insert(0, "/");
                }
                name = name.Replace(@"\", "/");
                return name;
            }
            name = "";
            return name;
        }

        public string TrimPrefix
        {
            get
            {
                return this.trimPrefix;
            }
            set
            {
                this.trimPrefix = value;
            }
        }
    }
}

