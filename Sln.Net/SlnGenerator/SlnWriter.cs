using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsSolutionGenerator.SlnGenerator.Models;
using VsSolutionGenerator.SlnGenerator.Models.Sections;

namespace VsSolutionGenerator.SlnGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class SlnWriter : StreamWriter
    {
        public const char Tab = '\t';
        public const string SectionEnd = "End";

        public SlnWriter(Stream stream) : base(stream)
        {
        }

        public SlnWriter(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public SlnWriter(Stream stream, Encoding encoding, int bufferSize) : base(stream, encoding, bufferSize)
        {
        }

        public void Write(Sln sln)
        {
            if (sln == null)
                return;

            Write(NewLine);
            Write(sln.Header);
            if (sln.Projects != null)
            {
                Write(NewLine);
                foreach (var item in sln.Projects)
                {
                    Write(item);
                    if (item == sln.Projects.Last())
                        continue;
                    Write(NewLine);
                }
            }
            if (sln.Global != null)
            {
                Write(NewLine);
                Write(sln.Global);
            }
            Write(NewLine);
            if (AutoFlush)
            {
                Flush();
            }
        }

        public void Write(ISection section, int level = 0)
        {
            if (section == null)
                return;

            string blankbefore = new string(Enumerable.Repeat(Tab, level).ToArray());

            if (blankbefore.Length > 0)
            {
                Write(blankbefore);
            }
            Write(section.TagName);
            if (!string.IsNullOrEmpty(section.Attribute))
            {
                Write($"({section.Attribute})");
            }
            if (!string.IsNullOrEmpty(section.Value))
            {
                Write($" = {section.Value}");
            }

            if (section.SubSections != null && section.SubSections.Any())
            {
                Write(NewLine);
                var newlevel = level + 1;
                var last = section.SubSections.Last();
                foreach (var item in section.SubSections)
                {
                    Write(item, newlevel);
                    if (item == last)
                        break;
                    Write(NewLine);
                }
            }

            if (!(section is GeneralSection))
            {
                Write(NewLine);
                if (blankbefore.Length > 0)
                {
                    Write(blankbefore);
                }
                Write($"{SectionEnd}{section.TagName}");
            }

            if (AutoFlush)
            {
                Flush();
            }
        }

        public void Write(VisualStudioHeader header)
        {
            if (header == null)
                return;

            Write($"Microsoft Visual Studio Solution File, Format Version {header.FormatVersion}");
            Write(NewLine);
            Write($"# Visual Studio Version {header.VisualStudioVersion}");
            Write(NewLine);
            Write($"VisualStudioVersion = {header.VisualStudioVersion}");
            Write(NewLine);
            Write($"MinimumVisualStudioVersion = {header.MinimumVisualStudioVersion}");

            if (AutoFlush)
            {
                Flush();
            }
        }

    }
}
