using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will control the audio in the scene, because having 
/// an audio source on every object in the scene is crazy expensive.
/// This will be a static object, that can be referenced when we 
/// want to trigger some audio. 
/// </summary>
public class AudioManager : MonoBehaviour {

    public static AudioManager audioManager;

    // The audio source on this object
    public AudioSource newComputerSource;
    public AudioSource tcpSource;
    public AudioSource udpSource;
    public AudioSource httpSource;

    /// <summary>
    /// Get the reference to the audio source
    /// </summary>
    void Awake()
    {
        // If there are no other objects that have been assigned this static reference yet... 
        if (audioManager == null)
        {
            // Set the static reference
            audioManager = this;
        }
        // There is another object assigned already, so destroiy this
        else if (audioManager != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Play the specified type of audio from the appropriate source.
    /// Use AudioSource.PlayOneShot() so that if there was something
    /// playing before, then it will stop
    /// </summary>
    public void PlayAudio(_MyAudioTypes whichtype)
    {
        // What kind of audio is it?
        switch (whichtype)
        {
            case (_MyAudioTypes.Tcp):
                //tcpSource.Play();
                tcpSource.PlayOneShot(tcpSource.clip);
                break;
            case (_MyAudioTypes.Udp):
                udpSource.Play();
                udpSource.PlayOneShot(udpSource.clip);
                break;
            case (_MyAudioTypes.Http):
                httpSource.PlayOneShot(httpSource.clip);
                break;
            case (_MyAudioTypes.Ssh):

                break;
            case (_MyAudioTypes.NewComputer):
                newComputerSource.PlayOneShot(newComputerSource.clip);
                break;
            default:
                break;
        }
    }


}
