using System.ComponentModel;

using static Attendance.Data.AppConstants;

namespace Attendance.Data
{
    public class Global
    {
        public static int PageSize = 16;
        public static int All = 0;
    }

    public enum PageAlertType
    {
        [Description(PageAlertTypes.INFO)]
        Info = 0,

        [Description(PageAlertTypes.SUCCESS)]
        Success = 1,

        [Description(PageAlertTypes.WARNING)]
        Warning = 2,

        [Description(PageAlertTypes.DANGER)]
        Danger = 3,

        [Description(PageAlertTypes.PRIMARY)]
        Primary = 4
    }

    public enum SortDirection
    {
        Desc = 0,
        Asc = 1
    }

    public enum NocSocType
    {
        HARDWARE = 1,
        PROCESS = 2,
        HUMAN = 3
    }

    public static class AppConstants
    {
        public struct PageAlertTypes
        {
            public const string INFO = "Info!";
            public const string SUCCESS = "Success!";
            public const string WARNING = "Warning!";
            public const string DANGER = "Error!";
            public const string PRIMARY = "Primary!";
        }

        public struct AppConfigKeys
        {
            public const string SDF_PASSWORD = "SdfPassword";
        }
    }
    public enum UserRoleEnum
    {
        ADMIN = 1,
        MANAGER = 2,
        USER = 3
    }
}