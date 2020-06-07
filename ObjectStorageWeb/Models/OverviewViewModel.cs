using System.Collections.Generic;
using ObjectStorage.MetaModel;

namespace ObjectStorageWeb.Models
{
    public class OverviewViewModel
    {
        public Class Class { get; set; }
        public List<Dictionary<string, object>> Elements { get; set; }
    }
}