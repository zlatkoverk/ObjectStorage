using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ObjectStorage.MetaModel
{
    [Table("model-properties")]
    public class Property
    {
        [Key] public Guid Id { get; set; }
        [Column] public string Name { get; set; }
        [Column] public string Type { get; set; }
    }
}