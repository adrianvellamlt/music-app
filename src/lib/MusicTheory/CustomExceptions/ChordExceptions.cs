using System;

namespace MusicTheory
{
    public class MalformedTriadException : Exception
    {
        public MalformedTriadException() : base("At least 3 notes are required for a triad.") { }
        public MalformedTriadException(string message) : base(message) { }
    }

    public class MalformedChordException : Exception
    {
        public MalformedChordException() : base("One or more parameters lead to a malformed chord.") { }
        public MalformedChordException(string message) : base(message) { }
    }
}