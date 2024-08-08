using System;
using System.ComponentModel.DataAnnotations;

namespace Main.Models;

public class ModelBase {
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedTime { get; set; } = DateTime.UtcNow;
}