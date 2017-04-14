using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Valve.VR;

namespace Valve.VR.InteractionSystem
{

    public class MonitorControlls : MonoBehaviour
    {
		public Sprite playSprite;     // The sprite for if we want to play or not
		public Sprite pauseSprite;    // The sprite for if we want to pause
		public Button pausePlayButton;// The button for toggling on and off with moniotring

		private bool isMonitoring;

		/// <summary>
		/// Toggles the monitors and switchs between the play and pause 
		/// sprite.
		/// 
		/// Author: Ben Hoffman
		/// </summary>
		public void ToggleMonitors()
		{
			// If we are monitoring, then stop
			if (isMonitoring)
			{
				// Make sure that we know that we are not monitoring anymore
				isMonitoring = false;

				// Stop monitoring
				ManageMonitors.currentMonitors.StopMonitor();

				// Set the play button as active
				pausePlayButton.image.sprite = playSprite;
			}
			// Start monitoring again
			else
			{
				pausePlayButton.image.sprite = pauseSprite;

				// Stop all coroutines first becore we start monitoring
				ManageMonitors.currentMonitors.StopMonitor();

				// Make sure that we know that we are monitoring now
				isMonitoring = true;

				// Start monitoring
				ManageMonitors.currentMonitors.StartMonitoringObjects();

			}
		}


		#region Application settings
		/// <summary>
		/// Just reload the current scene, which will reset everything
		/// </summary>
		public void Reset()
		{
			// Use the scene manager to reset the scene
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
		}


		/// <summary>
		/// Author: Ben Hoffman
		/// Exit the application
		/// </summary>
		public void Quit()
		{
			// Quit the application
			Application.Quit();
		}

		#endregion
			
    }
}