using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will allow me to test the audio manager
/// with different key inputs
/// </summary>
public class TestAudioWithInput : MonoBehaviour {

	
	// Update is called once per frame
	void Update ()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            AudioManager.audioManager.PlayAudio(_MyAudioTypes.NewComputer);
            Debug.Log("Played new computer sound!");
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            AudioManager.audioManager.PlayAudio(_MyAudioTypes.Tcp);
            Debug.Log("Played TCP sound!");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AudioManager.audioManager.PlayAudio(_MyAudioTypes.Http);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AudioManager.audioManager.PlayAudio(_MyAudioTypes.Udp);
        }

    }
}
