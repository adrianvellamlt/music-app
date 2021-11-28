using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicTheory.Extensions;

namespace MusicTheory.Models
{
    public record KeyNote(Note Note, NoteType NoteType)
    {
        private Models.Note[]? KeyNotes { get; set; }

        private void SetNotes()
        {
            var q = new Queue<Models.Note>();

            foreach (var note in Enum.GetValues(typeof(Models.Note)).Cast<Models.Note>())
            {
                q.Enqueue(note);
            }

            while (q.Peek() != Note)
            {
                q.Enqueue(q.Dequeue());
            }

            KeyNotes = q.ToArray();
        }

        public Note[] GetNotes()
        {
            if (KeyNotes == null)
            {
                SetNotes();
            }

            return KeyNotes!;
        }

        public Note NextNote()
        {
            if (KeyNotes == null)
            {
                SetNotes();
            }

            return KeyNotes![1];
        }

        public Note PreviousNote()
        {
            if (KeyNotes == null)
            {
                SetNotes();
            }

            return KeyNotes![^1];
        }

        public KeyNote Sharpen() => NoteType switch
        {
            Models.NoteType.Natural => this with { NoteType = Models.NoteType.Sharp },
            Models.NoteType.Flat => this with { NoteType = Models.NoteType.Natural},
            Models.NoteType.DoubleFlat => this with { NoteType = Models.NoteType.Flat},
            Models.NoteType.Sharp => this with { NoteType = Models.NoteType.DoubleSharp },
            Models.NoteType.DoubleSharp => new KeyNote(NextNote(), Models.NoteType.Natural),
            _ => this
        };

        public KeyNote Flatten() => NoteType switch
        {
            Models.NoteType.Natural => this with { NoteType = Models.NoteType.Flat },
            Models.NoteType.Flat => this with { NoteType = Models.NoteType.DoubleFlat},
            Models.NoteType.DoubleFlat => new KeyNote(PreviousNote(), Models.NoteType.Natural),
            Models.NoteType.Sharp => this with { NoteType = Models.NoteType.Natural },
            Models.NoteType.DoubleSharp => this with { NoteType = Models.NoteType.Sharp },
            _ => this
        };

        public string Stringify(bool ignoreNaturalSign = true) => ToString(ignoreNaturalSign);

        public override string ToString() => this.NoteType == NoteType.Natural ? Note.Stringify() : Note.Stringify() + NoteType.Stringify();

        public string ToString(bool ignoreNaturalSign)
        {
            if (ignoreNaturalSign && NoteType == NoteType.Natural) return ToString();

            return Note.Stringify() + NoteType.Stringify();
        }
    }

    public record Key
    {
        public KeyNote KeyNote { get; init; }
        private byte NoOfSharps { get; }
        private byte NoOfFlats { get; }
        private static Note[] FlatsOrder = new[] { Models.Note.B, Models.Note.E, Models.Note.A, Models.Note.D, Models.Note.G, Models.Note.C, Models.Note.F };
        private static Note[] SharpsOrder = new[] { Models.Note.F, Models.Note.C, Models.Note.G, Models.Note.D, Models.Note.A, Models.Note.E, Models.Note.B };
        private static Note[] NotesOrderedByNoOfSharps = new [] { Models.Note.G, Models.Note.D, Models.Note.A, Models.Note.E, Models.Note.B, Models.Note.F, Models.Note.C };
        private static Note[] NotesOrderedByNoOfFlats = new [] { Models.Note.F, Models.Note.B, Models.Note.E, Models.Note.A, Models.Note.D, Models.Note.G, Models.Note.C };
        public Key(Note note, NoteType noteType, bool allowDoubleSharpsAndFlats = false)
        {
            if (!allowDoubleSharpsAndFlats)
            {
                switch (noteType)
                {
                    case Models.NoteType.DoubleFlat:
                        note = note == Models.Note.A ? Models.Note.G : (Models.Note)((int)note - 1);
                        noteType = Models.NoteType.Natural;
                        break;

                    case Models.NoteType.DoubleSharp:
                        note = note == Models.Note.G ? Models.Note.A : (Models.Note)((int)note + 1);
                        noteType = Models.NoteType.Natural;
                        break;
                }
            }

            KeyNote = new KeyNote(note, noteType);

            NoOfFlats = 0;
            NoOfSharps = 0;

            switch (noteType)
            {
                case Models.NoteType.Natural:
                    switch (note)
                    {
                        case Models.Note.F: NoOfFlats = 1; break;
                        case Models.Note.C: break;
                        case Models.Note.G: NoOfSharps = 1; break;
                        case Models.Note.D: NoOfSharps = 2; break;
                        case Models.Note.A: NoOfSharps = 3; break;
                        case Models.Note.E: NoOfSharps = 4; break;
                        case Models.Note.B: NoOfSharps = 5; break;
                        default: throw new System.ArgumentException("Invalid Note.");
                    }
                    break;

                case Models.NoteType.Flat:
                case Models.NoteType.DoubleFlat:

                    var flatModifier = noteType == NoteType.Flat ? 1 : 2;

                    switch (note)
                    {
                        case Models.Note.B: NoOfFlats = (byte)(2 * flatModifier); break;
                        case Models.Note.E: NoOfFlats = (byte)(3 * flatModifier); break;
                        case Models.Note.A: NoOfFlats = (byte)(4 * flatModifier); break;
                        case Models.Note.D: NoOfFlats = (byte)(5 * flatModifier); break;
                        case Models.Note.G: NoOfFlats = (byte)(6 * flatModifier); break;
                        case Models.Note.C: NoOfFlats = (byte)(7 * flatModifier); break;
                        case Models.Note.F: NoOfFlats = (byte)(8 * flatModifier); break;
                        default: throw new System.ArgumentException("Invalid Note.");
                    }

                    if (NoOfFlats > 14) throw new System.ArgumentException("Invalid Note.");

                    break;

                case Models.NoteType.Sharp:
                case Models.NoteType.DoubleSharp:
                    
                    var sharpModifier = noteType == NoteType.Sharp ? 1 : 2;

                    switch (note)
                    {
                        case Models.Note.F: NoOfSharps = (byte)(6 * sharpModifier); break;
                        case Models.Note.C: NoOfSharps = (byte)(7 * sharpModifier); break;
                        case Models.Note.G: NoOfSharps = (byte)(8 * sharpModifier); break;
                        case Models.Note.D: NoOfSharps = (byte)(9 * sharpModifier); break;
                        case Models.Note.A: NoOfSharps = (byte)(10 * sharpModifier); break;
                        case Models.Note.E: NoOfSharps = (byte)(11 * sharpModifier); break;
                        case Models.Note.B: NoOfSharps = (byte)(12 * sharpModifier); break;
                        default: throw new System.ArgumentException("Invalid Note.");
                    }

                    if (NoOfSharps > 14) throw new System.ArgumentException("Invalid Note.");

                    break;
            }
        }

