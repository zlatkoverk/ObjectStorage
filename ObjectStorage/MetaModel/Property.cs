using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace ObjectStorage.MetaModel
{
    [Table("model-properties")]
    public class Property
    {
        private static SlugHelper _slugHelper;

        static Property()
        {
            var config = new SlugHelper.Config();
            config.StringReplacements[" "] = "_";
            _slugHelper = new SlugHelper(config);
        }

        [Key] public Guid Id { get; set; }
        [Column] public string DisplayName { get; set; }
        [Column] public Constraint Constraint { get; set; }

        [Column]
        public string Name
        {
            get => "_" + _slugHelper.GenerateSlug(DisplayName);
        }

        [Column] public string Type { get; set; }
    }
}