namespace ChatNet.Common.DataTransferObjects; 

public class ChatListDto {
    public Pagination<ChatShortDto> UserChats { get; set; }
}