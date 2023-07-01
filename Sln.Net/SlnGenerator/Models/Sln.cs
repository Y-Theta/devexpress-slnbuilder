using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VsSolutionGenerator.SlnGenerator.Models.Sections.Global;
using VsSolutionGenerator.SlnGenerator.Models.Sections.Project;

namespace VsSolutionGenerator.SlnGenerator.Models
{
    /// <summary>
    /// Sln 文件
    /// </summary>
    public class Sln
    {
        private Dictionary<Guid, ProjectItemSection> _projectMap;
        private Dictionary<string, List<ProjectItemSection>> _projectNameMap;
        private List<ProjectItemSection> _projects;

        #region   
        public string FileName { get; set; }

        public VisualStudioHeader Header { get; set; }

        public IReadOnlyList<ProjectItemSection> Projects => _projects;

        public GlobalItemSection Global { get; set; }
        #endregion


        public void AddProject(ProjectItemSection item)
        {
            _projects.Add(item);
            _projectMap[item.Guid] = item;
            if (!_projectNameMap.ContainsKey(item.Name))
            {
                _projectNameMap[item.Name] = new List<ProjectItemSection>();
            }
            if (!_projectNameMap[item.Name].Contains(item))
            {
                _projectNameMap[item.Name].Add(item);
            }
        }

        public void RemoveProject(ProjectItemSection item)
        {
            if (_projectMap.ContainsKey(item.Guid))
            {
                _projects.Remove(item);
                _projectMap.Remove(item.Guid);
                if (_projectNameMap.ContainsKey(item.Name))
                {
                    _projectNameMap[item.Name].Remove(item);
                    if (_projectNameMap[item.Name].Count == 0)
                    {
                        _projectNameMap.Remove(item.Name);
                    }
                }
            }
        }

        public ProjectItemSection this[Guid guid]
        {
            get
            {
                if(_projectMap.ContainsKey(guid))
                    return _projectMap[guid];
                return null;
            }
        }

        public List<ProjectItemSection> this[string name]
        {
            get
            {
                if (_projectNameMap.ContainsKey(name))
                {
                    return _projectNameMap[name];
                }

                return null;
            }
        }

        public Sln()
        {
            _projects = new List<ProjectItemSection>();
            _projectMap = new Dictionary<Guid, ProjectItemSection>();
            _projectNameMap = new Dictionary<string, List<ProjectItemSection>>();
        }
    }
}
