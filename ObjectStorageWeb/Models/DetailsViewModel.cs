using System.Collections.Generic;
using ObjectStorage.MetaModel;

namespace ObjectStorageWeb.Models
{
    public class DetailsViewModel
    {
        public Class Class { get; set; }
        public Dictionary<string, object> Element { get; set; }
        public Dictionary<string, List<OptionViewModel>> Options { get; set; }
        public bool Editable { get; set; } = true;
    }
}