using Consumer.Entities;
using Consumer.Models;

namespace Consumer.Services;

public interface IMessageService
{
    Task<MessageModel> CreateMessageAsync(MessageModel message);
    Task<MessageModel> GetMessageByIdAsync(int id);
    Task<List<MessageModel>> GetMessagesAsync();
    Task UpdateMessageAsync(MessageModel message);
    Task DeleteMessageAsync(MessageModel message);
}
