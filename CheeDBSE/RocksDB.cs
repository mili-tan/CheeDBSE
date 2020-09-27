using System;
using System.IO;
using RocksDbSharp;

namespace CheeDBSE
{
    public class RocksDB
    {
        private static string SetupBasePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public static RocksDb DB = RocksDb.Open(new DbOptions().SetCreateIfMissing().SetCreateMissingColumnFamilies(),
            SetupBasePath + "my.db");
        public static string IndexStr = File.Exists(SetupBasePath + "index.html")
            ? File.ReadAllText(SetupBasePath + "index.html")
            : "Welcome to CheeDBS";
        public static string SecretPath = File.Exists(SetupBasePath + "secret.txt")
            ? File.ReadAllText(SetupBasePath + "secret.txt")
            : "";
        public static bool CacheEnable = false;
    }
}
