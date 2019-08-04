using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    Keyboard keyboard;

    private void Start()
    {
        keyboard = new Keyboard();
    }

    private void Update()
    {
        keyboard.UpdateHeldNotes();
    }
}
