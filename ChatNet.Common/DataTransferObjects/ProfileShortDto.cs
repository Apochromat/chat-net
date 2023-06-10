namespace ChatNet.Common.DataTransferObjects; 
/// <summary>
/// Short profile DTO
/// </summary>
public class ProfileShortDto {
 /// <summary>
 /// Id
 /// </summary>
 public Guid Id { get; set; }
 
 /// <summary>
 /// Photo Identifier
 /// </summary>
 public Guid PhotoId { get; set; }
 
 /// <summary>
 /// User`s full name (surname, name, patronymic)
 /// </summary>
 public required string FullName { get; set; }
 
}