        private KeyNote[]? KeyNotes;

        public static Key? WithFlats(byte noOfFlats)
        {
            if (noOfFlats > 14) return null;

            var no = noOfFlats / 7;

            var rem = noOfFlats % 7;

            var note = rem == 0 ? NotesOrderedByNoOfFlats[6] : NotesOrderedByNoOfFlats[rem - 1];

            var noteType = NoteType.Flat;

            while (0 < no--) { noteType = noteType.Flatten(); }

            return new Key(note, noteType, allowDoubleSharpsAndFlats: true);
        }

        public static Key? WithSharps(byte noOfSharps)
        {
            if (noOfSharps > 14) return null;

            var no = noOfSharps / 7;

            var rem = noOfSharps % 7;

            var note = rem == 0 ? NotesOrderedByNoOfSharps[6] : NotesOrderedByNoOfSharps[rem - 1];

            var noteType = note == Note.F ? NoteType.Sharp : NoteType.Natural;

            while (0 < no--) { noteType = noteType.Sharpen(); }

            return new Key(note, noteType, allowDoubleSharpsAndFlats: true);
        }

        public KeyNote[] GetKeyNotes()
        {
            if (KeyNotes != null) return KeyNotes;

            var notes = KeyNote.GetNotes();

            var keyNotes = new KeyNote[notes.Length];

            var visitedIndices = new HashSet<int>();

            byte flatsIterator = NoOfFlats, sharpsIterator = NoOfSharps;

            while (flatsIterator > 0)
            {
                foreach (var flatNote in FlatsOrder)
                {
                    var index = Array.IndexOf(notes, flatNote);

                    visitedIndices.Add(index);
                    
                    var existingNote = keyNotes[index];

                    if (existingNote == null)
                    {
                        keyNotes[index] = new KeyNote(flatNote, Models.NoteType.Flat);
                    }
                    else
                    {
                        keyNotes[index] = existingNote with {  NoteType = NoteType.DoubleFlat };
                    }
                    
                    flatsIterator -= 1;

                    if (flatsIterator == 0) break;
                }
            }

            while (sharpsIterator > 0)
            {
                foreach (var sharpNote in SharpsOrder)
                {
                    var index = Array.IndexOf(notes, sharpNote);

                    visitedIndices.Add(index);

                    var existingNote = keyNotes[index];

                    if (existingNote == null)
                    {
                        keyNotes[index] = new KeyNote(sharpNote, Models.NoteType.Sharp);
                    }
                    else
                    {
                        keyNotes[index] = existingNote with {  NoteType = NoteType.DoubleSharp };
                    }
                    
                    sharpsIterator -= 1;

                    if (sharpsIterator == 0) break;
                }
            }

            if (visitedIndices.Count == keyNotes.Length) 
            {
                KeyNotes = keyNotes;

                return keyNotes;
            }

            for(var i = 0; i < notes.Length; i ++)
            {
                if (visitedIndices.Contains(i)) continue;

                keyNotes[i] = new KeyNote(notes[i], Models.NoteType.Natural);
            }

            KeyNotes = keyNotes;

            return keyNotes;
        }

        public string Stringify() => ToString();

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(KeyNote.Stringify());
            sb.AppendLine();

            var iterator = 1;
            foreach (var keyNote in GetKeyNotes())
            {
                sb.AppendLine($"{iterator++}. {keyNote.Stringify()}");
            }
            sb.AppendLine($"{iterator}. {KeyNotes![0].Stringify()}");

            return sb.ToString();
        }
    }
}