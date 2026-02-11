using System;
using System.IO;
using System.Security.Principal;

namespace CreateShortCut.Utilities
{
    public static class SecurityUtility
    {
        public static bool IsRunningAsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        public static bool HasDirectoryAccess(string path)
        {
            try
            {
                Directory.GetDirectories(path);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
