using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HostsFileManager;
using NUnit.Framework;

namespace HostsFileManagerTest
{
  [TestFixture]
  public class HostsFileTest
  {
    [Test]
    public void Test()
    {
      var entries = HostsFile.GetEntries().ToList().Where(f => f.IsEntry);
    }

    [Test]
    public void AddEntry()
    {
      //HostsFile.AddEntry("kabam.pndtest.com");

      bool hasAccess = HostsFile.HasWriteAccess;
      
    }
  }
}
