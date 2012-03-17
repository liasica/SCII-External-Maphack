namespace ICSharpCode.SharpZipLib.Core
{
    using System;

    public class ScanEventArgs : EventArgs
    {
        private bool continueRunning;
        private string name;

        public ScanEventArgs(string name)
        {
            this.name = name;
            this.ContinueRunning = true;
        }

        public bool ContinueRunning
        {
            get
            {
                return this.continueRunning;
            }
            set
            {
                this.continueRunning = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}

