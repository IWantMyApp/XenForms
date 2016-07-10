using System.Collections.Generic;
using System.Linq;

namespace XenForms.Core.Toolbox.Project
{
    public sealed class ProjectWorkspace
    {
        public SupportedFileTypes Supports { get; }
        public PropertyEditorManager Editors { get; }
        public DesignerBridge Designer { get; }
        public ProjectState State { get; }
        public HashSet<ProjectAssembly> UserAssemblies { get; }
        public HashSet<ProjectView> UserViews { get; }


        public ProjectWorkspace(PropertyEditorManager editors, DesignerBridge designer, ProjectState state)
        {
            Editors = editors;
            Designer = designer;
            State = state;

            Supports = new SupportedFileTypes();
            UserAssemblies = new HashSet<ProjectAssembly>(new ProjectAssemblyComparer());
            UserViews = new HashSet<ProjectView>();
        }



        public void AddSupportedType(string typeName)
        {
            Editors.Add(typeName, typeof(object).FullName);
            Designer.AddSupportedType(typeName);
        }


        public void AddAssemblies(IEnumerable<ProjectAssembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                UserAssemblies.Add(assembly);
            }

            UserAssemblies.RemoveWhere(assembly => !assembly.Include && !assembly.Sent);
        }


        public void NewPage()
        {
            VisualTree.Reset();
            State.OpenFile = null;
            Designer.NewPage();
        }


        public ProjectResetResult Reset()
        {
            var result = new ProjectResetResult
            {
                AssembliesLoaded = UserAssemblies.Any(a => a.Sent)
            };

            State.Reset();
            UserAssemblies.Clear();
            UserViews.Clear();

            return result;
        }
    }
}