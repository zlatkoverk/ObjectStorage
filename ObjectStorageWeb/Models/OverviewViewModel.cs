using System.Collections.Generic;
using ObjectStorage.MetaModel;

namespace ObjectStorageWeb.Models
{
    public class OverviewViewModel
    {
        public Class Class { get; set; }
        public List<object> Elements { get; set; }
    }
}