namespace ICSharpCode.SharpZipLib.Core
{
    using System;

    public class ScanFailureEventArgs
    {
        private bool continueRunning;
        private System.Exception exception;
        private string name;

        public ScanFailureEventArgs(string name, System.Exception e)
        {
            this.name = name;
            this.exception = e;
            this.continueRunning = true;
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

        public System.Exception Exception
        {
            get
            {
                return this.exception;
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

