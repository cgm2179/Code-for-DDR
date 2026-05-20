using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    // I changed this to 4 to match your 4 arrows/sensors
    public KeyCode[] laneKeys = new KeyCode[4];

    [Header("Arduino Integration")]
    public ArduinoController arduinoController; // Drag your ArduinoController here in the Inspector!

    // We need arrays to track the state from the previous frame
    private bool[] currentArduinoState = new bool[4];
    private bool[] previousArduinoState = new bool[4];

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        // Update our Arduino states every frame
        if (arduinoController != null)
        {
            // 1. Shift current state to previous state
            for (int i = 0; i < 4; i++)
            {
                previousArduinoState[i] = currentArduinoState[i];
            }

            // 2. Grab the newest state from the ArduinoController
            currentArduinoState[0] = arduinoController.arrow1Pressed;
            currentArduinoState[1] = arduinoController.arrow2Pressed;
            currentArduinoState[2] = arduinoController.arrow3Pressed;
            currentArduinoState[3] = arduinoController.arrow4Pressed;
        }
    }

    public bool GetStartPressed()
    {
        return GetLaneDown(3); // right arrow
    }

    public bool GetLaneDown(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= laneKeys.Length) return false;

        bool keyboardHit = Input.GetKeyDown(laneKeys[laneIndex]);
        bool arduinoHit = false;

        // It is a "Down" hit if it is pressed NOW, but was NOT pressed LAST frame
        if (arduinoController != null)
        {
            arduinoHit = currentArduinoState[laneIndex] && !previousArduinoState[laneIndex];
        }

        // Return true if EITHER the keyboard or the Arduino sensor was hit
        return keyboardHit || arduinoHit;
    }

    public bool GetLaneUp(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= laneKeys.Length) return false;

        bool keyboardUp = Input.GetKeyUp(laneKeys[laneIndex]);
        bool arduinoUp = false;

        // It is an "Up" release if it is NOT pressed NOW, but WAS pressed LAST frame
        if (arduinoController != null)
        {
            arduinoUp = !currentArduinoState[laneIndex] && previousArduinoState[laneIndex];
        }

        return keyboardUp || arduinoUp;
    }

    public bool GetLaneHeld(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= laneKeys.Length) return false;

        bool keyboardHeld = Input.GetKey(laneKeys[laneIndex]);
        bool arduinoHeld = false;

        // It is "Held" if it is currently pressed
        if (arduinoController != null)
        {
            arduinoHeld = currentArduinoState[laneIndex];
        }

        return keyboardHeld || arduinoHeld;
    }

    public void RebindLaneKey(int laneIndex, KeyCode newKey)
    {
        if (laneIndex < 0 || laneIndex >= laneKeys.Length) return;
        laneKeys[laneIndex] = newKey;
    }
}