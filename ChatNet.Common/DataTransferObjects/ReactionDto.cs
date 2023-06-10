using ChatNet.Common.Enumerations;

namespace ChatNet.Common.DataTransferObjects; 

public class ReactionDto {
    public Guid Id { get; set; }
    public List<Guid> Users  { get; set; } = new List<Guid>();
    public ReactionType ReactionType { get; set; }
}