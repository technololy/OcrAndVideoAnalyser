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
        public string Filter { get; set; } = "contrast(1.4) sepia(0.2) blur(3px) saturate(200%) hue-rotate(200deg)";

        public string Preview { get; set; }
        public string Recording { get; set; }
        public string StartButton { get; set; }
        public string StopButton { get; set; }
        public string DownloadButton { get; set; }
        public string LogElement { get; set; }
    }
}
