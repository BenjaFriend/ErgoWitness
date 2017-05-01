using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// This script will allow us to control the volume of different mix groups
/// via their exposed paramaters and change them with a slider.
/// </summary>
public class MixLevels : MonoBehaviour {

    // The audio mixer that we will use
    public AudioMixer masterMixer;

    /// <summary>
    /// Sett the master volume in the mixer
    /// </summary>
    /// <param name="volLevl">The volume that the mixer will be set to</param>
	public void SetMasterVol(float volLevl)
    {
        masterMixer.SetFloat("MasterVolume", volLevl);
    }

    /// <summary>
    /// Set the Ambient noise level
    /// </summary>
    /// <param name="volLevel">The volume that the mixer will be set to</param>
    public void SetAmbientVolume(float volLevel)
    {
        masterMixer.SetFloat("AmbientVolume", volLevel);
    }

    /// <summary>
    /// Sets the volume level of the netflow
    /// </summary>
    /// <param name="volLevel">The volume that the mixer will be set to</param>
    public void SetNetflowVolume(float volLevel)
    {
        masterMixer.SetFloat("NetflowVolume", volLevel);
    }
}
