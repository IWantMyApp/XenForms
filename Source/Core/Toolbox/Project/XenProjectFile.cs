using Newtonsoft.Json;
using XenForms.Core.FileSystem;

namespace XenForms.Core.Toolbox.Project
{
    public class XenProjectFile
    {
        private readonly IFileSystem _fs;

        [JsonProperty("schema")]
        public string Schema { get; set; }

        [JsonProperty("assemblies")]
        public ProjectAssembly[] Assemblies { get; set; }

        [JsonProperty("views")]
        public ProjectView[] Views { get; set; }


        public XenProjectFile(IFileSystem fs)
        {
            _fs = fs;
        }


        public void Save(string fileName)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            _fs.WriteAllText(fileName, json);
        }


        public bool Load(string fileName)
        {
            string json;
            var found = _fs.ReadAllText(fileName, out json);
            if (!found) return false;

            var tmp = JsonConvert.DeserializeObject<XenProjectFile>(json);

            Schema = tmp.Schema;
            Assemblies = tmp.Assemblies;
            Views = tmp.Views;

            return true;
        }
    }
}