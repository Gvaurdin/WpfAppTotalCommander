using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppTotalCommander.MVVM.Model
{
    public class DriveItem
    {
        public DriveInfo Drive { get; set; }

        public DriveItem(DriveInfo drive)
        {
            Drive = drive;
        }
    }
}
