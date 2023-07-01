using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections
{
    public class GeneralSection : ISection
    {
        string ISection.TagName => Key;

        string ISection.Attribute => string.Empty;

        IEnumerable<ISection> ISection.SubSections => null;

        public string Key { get; set; }

        public string Value { get; set; }

        public ISection Parent { get; set; }

        public GeneralSection(string key, string value)
        {
            Key = key;
            Value = value;
        }   
    }
}
