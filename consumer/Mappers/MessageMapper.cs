using Consumer.Entities;
using Consumer.Models;
namespace Consumer.Mappers
{
    public class MessageMapper : IMessageMapper
    {
        public MessageModel MapToModel(MessageEntity message)
        {
            return new MessageModel
            {
                Id = message.Id,
                Message = message.Message,
                CreatedAt = message.CreatedAt
            };
        }

        public MessageEntity MapToEntity(MessageModel message)
        {
            return new MessageEntity
            {
                Id = message.Id,
                Message = message.Message,
                CreatedAt = message.CreatedAt
            };
        }
    }
}