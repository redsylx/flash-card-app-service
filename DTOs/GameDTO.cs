using System.Collections.Generic;

namespace Main.DTO.GameDto;

public class CreateGameDTO {
    public List<string> CategoryIds { get; set; } = [];
    public string AccountId { get; set; } = "";
    public int NCard { get; set; } = 0;
    public int HideDurationInSecond { get; set; } = 0;
}