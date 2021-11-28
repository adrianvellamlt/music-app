using Microsoft.AspNetCore.Mvc;
using MusicTheory.Models;

namespace API.Controllers.Keys
{
    [ApiController]
    [Route("keys")]
    public class KeyController : ControllerBase
    {
        [HttpGet("{note}-{noteType}")]
        public IActionResult GetKeyNotes([FromRoute] Note note, [FromRoute] NoteType noteType)
        {
            var key = new MusicTheory.Models.Key(note, noteType);

            return Ok(key.Stringify());
        }

        [HttpGet("{note}")]
        public IActionResult GetNaturalKeyNotes([FromRoute] Note note)
        {
            var key = new MusicTheory.Models.Key(note, NoteType.Natural);

            return Ok(key.Stringify());
        }

        [HttpGet("search")]
        public IActionResult FindKey([FromQuery] byte? noOfSharps, [FromQuery] byte? noOfFlats)
        {
            Key? key = null;

            if (noOfSharps.HasValue)
            {
                key = Key.WithSharps(noOfSharps.Value);
            }

            if (noOfFlats.HasValue)
            {
                key = Key.WithFlats(noOfFlats.Value);
            }

            if (key == null) return NotFound();

            return Ok(key.Stringify());
        }
    }
}