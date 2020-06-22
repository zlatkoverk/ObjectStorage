using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;

namespace ObjectStorage.MetaModel
{
    [Table("model-classes")]
    public class Class
    {
        [Key] public string Name { get; set; }
        public IList<Property> Properties { get; set; } = new List<Property>();
        public Property PresentationProperty { get; set; }
        public string OverviewTemplate { get; set; } = File.ReadAllText(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Template/Overview.liquid"));
        public string DetailsTemplate { get; set; } = File.ReadAllText(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Template/Details.liquid"));
    }
}