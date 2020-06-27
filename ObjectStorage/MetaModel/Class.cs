using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Slugify;

namespace ObjectStorage.MetaModel
{
    [Table("model-classes")]
    public class Class
    {
        private static SlugHelper _slugHelper;
        private string _displayName;

        static Class()
        {
            var config = new SlugHelper.Config();
            config.StringReplacements[" "] = "_";
            _slugHelper = new SlugHelper(config);
        }

        [Key] public string Name { get; set; }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                Name = "_" + _slugHelper.GenerateSlug(value);
            }
        }

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