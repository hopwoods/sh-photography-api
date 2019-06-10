using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Entities
{
    public class GooglePhotosPhoto
    {
        public string CameraMake { get; set; }
        public string CameraModel { get; set; }
        public double FocalLength { get; set; }
        public double ApertureFNumber { get; set; }
        public int IsoEquivalent { get; set; }
    }
}
