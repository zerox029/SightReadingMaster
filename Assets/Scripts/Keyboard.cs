using System.Collections;
using System.Collections.Generic;
using MidiJack;
using TMPro;

public class Keyboard
{
    KeyboardAndMidiConstants knmConstants;
    public Note[] keyboardNotes;
    List<Note> heldNotes;

    public TextMeshProUGUI text;
    string noteString;


    public Keyboard()
    {
        knmConstants = new KeyboardAndMidiConstants();
        heldNotes = new List<Note>();

        int noteAmount = knmConstants.noteAmount;
        int midiStartingPoint = knmConstants.midiStartingPoint;

        keyboardNotes = new Note[noteAmount];
        
        for(int i = 0; i < noteAmount; i++)
        {
            keyboardNotes[i] = new Note(i + midiStartingPoint);
        }
    }

    public void UpdateHeldNotes()
    {
        for (int i = 0; i < keyboardNotes.Length; i++)
        {
            if (MidiMaster.GetKeyDown(keyboardNotes[i].midiNumber))
            {
                heldNotes.Add(keyboardNotes[i]);
            }
            if (MidiMaster.GetKeyUp(keyboardNotes[i].midiNumber))
            {
                heldNotes.Remove(keyboardNotes[i]);
            }
        }

        UpdateHeldNotesText();
    }

    void UpdateHeldNotesText()
    {
        noteString = "";
        text.text = noteString;

        foreach (Note note in heldNotes)
        {
            noteString += note.GetNoteName();
            noteString += ", ";
        }

        text.text = noteString;
    }
}
