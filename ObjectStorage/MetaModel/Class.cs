using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectStorage.MetaModel
{
    [Table("model-classes")]
    public class Class
    {
        [Key] public string Name { get; set; }
        public IList<Property> Properties { get; set; } = new List<Property>();
        public Property PresentationProperty { get; set; }
    }
}