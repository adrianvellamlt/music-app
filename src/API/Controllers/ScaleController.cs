using Microsoft.AspNetCore.Mvc;
using MusicTheory.Models;

namespace API.Controllers.Scales
{
    [ApiController]
    [Route("scales")]
    public class ScaleController : ControllerBase
    {
        [HttpGet("{note}-{noteType}/{scaleName}")]
        public IActionResult GetKeyNotes([FromRoute] Note note, [FromRoute] NoteType noteType, [FromRoute] Scale.Name scaleName)
        {
            var key = new MusicTheory.Models.Key(note, noteType);

            var scale = new Scale(key, scaleName);

            return Ok(scale.Stringify());
        }

        [HttpGet("{note}/{scaleName}")]
        public IActionResult GetNaturalKeyNotes([FromRoute] Note note, [FromRoute] Scale.Name scaleName)
        {
            var key = new MusicTheory.Models.Key(note, NoteType.Natural);

            var scale = new Scale(key, scaleName);

            return Ok(scale.Stringify());
        }
    }
}