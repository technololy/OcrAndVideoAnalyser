using System;
using System.Collections.Generic;

namespace ReadTextFromImageConsole.Models
{
    public partial class SterlingBranchNew
    {
        public long Id { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchAlias { get; set; }
        public string SubDivCode { get; set; }
        public string StateId { get; set; }
        public string StateName { get; set; }
        public string RegionId { get; set; }
        public string RegionName { get; set; }
    }
}
