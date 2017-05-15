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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AudioManager.audioManager.PlayAudio(_MyAudioTypes.NewComputer);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AudioManager.audioManager.PlayAudio(_MyAudioTypes.Tcp);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AudioManager.audioManager.PlayAudio(_MyAudioTypes.Http);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AudioManager.audioManager.PlayAudio(_MyAudioTypes.Udp);
        }

    }

    void OnGUI()
    {
        GUI.skin.box.fontSize = 15;

        GUI.Box(new Rect(10f, 10f, 300, 80),
            "Press 1: New Computer Sound\nPress2: TCP Sound\nPress3: HTTP sound\nPress4: UDP Sound");
    }
}
