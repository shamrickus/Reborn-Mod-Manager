using System;
using System.Linq;

namespace DotaInstaller.src.Services
{
    public static class VersionManager
    {
        public static Version BuildFromString(string pVersion)
        {
            var parts = pVersion.Split('.').ToList().Select(Int16.Parse).ToList();
            return new Version(parts[0], parts[1], parts[2]);
        }
    }
}
