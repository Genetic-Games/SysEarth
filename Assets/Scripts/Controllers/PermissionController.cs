using SysEarth.Models;

namespace SysEarth.Controllers
{
    public class PermissionController
    {
        public Permission GetCustomPermission(bool canRead = false, bool canWrite = false, bool canExecute = false)
        {
            return new Permission
            {
                Read = canRead,
                Write = canWrite,
                Execute = canExecute
            };
        }

        public Permission GetPermissions(File target)
        {
            return target?.Access;
        }

        public Permission GetPermissions(Directory target)
        {
            return target?.Access;
        }

        public bool TrySetPermissions(File target, Permission access)
        {
            if (target == null)
            {
                return false;
            }

            target.Access = access;
            return true;
        }

        public bool TrySetPermissions(Directory target, Permission access)
        {
            if (target == null)
            {
                return false;
            }

            target.Access = access;
            return true;
        }
    }
}
