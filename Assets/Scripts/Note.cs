using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{
    KeyboardAndMidiConstants knmConstants;
    public int midiNumber;
    public float frequency;
    public float period;

    public Note(int _midiNumber)
    {
        knmConstants = new KeyboardAndMidiConstants();

        midiNumber = _midiNumber;
        frequency = SetFrequency();
        period = SetPeriod();
    }

    float SetFrequency()
    {
        float lFrequency = Mathf.Pow(2, (midiNumber - 69) / 12) * 440;

        return lFrequency;
    }

    //Period of the wave in miliseconds
    float SetPeriod()
    {
        float lPeriod = (1 / frequency) * 1000;

        return lPeriod;
    }

    public string GetNoteName(AlterationType alteration = AlterationType.NATURAL)
    {
        AlterationType alterationType = alteration;
        int id = midiNumber - 20;
        int octave = FindOctave(id);
        int baseNoteID = id - (octave * 12);

        //If ID is on black key
        if (System.Array.Exists(knmConstants.baseBlackNotesIDs, element => element == baseNoteID))
        {
            //If no alteration is specified, use sharp
            if (alteration == AlterationType.NATURAL)
            {
                alterationType = AlterationType.SHARP;
            }

            //If alteration is double-flat/double-sharp, change to sharp and notify user
            if(alteration == AlterationType.DOUBLESHARP || alteration == AlterationType.DOUBLEFLAT)
            {
                alterationType = AlterationType.SHARP;
                Debug.Log("Cannot have double alteration starting on a black note");
            }
        }

        //Just return the base name if there is no alteration
        if (alterationType == AlterationType.NATURAL)
        {
            return knmConstants.baseNoteNames[baseNoteID];
        }

        return CasesForAlterations(alterationType, baseNoteID);
    }

    string CasesForAlterations(AlterationType alterationType, int baseNoteID)
    {
        //Loop through every kind of alteration and make the necessary adjustement
        switch (alterationType)
        {
            case (AlterationType.SHARP):
                return knmConstants.baseNoteNames[baseNoteID - 1] + "♯";
            case (AlterationType.FLAT):
                return knmConstants.baseNoteNames[baseNoteID + 1] + "♭";
            case (AlterationType.DOUBLESHARP):
                return knmConstants.baseNoteNames[baseNoteID - 2] + "♯♯"; //?? double sharp symbol doesnt work??
            case (AlterationType.DOUBLEFLAT):
                return knmConstants.baseNoteNames[baseNoteID - 2] + "♭♭"; //?? double flat symbol doesnt work??
            default:
                Debug.LogError("I have no idea what you did to see this message but congratulations, you broke the system");
                return "";
        }
    }

    int FindOctave(int id)
    {
        for (int i = 1; i <= 8; i++)
        {
            if ((float)id / i <= 12f)
            {
                return i - 1;
            }
        }

        Debug.LogError("Note out of range");
        return 0;
    }
}
