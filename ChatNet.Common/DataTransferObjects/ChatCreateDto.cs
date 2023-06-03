using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 

public class ChatCreateDto {
    public List<Guid> Users { get; set; } = new List<Guid>();
    public Guid? AvatarId { get; set; }
    [Required]
    public string ChatName { get; set; }
}