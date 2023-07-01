using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.SlnGenerator.Models.Sections
{
    public interface ISection
    {
        string TagName { get; }

        string Attribute { get; }

        string Value { get; }

        IEnumerable<ISection> SubSections { get; }

        ISection Parent { get; }

    }
}
