using System;

namespace Main.Models;

public class ModelBase {
    public string? Id { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedTime { get; set; } = DateTime.UtcNow;

    public void GenerateId() {
        this.Id = Guid.NewGuid().ToString();
    }
}