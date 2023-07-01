using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections
{
    public class ProjectConfigPlatform
    {
        public string ConfigName { get; set; }

        public ProjectConfigPlatformValue Key { get; set; }

        public ProjectConfigPlatformValue Value { get; set; }

    }

    public class ProjectConfigPlatformValue
    {
        public string Config { get; set; }

        public string Platform { get; set; }
    }
}
