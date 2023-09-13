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
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Specify identity column for SQL Server
    public long Id { get; set; }

    [Column("category")]
    [MaxLength(255)] // Set an appropriate max length for SQL Server
    public string? Category { get; set; }

    [Column("color")]
    [MaxLength(255)] // Set an appropriate max length for SQL Server
    public string? Color { get; set; }

    [Column("title")]
    [MaxLength(255)] // Set an appropriate max length for SQL Server
    public string? Title { get; set; }

    [Column("owner")]
    [MaxLength(255)] // Set an appropriate max length for SQL Server
    public string? Owner { get; set; }

    [Column("avatar")]
    [MaxLength(255)] // Set an appropriate max length for SQL Server
    public string? Avatar { get; set; }

    [Column("status")]
    [MaxLength(255)] // Set an appropriate max length for SQL Server
    public string? Status { get; set; }

    [Column("priority")]
    public int? Priority { get; set; } // Use int for priority, adjust data type as needed

    [Column("progress")]
    public int? Progress { get; set; } // Use int for progress, adjust data type as needed

    [Column("description")]
    [MaxLength(1000)] // Set an appropriate max length for SQL Server
    public string? Description { get; set; }

    [Column("timestamp")]
    public DateTime? Timestamp { get; set; } // Use DateTime for timestamp

    [Column("notes")]
    [MaxLength(2000)] // Set an appropriate max length for SQL Server
    public string? Notes { get; set; }

    [Column("closed")]
    public bool Closed { get; set; }
}