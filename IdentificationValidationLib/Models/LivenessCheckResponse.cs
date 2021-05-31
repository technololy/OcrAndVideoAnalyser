using System;
namespace IdentificationValidationLib.Models
{
    public class LivenessCheckResponse
    {
        public bool HeadNodingDetected { get; set; }
        public bool HeadShakingDetected { get; set; }
        public bool HeadRollingDetected { get; set; }
        public bool HasFaceSmile { get; set; }
    }
}
