using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ObjectStorage.MetaModel
{
    [Table("model-classes")]
    public class Class
    {
        [Key] public string Name { get; set; }
        public IList<Property> Properties { get; set; } = new List<Property>();
        public Property PresentationProperty { get; set; }
        [JsonIgnore]
        public string OverviewTemplate { get; set; } = File.ReadAllText(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Template/Overview.liquid"));
        [JsonIgnore]
        public string DetailsTemplate { get; set; } = File.ReadAllText(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Template/Details.liquid"));
    }
}