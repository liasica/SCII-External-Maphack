using System;
using System.Collections.Generic;
using System.Text;
using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace maphack_external_directx.screen_utilities
{
    class hudManager
    {
        // screen info
        public static int resolutionHeight = 0;
        public static int resolutionWidth = 0;
        public static int numOfScreens = 0;
        
        public void screenInfo()
        {
            if (resolutionHeight == 0 && resolutionWidth == 0 && numOfScreens == 0)
            {
                Screen[] screens = Screen.AllScreens;
                resolutionWidth  = Screen.PrimaryScreen.Bounds.Width;
                resolutionHeight = Screen.PrimaryScreen.Bounds.Height;
                numOfScreens = screens.Count();
            }
        }


        //set position code

        public void setPosition()
        {
            setPosition_();
        }

        private void setPosition_()
        {

        }


    }
}
