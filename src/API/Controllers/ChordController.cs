using Microsoft.AspNetCore.Mvc;
using MusicTheory.Extensions;
using MusicTheory.Models;

namespace API.Controllers.Scales
{
    [ApiController]
    public class ChordController : ControllerBase
    {
        [HttpGet("triads/{note}-{noteType}/{triadName}")]
        public IActionResult GetChordNotes([FromRoute] Note note, [FromRoute] NoteType noteType, [FromRoute] Triad.Name triadName)
        {
            var key = new MusicTheory.Models.Key(note, noteType);

            var triad = new Triad(key, triadName);

            return Ok(triad.Stringify());
        }

        [HttpGet("triads/{note}/{triadName}")]
        public IActionResult GetChordNotes([FromRoute] Note note, [FromRoute] Triad.Name triadName)
        {
            var key = new MusicTheory.Models.Key(note, NoteType.Natural);

            var triad = new Triad(key, triadName);

            return Ok(triad.Stringify());
        }

        [HttpGet("chords/{chordName}")]
        public IActionResult GetChordNotes([FromRoute] string chordName)
        {
            var chord = chordName.Chordify();

            if (chord == null) return NotFound();

            return Ok(chord.Stringify());
        }
    }
}