using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace HostsFileManager
{
  internal class AccessControl
  {
    private readonly DirectoryInfo directory;
    private readonly WindowsPrincipal principal;

    public DirectoryInfo Directory
    {
      get
      {
        return directory;
      }
    }

    public WindowsPrincipal Principal
    {
      get
      {
        return principal;
      }
    }

    public bool HasReadAccess
    {
      get
      {
        return HasAccess(FileSystemRights.Read);
      }
    }

    public bool HasWriteAccess
    {
      get
      {
        return HasAccess(FileSystemRights.Write);
      }
    }

    public bool HasModifyAccess
    {
      get
      {
        return HasAccess(FileSystemRights.Modify);
      }
    }

    public AccessControl(DirectoryInfo directory)
      : this(directory, new WindowsPrincipal(WindowsIdentity.GetCurrent()))
    {
    }

    public AccessControl(DirectoryInfo directory, WindowsPrincipal principal)
    {
      if (directory == null)
        throw new ArgumentNullException("directory");

      if (principal == null)
        throw new ArgumentNullException("principal");

      this.directory = directory;
      this.principal = principal;
    }

    public bool HasAccess(FileSystemRights rights)
    {
      WindowsIdentity user = (WindowsIdentity)principal.Identity;
      // Get the collection of authorization rules that apply to the specified directory
      AuthorizationRuleCollection acl = directory.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));

      // These are set to true if either the allow access or deny access rights are set
      bool allowAccess = false;
      bool denyAccesss = false;

      foreach (FileSystemAccessRule currentRule in acl)
      {
        // If the current rule applies to the current user
        if (user.User.Equals(currentRule.IdentityReference) || principal.IsInRole((SecurityIdentifier)currentRule.IdentityReference))
        {
          if (currentRule.AccessControlType.Equals(AccessControlType.Deny))
          {
            if ((currentRule.FileSystemRights & rights) == rights)
            {
              denyAccesss = true;
            }
          }
          else if (currentRule.AccessControlType.Equals(AccessControlType.Allow))
          {
            if ((currentRule.FileSystemRights & rights) == rights)
            {
              allowAccess = true;
            }
          }
        }
      }

      return allowAccess & !denyAccesss;
    }
  }
}
