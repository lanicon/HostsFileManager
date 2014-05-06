using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace HostsFileManager
{
  public static class HostsFile
  {
    public static bool HasWriteAccess
    {
      get
      {
        return new AccessControl(new DirectoryInfo(Path)).HasWriteAccess;
      }
    }

    private static readonly string Path = System.IO.Path.Combine(Environment.SystemDirectory, "drivers", "etc", "hosts");

    public static IEnumerable<HostsFileEntry> GetEntries()
    {
      return File.ReadAllLines(Path).Select(lineEntry => new HostsFileEntry(lineEntry));
    }

    public static bool AddEntry(string hostName)
    {
      return AddEntry("127.0.0.1", hostName);
    }

    public static bool AddEntry(string ipAddress, string hostName)
    {
      return AddEntry(IPAddress.Parse(ipAddress), hostName);
    }

    public static bool AddEntry(IPAddress ipAddress, string hostName)
    {
      Uri uri;
      if (!Uri.TryCreate(Uri.UriSchemeHttp + "://" + hostName, UriKind.Absolute, out uri))
      {
        throw new ArgumentException("The host name is invalid", "hostName");
      }

      HostsFileEntry newEntry = new HostsFileEntry(ipAddress + " " + hostName);

      List<HostsFileEntry> entries = new List<HostsFileEntry>(GetEntries());
      bool exists = false;
      bool dirty = false;
      for (int i = 0; i < entries.Count; i++)
      {
        HostsFileEntry entry = entries[i];
        if (entry.IsEntry && entry.HostName.Equals(hostName, StringComparison.InvariantCultureIgnoreCase))
        {
          if (!entry.IPAddress.ToString().Equals(ipAddress.ToString()))
          {
            entries[i] = newEntry;
            dirty = true;
          }
          exists = true;
          break;
        }
      }

      if (!exists)
      {
        entries.Add(newEntry);
        dirty = true;
      }

      if (dirty)
      {
        FileInfo fi = new FileInfo(Path);
        FileAttributes originalAttributes = fi.Attributes;
        fi.Attributes = FileAttributes.Normal;
        try
        {
          using (StreamWriter textFileWriter = new StreamWriter(Path))
          {
            entries.ForEach(e => textFileWriter.WriteLine(e.LineEntry));
          }
        }
        finally
        {
          fi.Attributes = originalAttributes;
        }
      }

      return true;
    }
  }
}
