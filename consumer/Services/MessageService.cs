using Consumer.Data;
using Consumer.Entities;
using Microsoft.EntityFrameworkCore;
using Consumer.Mappers;
using Consumer.Models;

namespace Consumer.Services;

public class MessageService : IMessageService
{
    private readonly DataContext _context;
    private readonly IMessageMapper _mapper;
    public MessageService(DataContext context, IMessageMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<MessageModel>> GetMessagesAsync()
    {
        var messages = await _context.Messages.ToListAsync();
        return messages.Select(x => _mapper.MapToModel(x)).ToList();
    }
    public async Task<MessageModel> GetMessageByIdAsync(int id)
    {
        var message = await _context.Messages.FirstOrDefaultAsync(x => x.Id == id);
        return _mapper.MapToModel(message);
    }
    public async Task<MessageModel> CreateMessageAsync(MessageModel message)
    {
        var messageEntity = _mapper.MapToEntity(message);
        await _context.Messages.AddAsync(messageEntity);
        await _context.SaveChangesAsync();
        return _mapper.MapToModel(messageEntity);
    }
    public async Task UpdateMessageAsync(MessageModel message)
    {
        var messageEntity = _mapper.MapToEntity(message);
        _context.Messages.Update(messageEntity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteMessageAsync(MessageModel message)
    {
        var messageEntity = _mapper.MapToEntity(message);
        _context.Messages.Remove(messageEntity);
        await _context.SaveChangesAsync();
    }
}