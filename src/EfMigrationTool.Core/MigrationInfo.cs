using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfMigrationTool.Core
{
    public class MigrationInfo
    {
        public string MigrationId { get; set; }
        public byte[]  ModelBlobb { get; set; }

        public MigrationSource Source { get; set; }
    }
}
