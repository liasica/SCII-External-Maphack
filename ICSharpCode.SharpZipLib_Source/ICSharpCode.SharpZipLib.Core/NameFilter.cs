namespace ICSharpCode.SharpZipLib.Core
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;

    public class NameFilter
    {
        private ArrayList exclusions;
        private string filter;
        private ArrayList inclusions;

        public NameFilter(string filter)
        {
            this.filter = filter;
            this.inclusions = new ArrayList();
            this.exclusions = new ArrayList();
            this.Compile();
        }

        private void Compile()
        {
            if (this.filter != null)
            {
                string[] strArray = this.filter.Split(new char[] { ';' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    if ((strArray[i] != null) && (strArray[i].Length > 0))
                    {
                        string str;
                        bool flag = strArray[i][0] != '-';
                        if (strArray[i][0] == '+')
                        {
                            str = strArray[i].Substring(1, strArray[i].Length - 1);
                        }
                        else if (strArray[i][0] == '-')
                        {
                            str = strArray[i].Substring(1, strArray[i].Length - 1);
                        }
                        else
                        {
                            str = strArray[i];
                        }
                        if (flag)
                        {
                            this.inclusions.Add(new Regex(str, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase));
                        }
                        else
                        {
                            this.exclusions.Add(new Regex(str, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase));
                        }
                    }
                }
            }
        }

        public bool IsExcluded(string testValue)
        {
            foreach (Regex regex in this.exclusions)
            {
                if (regex.IsMatch(testValue))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsIncluded(string testValue)
        {
            if (this.inclusions.Count == 0)
            {
                return true;
            }
            foreach (Regex regex in this.inclusions)
            {
                if (regex.IsMatch(testValue))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsMatch(string testValue)
        {
            return (this.IsIncluded(testValue) && !this.IsExcluded(testValue));
        }

        public static bool IsValidExpression(string e)
        {
            bool flag = true;
            try
            {
                new Regex(e, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public static bool IsValidFilterExpression(string toTest)
        {
            bool flag = true;
            try
            {
                string[] strArray = toTest.Split(new char[] { ';' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    if ((strArray[i] != null) && (strArray[i].Length > 0))
                    {
                        string str;
                        if (strArray[i][0] == '+')
                        {
                            str = strArray[i].Substring(1, strArray[i].Length - 1);
                        }
                        else if (strArray[i][0] == '-')
                        {
                            str = strArray[i].Substring(1, strArray[i].Length - 1);
                        }
                        else
                        {
                            str = strArray[i];
                        }
                        new Regex(str, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    }
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        public override string ToString()
        {
            return this.filter;
        }
    }
}

