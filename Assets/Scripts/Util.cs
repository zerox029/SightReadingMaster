using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AlterationType
{
    NATURAL = 0,
    SHARP = 1,
    FLAT = 2,
    DOUBLESHARP = 3,
    DOUBLEFLAT = 4
}

public class KeyboardAndMidiConstants
{
    public readonly int[] baseBlackNotesIDs = new int[] { 2, 5, 7, 10, 12 };
    public readonly Dictionary<int, string> baseNoteNames = new Dictionary<int, string>()
    {
    { 1, "A" }, { 3, "B"}, { 4, "C" }, { 6, "D" }, { 8, "E" }, { 9, "F" }, { 11, "G" },
    { 2, "Alt" }, { 5, "Alt" }, { 7, "Alt" }, { 10, "Alt" }, { 12, "Alt" }
    };
    public readonly int noteAmount = 88;
    public readonly int midiStartingPoint = 21;
}