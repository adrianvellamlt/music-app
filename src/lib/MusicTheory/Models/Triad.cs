using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MusicTheory.Extensions;

namespace MusicTheory.Models
{
    public record Triad
    {
        public Key Key { get; }
        public Name TriadName { get; }
        protected ICollection<NoteSpelling> _ChordSpelling { get; }
        public IReadOnlyCollection<NoteSpelling> ChordSpelling { get => new ReadOnlyCollection<NoteSpelling>(_ChordSpelling.OrderBy(x => x.NoteNumber).ToList()); }
        public NoteSpelling First { get => ChordSpelling.FirstOrDefault(x => x.NoteNumber == 1) ?? throw new MalformedTriadException("Root is required."); }
        public NoteSpelling? Third { get => ChordSpelling.FirstOrDefault(x => x.NoteNumber == 3); } // null when suspended or powerchord
        public NoteSpelling Fifth { get => ChordSpelling.FirstOrDefault(x => x.NoteNumber == 5) ?? throw new MalformedTriadException("Fifth is required."); }
        public Triad(Key Key, Name Name)
        {
            this.Key = Key;
            TriadName = Name;

            var spelling = Spellings[Name];

            if (spelling.Count != 3) throw new MalformedTriadException();

            _ChordSpelling = new List<NoteSpelling>(spelling);

            if (First == null || Fifth == null)
            {
                throw new MalformedTriadException("First or Fifth are required.");
            }
        }

        private KeyNote[]? ChordNotes { get; set; }

        public KeyNote[] GetChordNotes()
        {
            if (ChordNotes != null) return ChordNotes;

            var keyNotes = Key.GetKeyNotes();

            var iterator = 0;

            var chordNodes = new KeyNote[ChordSpelling.Count];

            foreach (var note in ChordSpelling)
            {
                // catering for extensions
                var noteNumber = note.NoteNumber;

                while (noteNumber > 7) { noteNumber -= 7; }

                var keyNote = keyNotes[noteNumber - 1];

                chordNodes[iterator++] = note.NoteType switch
                {
                    NoteType.Natural => keyNote,
                    NoteType.Flat => keyNote.Flatten(),
                    NoteType.DoubleFlat => keyNote.Flatten().Flatten(),
                    NoteType.Sharp => keyNote.Sharpen(),
                    NoteType.DoubleSharp => keyNote.Sharpen().Sharpen(),
                    _ => keyNote
                };
            }

            ChordNotes = chordNodes;

            return ChordNotes;
        }

        public enum Name
        {
            [EnumMember(Value = "Major")]
            Major,
            [EnumMember(Value = "Minor")]
            Minor,
            [EnumMember(Value = "Diminished")]
            Diminished,
            [EnumMember(Value = "Augmented")]
            Augmented,
            [EnumMember(Value = "Suspended 2nd")]
            Sus2,
            [EnumMember(Value = "Suspended 4th")]
            Sus4,
            [EnumMember(Value = "5th (Power Chord)")]
            PowerChord
        };

