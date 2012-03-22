namespace MpqTool
{
    using Foole.Mpq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    internal class App
    {
        private string archiveFile;
        private string destDir;
        private static Regex destDirExpression = new Regex(@"-d\s*(?<file>.+)", RegexOptions.Singleline | RegexOptions.Compiled);
        private string listFile;
        private static Regex listFileExpression = new Regex(@"-l\s*(?<file>.+)", RegexOptions.Singleline | RegexOptions.Compiled);
        private bool quietOutput;
        private Regex regex;
        private static Regex regexArgExpression = new Regex(@"-r\s*(?<exp>.+)", RegexOptions.Singleline | RegexOptions.Compiled);
        private bool stripPath;

        private string BuildUsage()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Usage: mpqtool <command> <archive> [option]\n");
            builder.Append("\n");
            builder.Append("Commands:\n");
            builder.Append("\n");
            builder.Append("Extract files from the given mpq archive.\n");
            builder.Append("  -e, --extract [wildcard] [-d<destDir>] [-r<regex>] [-sp] [-q] [-l<listFile>]\n");
            builder.Append("\n");
            builder.Append("List the contents of a mpq archive.\n");
            builder.Append("  -l, --list [wildcard] [-r<regex>]  [-sp] [-q] [-l<listFile>]\n");
            builder.Append("\n");
            builder.Append("Display this help and exit\n");
            builder.Append("  -h, --help\n");
            builder.Append("\n");
            builder.Append("\n");
            builder.Append("Options:\n");
            builder.Append("\n");
            builder.Append("  <wildcard>\tStandard wildcard (*.mpq)\n");
            builder.Append("  -sp\t\tStrips path information from destination file.\n");
            builder.Append("  -d\t\tSpecifies the output directory.\n");
            builder.Append("  -l\t\tSpecifies a custom list file.\n");
            builder.Append("  -q\t\tSuppress console output.\n");
            return builder.ToString();
        }

        private string CompressionRatioString(double CompressedSize, double UncompressedSize)
        {
            if (UncompressedSize == 0.0)
            {
                return "0%";
            }
            double num = CompressedSize - UncompressedSize;
            return string.Format("{0:#0}%", num / (UncompressedSize / 100.0));
        }

        private string CompressionTypeString(MpqFileFlags Flags)
        {
            if ((Flags & MpqFileFlags.CompressedMulti) != ((MpqFileFlags) 0))
            {
                return "Multi";
            }
            if ((Flags & MpqFileFlags.CompressedPK) != ((MpqFileFlags) 0))
            {
                return "PKZip";
            }
            return "";
        }

        private void CreateDirectory(string dir)
        {
            string[] strArray = dir.Split(new char[] { Path.DirectorySeparatorChar });
            if (strArray.Length > 1)
            {
                string path = strArray[0];
                for (int i = 1; i < strArray.Length; i++)
                {
                    path = path + Path.DirectorySeparatorChar + strArray[i];
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
        }

        private void ExtractArchive()
        {
            using (MpqArchive archive = new MpqArchive(this.archiveFile))
            {
                if ((this.destDir == null) || (this.destDir == ""))
                {
                    this.destDir = Path.GetTempPath();
                }
                archive.AddListfileFilenames();
                if ((this.listFile != null) && (this.listFile != ""))
                {
                    using (Stream stream = File.OpenRead(this.listFile))
                    {
                        archive.AddFilenames(stream);
                    }
                }
                byte[] buffer = new byte[0x40000];
                if (!this.quietOutput)
                {
                    Console.WriteLine("Extracting to {0}", this.destDir);
                }
                foreach (MpqEntry entry in (IEnumerable<MpqEntry>) archive)
                {
                    if ((this.regex != null) && !this.regex.Match(entry.Filename).Success)
                    {
                        continue;
                    }
                    if (!this.quietOutput)
                    {
                        Console.Write(entry.Filename + " .. ");
                    }
                    string filename = entry.Filename;
                    if (this.stripPath)
                    {
                        filename = Path.GetFileName(filename);
                    }
                    string path = Path.Combine(this.destDir, filename);
                    string directoryName = Path.GetDirectoryName(path);
                    this.CreateDirectory(directoryName);
                    using (Stream stream2 = archive.OpenFile(entry))
                    {
                        using (Stream stream3 = new FileStream(path, FileMode.Create))
                        {
                            int num;
                        Label_0135:
                            num = stream2.Read(buffer, 0, buffer.Length);
                            if (num != 0)
                            {
                                stream3.Write(buffer, 0, num);
                                goto Label_0135;
                            }
                            stream3.Close();
                        }
                    }
                    if (!this.quietOutput)
                    {
                        Console.WriteLine("Done.");
                    }
                }
            }
        }

        private void ListArchive()
        {
            using (MpqArchive archive = new MpqArchive(this.archiveFile))
            {
                archive.AddListfileFilenames();
                if ((this.listFile != null) && (this.listFile != ""))
                {
                    using (Stream stream = File.OpenRead(this.listFile))
                    {
                        archive.AddFilenames(stream);
                    }
                }
                Console.WriteLine("ucmp. size   cmp. size   ratio   cmp. type   filename");
                Console.WriteLine("----------   ---------   -----   ---------   --------");
                foreach (MpqEntry entry in (IEnumerable<MpqEntry>) archive)
                {
                    if ((this.regex == null) || this.regex.Match(entry.Filename).Success)
                    {
                        string filename = entry.Filename;
                        if (this.stripPath)
                        {
                            filename = Path.GetFileName(filename);
                        }
                        Console.WriteLine("{0, 10}   {1, 9}   {2, 5}   {3, 9}   {4}", new object[] { entry.FileSize, entry.CompressedSize, this.CompressionRatioString((double) entry.FileSize, (double) entry.CompressedSize), this.CompressionTypeString(entry.Flags), filename });
                    }
                }
            }
        }

		[STAThread]
        private static void Main(string[] args)
        {
			try
			{
				new App().Run(args);
			}
			catch (Exception error)
			{

			}
        }

        private void ParseAdditionalArgs(string[] args, int StartArgIndex)
        {
            for (int i = StartArgIndex; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-sp")
                {
                    this.stripPath = true;
                }
                else if (args[i].ToLower() == "-q")
                {
                    this.quietOutput = true;
                }
                else
                {
                    Match match = listFileExpression.Match(args[i]);
                    if (match.Success)
                    {
                        if (i < (args.Length - 1))
                        {
                            throw new ArgumentException("The list file must be the last argument!");
                        }
                        this.listFile = match.Groups["file"].Value;
                        return;
                    }
                    match = destDirExpression.Match(args[i]);
                    if (match.Success)
                    {
                        this.destDir = match.Groups["file"].Value;
                    }
                    else
                    {
                        match = regexArgExpression.Match(args[i]);
                        if (match.Success)
                        {
                            if (this.regex != null)
                            {
                                throw new ArgumentException("Do not specify multiple wildcards/regex!");
                            }
                            string pattern = match.Groups["exp"].Value;
                            this.regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled);
                        }
                        else
                        {
                            if (this.regex != null)
                            {
                                throw new ArgumentException("Do not specify multiple wildcards/regex!");
                            }
                            this.regex = new Regex(this.wildcardToRegexp(args[i]), RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        }
                    }
                }
            }
        }

        private void Run(string[] args)
        {
            string str = args[0].ToLower();
            if (str != null)
            {
                if (!(str == "-h") && !(str == "--help"))
                {
                    if (!(str == "-l") && !(str == "--list"))
                    {
                        if ((str == "-e") || (str == "--extract"))
                        {
                            if (args.Length < 2)
                            {
                                throw new ArgumentException("You must specify an archive!");
                            }
                            this.archiveFile = args[1];
                            this.ParseAdditionalArgs(args, 2);
                            if (!this.quietOutput)
                            {
                                ShowBanner();
                            }
                            this.ExtractArchive();
                        }
                        return;
                    }
                }
                else
                {
                    this.ShowHelp();
                    return;
                }
                if (args.Length < 2)
                {
                    throw new ArgumentException("You must specify an archive!");
                }
                this.archiveFile = args[1];
                this.ParseAdditionalArgs(args, 2);
                if (!this.quietOutput)
                {
                    ShowBanner();
                }
                this.ListArchive();
            }
        }

        private static void ShowBanner()
        {
            Console.WriteLine("mpqtool v0.86");
        }

        private void ShowHelp()
        {
            Console.Write(this.BuildUsage());
        }

        public string wildcardToRegexp(string wildcard)
        {
            StringBuilder builder = new StringBuilder(wildcard.Length);
            for (int i = 0; i < wildcard.Length; i++)
            {
                char ch = wildcard[i];
                switch (ch)
                {
                    case '$':
                    case '(':
                    case ')':
                    case '.':
                    case '[':
                    case '\\':
                    case ']':
                    case '^':
                    case '{':
                    case '|':
                    case '}':
                    {
                        builder.Append('\\');
                        builder.Append(ch);
                        continue;
                    }
                    case '*':
                    {
                        builder.Append('.');
                        builder.Append('*');
                        continue;
                    }
                    case '?':
                    {
                        builder.Append('.');
                        continue;
                    }
                }
                builder.Append(ch);
            }
            builder.Append('$');
            return builder.ToString();
        }
    }
}

