using System.ComponentModel.DataAnnotations;

namespace ChatNet.Common.DataTransferObjects; 

public class MessageActionsDto { 
    public string TextMessage { get; set; }
    public List<Guid>? FileIds { get; set; } = new List<Guid>();
    
}