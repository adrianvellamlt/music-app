using System.Runtime.Serialization;

namespace MusicTheory.Models
{
    public enum Note : byte
    {
        [EnumMember(Value = "a")]
        A = 1,
        [EnumMember(Value = "b")]
        B = 2,
        [EnumMember(Value = "c")]
        C = 3,
        [EnumMember(Value = "d")]
        D = 4,
        [EnumMember(Value = "e")]
        E = 5,
        [EnumMember(Value = "f")]
        F = 6,
        [EnumMember(Value = "g")]
        G = 7
    }

    public enum NoteType : byte
    {
        [EnumMember(Value = "natural")]
        Natural,
        [EnumMember(Value = "sharp")]
        Sharp,
        [EnumMember(Value = "double-sharp")]
        DoubleSharp,
        [EnumMember(Value = "flat")]
        Flat,
        [EnumMember(Value = "double-flat")]
        DoubleFlat
    }
}