using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MusicTheory.Extensions;

namespace MusicTheory.Models
{
    public record NoteSpelling(byte NoteNumber, NoteType NoteType)
    {
        public override string ToString() => Stringify();
        public string Stringify(bool ignoreNaturalSign = true) 
        {
            if (ignoreNaturalSign && NoteType == NoteType.Natural) return NoteNumber.ToString();

            return NoteNumber.ToString() + NoteType.Stringify();
        }
        
    }
    public record Scale
    {
        public Key Key { get; }
        public ICollection<NoteSpelling> ScaleSpelling { get; }
        public Scale.Name? ScaleName { get; init; }
        public Scale(Key Key, ICollection<NoteSpelling> ScaleSpelling)
        {
            this.Key = Key;
            this.ScaleSpelling = ScaleSpelling;

            foreach (var scaleName in Enum.GetValues<Scale.Name>())
            {
                if (Enumerable.SequenceEqual(ScaleSpelling, Scale.Spellings[scaleName]))
                {
                    ScaleName = scaleName;
                    break;
                }
            }
        }

        public Scale(Key Key, Scale.Name ScaleName)
        {
            this.Key = Key;
            this.ScaleName = ScaleName;
            ScaleSpelling = new List<NoteSpelling>(Scale.Spellings[ScaleName]);
        }

        public enum Name
        {
            [EnumMember(Value = "major")]
            Major,
            [EnumMember(Value = "major-pentatonic")]
            MajorPentatonic,
            [EnumMember(Value = "natural-minor")]
            NaturalMinor,
            [EnumMember(Value = "natural-minor-pentatonic")]
            NaturalMinorPentatonic,
            [EnumMember(Value = "harmonic-minor")]
            HarmonicMinor,
            [EnumMember(Value = "blues")]
            Blues,
            [EnumMember(Value = "chromatic")]
            Chromatic,
            [EnumMember(Value = "dorian-mode")]
            DorianMode,
            [EnumMember(Value = "phrygian-mode")]
            PhrygianMode,
            [EnumMember(Value = "lydian-mode")]
            LydianMode,
            [EnumMember(Value = "mixolydian-mode")]
            MixoLydianMode,
            [EnumMember(Value = "locrian-mode")]
            LocrianMode,
            [EnumMember(Value = "locrian-mode-flat7")]
            LydianFlat7Mode,
            [EnumMember(Value = "whole-tone")]
            WholeTone,
            [EnumMember(Value = "jazz-melodic-minor")]
            JazzMelodic,
            [EnumMember(Value = "whole-half-diminished")]
            WholeHalfDiminished,
            [EnumMember(Value = "half-whole-diminished")]
            HalfWholeDiminished
        }

        public static IDictionary<Name, ICollection<NoteSpelling>> Spellings =
            new Dictionary<Name, ICollection<NoteSpelling>>()
            {
                [Name.Major] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Natural)
                },
                [Name.MajorPentatonic] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Natural)
                },
                [Name.NaturalMinor] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Flat),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.NaturalMinorPentatonic] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.HarmonicMinor] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Flat),
                    new NoteSpelling(7, NoteType.Natural)
                },
                [Name.Blues] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Flat),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.Chromatic] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Flat),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Sharp),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Flat),
                    new NoteSpelling(6, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Flat),
                    new NoteSpelling(7, NoteType.Natural)
                },
                [Name.DorianMode] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.PhrygianMode] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Flat),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Flat),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.LydianMode] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Sharp),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Natural)
                },
                [Name.LydianFlat7Mode] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Sharp),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.MixoLydianMode] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.LocrianMode] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Flat),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Flat),
                    new NoteSpelling(6, NoteType.Flat),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.WholeTone] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Sharp),
                    new NoteSpelling(5, NoteType.Sharp),
                    new NoteSpelling(7, NoteType.Flat)
                },
                [Name.JazzMelodic] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Natural)
                },
                [Name.WholeHalfDiminished] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Natural),
                    new NoteSpelling(3, NoteType.Flat),
                    new NoteSpelling(4, NoteType.Natural),
                    new NoteSpelling(5, NoteType.Flat),
                    new NoteSpelling(6, NoteType.Flat),
                    new NoteSpelling(7, NoteType.DoubleFlat),
                    new NoteSpelling(7, NoteType.Natural)
                },
                [Name.HalfWholeDiminished] = new NoteSpelling[]
                {
                    new NoteSpelling(1, NoteType.Natural),
                    new NoteSpelling(2, NoteType.Flat),
                    new NoteSpelling(2, NoteType.Sharp),
                    new NoteSpelling(3, NoteType.Natural),
                    new NoteSpelling(4, NoteType.Sharp),
                    new NoteSpelling(5, NoteType.Natural),
                    new NoteSpelling(6, NoteType.Natural),
                    new NoteSpelling(7, NoteType.Flat)
                }
            };

        private KeyNote[]? ScaleNotes { get; set; }

        public KeyNote[] GetScaleNotes()
        {
            if (ScaleNotes != null) return ScaleNotes;

            var keyNotes = Key.GetKeyNotes();

            var iterator = 0;

            var scaleNotes = new KeyNote[ScaleSpelling.Count];

            foreach (var note in ScaleSpelling)
            {
                var keyNote = keyNotes[note.NoteNumber - 1];

                scaleNotes[iterator++] = note.NoteType switch
                {
                    NoteType.Natural => keyNote,
                    NoteType.Flat => keyNote.Flatten(),
                    NoteType.DoubleFlat => keyNote.Flatten().Flatten(),
                    NoteType.Sharp => keyNote.Sharpen(),
                    NoteType.DoubleSharp => keyNote.Sharpen().Sharpen(),
                    _ => keyNote
                };
            }

            ScaleNotes = scaleNotes;

            return ScaleNotes;
        }
        
        public string Stringify()
        {
            var sb = new StringBuilder();

            sb.AppendLine(ToString());
            sb.AppendLine(ScaleSpelling.Stringify(addTonic: true));
            sb.AppendLine();

            var iterator = 1;
            foreach (var scaleNote in GetScaleNotes())
            {
                sb.AppendLine($"{iterator++}. {scaleNote.Stringify()}");
            }
            sb.AppendLine($"{iterator}. {ScaleNotes![0].Stringify()}");

            return sb.ToString();
        }

        public override string ToString() => Key.KeyNote.Stringify() + (ScaleName.HasValue ? (" " + ScaleName?.Stringify()) : string.Empty);
    }
}