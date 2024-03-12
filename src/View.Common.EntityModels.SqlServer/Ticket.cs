using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace View.Shared;

[Table("tickets")]
public partial class Ticket
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column("category")]
    [MaxLength(255)]
    public string? Category { get; set; }

    [Column("color")]
    [MaxLength(255)]
    public string? Color { get; set; }

    [Column("title")]
    [MaxLength(255)]
    public string? Title { get; set; }

    [Column("owner")]
    [MaxLength(255)]
    public string? Owner { get; set; }

    [Column("avatar")]
    [MaxLength(255)]
    public string? Avatar { get; set; }

    [Column("status")]
    [MaxLength(255)]
    public string? Status { get; set; }

    [Column("priority")]
    public int? Priority { get; set; }

    [Column("progress")]
    public int? Progress { get; set; }

    [Column("description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("timestamp")]
    public DateTime? Timestamp { get; set; }

    [Column("notes")]
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Column("closed")]
    public bool Closed { get; set; }
}