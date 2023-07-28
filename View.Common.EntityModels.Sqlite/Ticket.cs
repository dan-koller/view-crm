using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace View.Shared;

[Table("tickets")]
public partial class Ticket
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("color")]
    public string? Color { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("owner")]
    public string? Owner { get; set; }

    [Column("avatar")]
    public string? Avatar { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("priority")]
    public long? Priority { get; set; }

    [Column("progress")]
    public long? Progress { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("timestamp")]
    public string? Timestamp { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("closed")]
    public bool Closed { get; set; }
}
