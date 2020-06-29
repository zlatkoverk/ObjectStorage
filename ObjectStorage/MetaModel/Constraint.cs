using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectStorage.MetaModel
{
    [Table("model-constraints")]
    public class Constraint
    {
        [Key] public Guid Id { get; set; }
        [Column] public float MinValue { get; set; }
        [Column] public float MaxValue { get; set; }
    }
}