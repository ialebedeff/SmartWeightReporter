using Database.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Database;

public class NoteManager
{
    private readonly NoteRepository _noteRepository;
    private readonly FactoryManager _factoryManager;
    private readonly UserManager<User> _userManager;
    public NoteManager(
        FactoryManager factoryManager,
        NoteRepository noteRepository, 
        UserManager<User> userManager)
    {
        _noteRepository = noteRepository;
        _userManager = userManager;
        _factoryManager = factoryManager;
    }

    public async Task<OperationResult<Note>> CreateNoteAsync(Note note, ClaimsPrincipal author)
    {
        var user = await _userManager.GetUserAsync(author);
        var factory = await _factoryManager.GetFactoryAsync(note.FactoryId);
        if (user is null || factory is null) { return OperationResult<Note>.Failed("Не найден пользователь или завод"); }

        note.User = user;
        note.Factory = factory;

        return await _noteRepository.Add(note);
    }

    public Task<IEnumerable<Note>> GetNotesAsync(int factoryId, int skip, int take)
        => _noteRepository.GetNotesAsync(factoryId, skip, take);
}
