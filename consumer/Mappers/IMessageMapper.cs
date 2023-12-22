using Consumer.Entities;
using Consumer.Models;
namespace Consumer.Mappers
{
    public interface IMessageMapper
    {
        MessageModel MapToModel(MessageEntity message);
        MessageEntity MapToEntity(MessageModel message);
    }
}