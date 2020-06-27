using System.Collections.Generic;
using System.Linq;
using ObjectStorage;
using ObjectStorage.MetaModel;

namespace ObjectStorageWeb.Models
{
    public class OverviewViewModel
    {
        public Class Class { get; set; }
        public List<Dictionary<string, object>> Elements { get; set; }
        public Dictionary<string, List<OptionViewModel>> Options { get; set; }

        public static OverviewViewModel create(Storage storage, string className, bool apiResponse = false)
        {
            var v = new OverviewViewModel();
            v.Class = storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            v.Elements = storage.getEntities(v.Class).Select(e =>
                {
                    var dict = v.Class.Properties.ToDictionary(p => apiResponse ? p.DisplayName : p.Name,
                        v =>
                        {
                            dynamic val = e.GetType().GetProperty(v.Name).GetValue(e);
                            if (apiResponse && !(val is string || val is int || val is float))
                            {
                                return val.Id;
                            }

                            return val;
                        });
                    dict.Add("Id", e.GetType().GetProperty("Id").GetValue(e));
                    return dict;
                })
                .ToList();
            v.Options = new Dictionary<string, List<OptionViewModel>>();
            foreach (var property in v.Class.Properties)
            {
                var c = storage.getClasses().Find(c => c.Name.ToLower().Equals(property.Type.ToLower()));
                if (c != null)
                {
                    v.Options[apiResponse ? property.DisplayName : property.Name] =
                        storage.getEntities(c).Select(e => new OptionViewModel() {Object = e}).ToList();
                }
            }

            return v;
        }
    }
}