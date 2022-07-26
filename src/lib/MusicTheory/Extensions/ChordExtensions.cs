using System;
using System.Collections.Generic;
using System.Linq;
using MusicTheory.Models;

namespace MusicTheory.Extensions
{
    public static class ChordExtensions
    {
        public static NoteType Flatten(this NoteType noteType) => noteType switch
        {
            NoteType.Natural => NoteType.Flat,
            NoteType.Flat => NoteType.DoubleFlat,
            NoteType.DoubleFlat => throw new ArgumentException("Cannot flatten further"),
            NoteType.Sharp => NoteType.Natural,
            NoteType.DoubleSharp => NoteType.Sharp,
            _ => noteType
        };
        public static NoteType Sharpen(this NoteType noteType) => noteType switch
        {
            NoteType.Natural => NoteType.Sharp,
            NoteType.Flat => NoteType.Natural,
            NoteType.DoubleFlat => NoteType.Flat,
            NoteType.Sharp => NoteType.DoubleSharp,
            NoteType.DoubleSharp => throw new ArgumentException("Cannot sharpen further"),
            _ => noteType
        };
        public static string Stringify(this NoteType noteType)
        {
            const string natural = "♮", flatSign = "♭", doubleFlatSign = "𝄫", sharpSign = "♯", doubleSharpSign = "𝄪";

            var sign = noteType switch
            {
                Models.NoteType.Natural => natural,
                Models.NoteType.Sharp => sharpSign,
                Models.NoteType.DoubleSharp => doubleSharpSign,
                Models.NoteType.Flat => flatSign,
                Models.NoteType.DoubleFlat => doubleFlatSign,
                _ => string.Empty
            };

            return sign;
        }
        public static string Stringify(this Note note) => note.ToString().ToUpper();
        public static string Stringify(this Scale.Name ScaleName) => ScaleName switch
        {
            Scale.Name.MajorPentatonic => "Major Pentatonic",
            Scale.Name.NaturalMinor => "Natural Minor",
            Scale.Name.NaturalMinorPentatonic => "Natural Minor Pentatonic",
            Scale.Name.HarmonicMinor => "Harmonic Minor",
            Scale.Name.Blues => "Blues",
            Scale.Name.Chromatic => "Chromatic",
            Scale.Name.DorianMode => "Dorian Mode",
            Scale.Name.PhrygianMode => "Phrygian Mode",
            Scale.Name.LydianMode => "Lydian Mode",
            Scale.Name.MixoLydianMode => "MixoLydian Mode",
            Scale.Name.LocrianMode => "Locrian Mode",
            Scale.Name.LydianFlat7Mode => "Lydian Flat7 Mode",
            Scale.Name.WholeTone => "Whole Tone",
            Scale.Name.JazzMelodic => "Jazz Melodic Minor",
            Scale.Name.WholeHalfDiminished => "Whole Half Diminished",
            Scale.Name.HalfWholeDiminished => "Half Whole Diminished",
            Scale.Name.Altered => "Altered",
            _ => "Unknown"
        };
        public static string Stringify(this ICollection<NoteSpelling> ScaleSpelling, bool addTonic = false)
        {
            var str = string.Join(" - ", ScaleSpelling.Select(x => x.ToString()));

            NoteSpelling first = ScaleSpelling.First(), last = ScaleSpelling.Last();

            if (addTonic && first != last)
            {
                var tonic = (first with { NoteNumber = (byte)(last.NoteNumber + 1) });

                str += " - " + tonic.ToString();
            }

            return str;
        }
        public static string Stringify(this Triad.Name Name, bool shortened = true) => Name switch
        {
            Triad.Name.Major => shortened ? "Δ" : "maj",
            Triad.Name.Minor => shortened ? "−" : "min",
            Triad.Name.Sus2 => "sus2",
            Triad.Name.Sus4 => "sus4",
            Triad.Name.Augmented => shortened ? "+" : "aug",
            Triad.Name.Diminished => shortened ? "°" : "dim",
            Triad.Name.PowerChord => "5",
            _ => "Unknown"
        };
        public static Chord? Chordify(this string chordName)
        {
            const string Major = "major";
            const string Minor = "minor";
            const string Diminished = "dim";
            const string Dominant = "dominant";
            const string Sharp = "sharp";
            const string Flat = "flat";
            const string DoubleSharp = "doublesharp";
            const string DoubleFlat = "doubleflat";
            const string SupportedNoteTypes = Sharp + ":" + Flat + ":" + DoubleSharp + ":" + DoubleFlat;

            chordName = chordName.ToLower();

            var chordNameSections = chordName.Split("-", StringSplitOptions.RemoveEmptyEntries);

            if (chordNameSections.Length < 2) return null;

            if (chordNameSections[0].Length > 1)
            {
                throw new MalformedChordException("Root note could not be parsed");
            }

            var rootNote = ParseNote(chordNameSections[0]);

            var noteType = NoteType.Natural;

            var skipSections = 1;

            if (SupportedNoteTypes.Contains(chordNameSections[1].ToLower()))
            {
                noteType = ParseNoteType(chordNameSections[1]);

                skipSections++;
            }

            var key = new Key(rootNote, noteType);

            chordNameSections = chordNameSections.Skip(skipSections).ToArray();

            Chord? chord = null;

            var isTriad = false;

            Note ParseNote(string note) => note.ToLower() switch
            {
                "a" => Note.A,
                "b" => Note.B,
                "c" => Note.C,
                "d" => Note.D,
                "e" => Note.E,
                "f" => Note.F,
                "g" => Note.G,
                _ => throw new ArgumentException("Invalid note")
            };

            NoteType ParseNoteType(string noteType) => noteType.ToLower() switch
            {
                Flat => NoteType.Flat,
                DoubleFlat => NoteType.DoubleFlat,
                Sharp => NoteType.Sharp,
                DoubleSharp => NoteType.DoubleSharp,
                _ => NoteType.Natural
            };

            void AddAddedNotes(string addPart)
            {
                if (chord == null) return;

                var addedNotes = addPart.Split("add", StringSplitOptions.RemoveEmptyEntries);

                foreach (var addedNote in addedNotes)
                {
                    byte extensionNumber;

                    NoteType? noteType = null;

                    if (addedNote.Contains("sharp") && byte.TryParse(addedNote.Replace("sharp", string.Empty), out extensionNumber))
                    {
                        noteType = NoteType.Sharp;
                    }
                    else if (addedNote.Contains("flat") && byte.TryParse(addedNote.Replace("flat", string.Empty), out extensionNumber))
                    {
                        noteType = NoteType.Flat;
                    }
                    else if (byte.TryParse(addedNote, out extensionNumber))
                    {
                        noteType = NoteType.Natural;
                    }

                    if (noteType.HasValue)
                    {
                        chord.AddExtension(new NoteSpelling(extensionNumber, noteType.Value));
                    }
                }
            }

            void AddAlteredFifth(string alteredFifthPart)
            {
                if (chord == null) return;

                if (alteredFifthPart.Contains("sharp5th"))
                {
                    if (isTriad && chord.Third?.NoteType == NoteType.Natural)
                    {
                        chord = new Chord(key, Triad.Name.Augmented);
                    }
                    else chord.Sharpen5th();
                }
                else if (alteredFifthPart.Contains("flat5th"))
                {
                    if (isTriad && chord.Third?.NoteType == NoteType.Flat)
                    {
                        chord = new Chord(key, Triad.Name.Diminished);
                    }
                    else chord.Flatten5th();
                }
            }

            void SetChord(string name, Triad.Name baseTriad, NoteType seventhNoteType)
            {
                chord = new Triad(key, baseTriad).Chordify();

                if (byte.TryParse(chordNameSections[0].Replace(name, string.Empty), out var extension))
                {
                    while (extension > 5)
                    {
                        chord.AddExtension(new NoteSpelling(extension, extension == 7 ? seventhNoteType : NoteType.Natural));

                        extension -= 2;
                    }
                }
                else isTriad = true;


                if (chordNameSections.Length > 1)
                {
                    AddAlteredFifth(chordNameSections[1]);
                }

                var lastPart = chordNameSections.Last();

                if (lastPart.Contains("add")) AddAddedNotes(lastPart);
            }

            if (chordNameSections[0].Contains(Major))
            {
                SetChord(Major, Triad.Name.Major, NoteType.Natural);
            }
            else if (chordNameSections[0].Contains(Minor))
            {
                SetChord(Minor, Triad.Name.Minor, NoteType.Flat);
            }
            else if (chordNameSections[0].Contains(Diminished))
            {
                SetChord(Diminished, Triad.Name.Diminished, NoteType.DoubleFlat);
            }
            else if (chordNameSections[0].Contains(Dominant))
            {
                SetChord(Dominant, Triad.Name.Major, NoteType.Flat);
            }

            return chord;
        }
    }
}