        private static IDictionary<Name, ICollection<NoteSpelling>> Spellings =
            new Dictionary<Name, ICollection<NoteSpelling>>()
            {
                [Name.Major] = new List<NoteSpelling>()
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural)
                },
                [Name.Minor] = new List<NoteSpelling>()
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(5, NoteType.Natural)
                },
                [Name.Diminished] = new List<NoteSpelling>()
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(5, NoteType.Flat)
                },
                [Name.Augmented] = new List<NoteSpelling>()
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Sharp)
                },
                [Name.Sus2] = new List<NoteSpelling>()
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural)
                },
                [Name.Sus4] = new List<NoteSpelling>()
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural)
                },
                [Name.PowerChord] = new List<NoteSpelling>()
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(8, NoteType.Natural)
                }
            };

        public string Stringify()
        {
            var sb = new StringBuilder();

            var chordSpelling = ChordSpelling.ToArray();

            sb.AppendLine(ToString());
            sb.AppendLine(chordSpelling.Stringify());
            sb.AppendLine();

            var iterator = 0;
            foreach (var scaleNote in GetChordNotes())
            {
                var noteSpelling = chordSpelling[iterator++].Stringify();
                sb.AppendLine($"{noteSpelling} {(noteSpelling.Length == 2 ? ".." : "...")} {scaleNote.Stringify()}");
            }

            return sb.ToString();
        }

        public override string ToString() => Key.KeyNote.Stringify() + TriadName.Stringify();
    }

    public record Chord : Triad
    {
        public Chord(Triad Triad) : base(Triad.Key, Triad.TriadName) { }
        public Chord(Key Key, Name Name) : base(Key, Name) { }
        public Chord(Key Key, Name Name, IDictionary<Extension, NoteType> Extensions) : base(Key, Name)
        {
            foreach (var extension in Extensions)
            {
                AddExtension(new NoteSpelling((byte)extension.Key, extension.Value));
            }
        }

        private IDictionary<byte, NoteType> Extensions { get; } = new Dictionary<byte, NoteType>();
        public enum Extension : byte
        {
            Sixth = 6,
            Seventh = 7,
            Nineth = 9,
            Eleventh = 11,
            Thirteenth = 13
        }
        public Chord Flatten5th()
        {
            var oldFifth = Fifth;

            var newFifth = new NoteSpelling(oldFifth.NoteNumber, oldFifth.NoteType.Flatten());

            _ChordSpelling.Remove(oldFifth);

            _ChordSpelling.Add(newFifth);

            return this;
        }
        public Chord Sharpen5th()
        {
            var oldFifth = Fifth;

            var newFifth = new NoteSpelling(oldFifth.NoteNumber, oldFifth.NoteType.Sharpen());

            _ChordSpelling.Remove(oldFifth);

            _ChordSpelling.Add(newFifth);

            return this;
        }

        public Chord Add6th(NoteType noteType = NoteType.Natural)
        {
            _ChordSpelling.Add(new NoteSpelling(6, noteType));
            Extensions.Add((byte)Extension.Sixth, noteType);
            return this;
        }
        public Chord Add7th(NoteType noteType = NoteType.Natural)
        {
            // 6th with a 7th??
            if (Extensions.ContainsKey((byte)Extension.Sixth)) return this;

            _ChordSpelling.Add(new NoteSpelling(7, noteType));
            Extensions.Add((byte)Extension.Sixth, noteType);
            return this;
        }
        public Chord Add9th(NoteType noteType = NoteType.Natural)
        {
            _ChordSpelling.Add(new NoteSpelling(9, noteType));
            Extensions.Add((byte)Extension.Sixth, noteType);
            return this;
        }
        public Chord Add11th(NoteType noteType = NoteType.Natural)
        {
            _ChordSpelling.Add(new NoteSpelling(11, noteType));
            Extensions.Add((byte)Extension.Sixth, noteType);
            return this;
        }
        public Chord Add13th(NoteType noteType = NoteType.Natural)
        {
            _ChordSpelling.Add(new NoteSpelling(13, noteType));
            Extensions.Add((byte)Extension.Sixth, noteType);
            return this;
        }
        public Chord AddExtension(NoteSpelling NoteSpelling)
        {
            // 6th with 7th ??
            if (
                (Extensions.ContainsKey((byte)Extension.Sixth) && NoteSpelling.NoteNumber == (byte)Extension.Seventh) ||
                (Extensions.ContainsKey((byte)Extension.Seventh) && NoteSpelling.NoteNumber == (byte)Extension.Sixth)

            )
            {
                return this;
            }

            _ChordSpelling.Add(NoteSpelling);
            Extensions.Add(NoteSpelling.NoteNumber, NoteSpelling.NoteType);
            return this;
        }

        public override string ToString()
        {
            var fifthAltered = string.Empty;

            if (Fifth.NoteType != NoteType.Natural && TriadName != Name.Augmented && TriadName != Name.Diminished)
            {
                fifthAltered = Fifth.NoteType.Stringify() + "5";
            }

            // Chord is a triad if it has no extensions
            if (Extensions.Count == 0) return base.ToString() + fifthAltered;

            var chordSpelling = ChordSpelling.ToArray();

            NoteType? SeventhNoteType = null;

            if (Extensions.TryGetValue((byte)Extension.Seventh, out var seventh))
            {
                SeventhNoteType = seventh;
            }

            var lastNote = chordSpelling[chordSpelling.Length - 1];

            // if chord has 11th extension and does not have 6 notes, then it must be an add chord.
            var isAddChord = ChordSpelling.Count < (((lastNote.NoteNumber - 1) / 2) + 1);

            var chordName = Key.KeyNote.Stringify();

            // if it is dominant, we do not want to show the major or minor sign
            // if it is not dominant get the base triad chord name and later on add the extensions
            if (
                !(
                    Third != null &&
                    SeventhNoteType.HasValue &&
                    Third.NoteType == NoteType.Natural &&
                    SeventhNoteType == NoteType.Flat
                )
            )
            {
                chordName += TriadName.Stringify();
            }

            if (isAddChord)
            {
                var prev = SeventhNoteType.HasValue ? 7 : 5;

                var adds = string.Empty;

                foreach (var extension in Extensions.Keys.OrderBy(x => x))
                {
                    if (extension < (byte)Extension.Nineth) continue;

                    var noteType = Extensions[extension];

                    if (adds != string.Empty || !Extensions.ContainsKey((byte)(extension - 2)))
                    {
                        var noteTypeStr = noteType == NoteType.Natural ? string.Empty : noteType.Stringify();

                        adds += " add" + noteTypeStr + (byte)extension;
                    }
                    // if it already has add notes then prev is already set;
                    else if (adds == string.Empty)
                    {
                        prev = (byte)extension;
                    }
                }

                if (prev > 5) chordName += prev + fifthAltered;
                else chordName += fifthAltered;

                chordName += adds;
            }
            // if it is not an added chord just use the last extension
            // ex: if base is a major triad and the last extension is the 9th, then this is a maj9th chord.
            // ex: if the base is a minor triad and we add a flattened 7th and a 8th, then this is a dominant 9th chord.
            else
            {
                chordName += lastNote.NoteNumber + fifthAltered;
            }


            return chordName;
        }
    }

    public static class ChordExtensions
    {
        public static Chord Chordify(this Triad triad) => new Chord(triad);
    }
}