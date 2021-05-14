using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageValidationBlazorServer.Models
{
    public class WebCamOptions
    {
        public int Width { get; set; } = 320;
        public string VideoID { get; set; }
        public string CanvasID { get; set; }
        public string Filter { get; set; } = null;
    }
}
