// ***********************************
// ** Application Info Library v1.0 **
// ***********************************
// ** Created: December 22, 2020    **
// ** Guillermo Musumeci            **
// ***********************************
// ** Updated: February 01, 2021    **
// ** Guillermo Musumeci            **
// ***********************************

using System;
using System.Collections.Generic;
using System.Text;

namespace DetectNET
{
    class AppInfo
    {
        #region "Variables"
        public static string AppName = "DetectNET";
        public static string AppVersion = "1.00";
        public static string AppBuild = "2021.07.26";
        public static string AppDeveloper = "Guillermo Musumeci";
        public static string AppCompany = "KopiCloud";
        public static string AppVersionFull = AppVersion + " Build " + AppBuild;
        public static string AppNameMsg = AppName + " v" + AppVersionFull;
        public static string AppFullName = AppCompany + " " + AppNameMsg;
        #endregion
    }
}
