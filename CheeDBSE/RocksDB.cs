using System;
using RocksDbSharp;

namespace CheeDBSE
{
    public class RocksDB
    {
        private static string SetupBasePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public static RocksDb DB = RocksDb.Open(new DbOptions().SetCreateIfMissing().SetCreateMissingColumnFamilies(),
            SetupBasePath + "my.db");
    }
}
