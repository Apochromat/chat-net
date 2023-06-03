using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 

public class ChatEditDto {
    [Required]
    public Guid AvatarId { get; set; }
    [Required]
    public string ChatName { get; set; }
}