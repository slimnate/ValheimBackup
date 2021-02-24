using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimBackup.BO
{
    public class Backup
    {
        public string WorldName { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
    }
}
