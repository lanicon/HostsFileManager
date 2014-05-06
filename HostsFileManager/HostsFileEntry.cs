using System;
using System.Net;

namespace HostsFileManager
{
  public class HostsFileEntry
  {
    private readonly string _line;

    public string LineEntry
    {
      get
      {
        return _line;
      }
    }

    public bool IsComment
    {
      get
      {
        return _line.Trim().StartsWith("#");
      }
    }

    public bool IsBlankLine
    {
      get
      {
        return _line.Trim().Length == 0;
      }
    }

    public bool IsEntry
    {
      get
      {
        return !IsComment && !IsBlankLine;
      }
    }

    public IPAddress IPAddress
    {
      get
      {
        if (!IsEntry)
          throw new InvalidOperationException("Cannot parse IPAddress when the entry is a comment or a blank line");

        var split = _line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length < 2)
          throw new FormatException("The entry is malformed: " + _line);

        IPAddress value;
        if (!IPAddress.TryParse(split[0], out value))
          throw new FormatException("The entry is malformed: " + split[0]);

        return value;
      }
    }

    public string HostName
    {
      get
      {
        if (!IsEntry)
          throw new InvalidOperationException("Cannot parse host name when the entry is a comment or a blank line");

        var split = _line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return split[1];
      }
    }

    public HostsFileEntry(string line)
    {
      _line = line;
    }
  }
}
