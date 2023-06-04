using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 

public class ChatPrivateCreateDto {
    [Required]
    public Guid UserId { get; set; }
    public Guid? AvatarId { get; set; }
    [Required]
    public string ChatName { get; set; }
}