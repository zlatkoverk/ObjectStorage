using System;
using System.Collections.Generic;
using System.Linq;
using ObjectStorage;
using ObjectStorage.MetaModel;

namespace ObjectStorageWeb.Models
{
    public class DetailsViewModel
    {
        public Class Class { get; set; }
        public Dictionary<string, object> Element { get; set; }
        public Dictionary<string, List<OptionViewModel>> Options { get; set; }
        // public bool Editable { get; set; } = true;

        public static DetailsViewModel create(Storage storage, string className, string entityId)
        {
            var o = storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
 
            Guid id;
            if (!Guid.TryParse(entityId, out id))
            {
                return null;
            }


            var v = new DetailsViewModel();
            v.Class = storage.getClasses().Find(c => c.Name.ToLower().Equals(className.ToLower()));
            v.Element = storage.getEntities(v.Class)
                .FindAll(e => e.GetType().GetProperty("Id").GetValue(e).Equals(id)).Select(e =>
                {
                    var dict = v.Class.Properties.ToDictionary(p => p.Name,
                        v => e.GetType().GetProperty(v.Name).GetValue(e));
                    dict.Add("Id", e.GetType().GetProperty("Id").GetValue(e));
                    return dict;
                }).First();
            v.Options = new Dictionary<string, List<OptionViewModel>>();
            foreach (var property in v.Class.Properties)
            {
                var c = storage.getClasses().Find(c => c.Name.ToLower().Equals(property.Type.ToLower()));
                if (c != null)
                {
                    v.Options[property.Name] =
                        storage.getEntities(c).Select(e => new OptionViewModel() {Object = e}).ToList();
                }
            }

            return v;
        }
    }
}