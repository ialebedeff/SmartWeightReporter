using AutoMapper;
using Database;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace SmartWeight.Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly NoteManager _noteManager;
        public NoteController(
            IMapper mapper,
            NoteManager noteManager)
        {
            _mapper = mapper;
            _noteManager = noteManager;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteNote")]
        public async Task DeleteAsync(int noteId)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteRequest"></param>
        /// <returns></returns>
        [HttpPost("CreateNote")]
        public Task<OperationResult<Note>> CreateAsync(CreateNoteRequest noteRequest)
        {
            var note = _mapper.Map<Note>(noteRequest);
            return _noteManager.CreateNoteAsync(note, User);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factoryId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet("GetNotes")]
        public Task<IEnumerable<Note>> GetNotesAsync(int factoryId, int skip, int take)
            => _noteManager.GetNotesAsync(factoryId, skip, take);
    }
}
