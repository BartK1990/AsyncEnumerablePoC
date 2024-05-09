using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsyncEnumerablePoC.Server.DataAccess;
public abstract record DatabaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [Column(Order = 0)]
    public int Id { get; set; }
}
