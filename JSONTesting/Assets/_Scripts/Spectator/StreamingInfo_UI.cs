using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will take in a data object, and display the
/// informatino in the streaming information UI like
/// Norse. 
/// </summary>
public class StreamingInfo_UI : MonoBehaviour {

    #region Fields
	public static StreamingInfo_UI currentStreamInfo;

    public SetInfo[] infoObjects;
    public LeaderboardItem[] leaderboardItems;

    [SerializeField]
    private int _leadboardDisplaySize = 5;

    [SerializeField] private float topYCoord = -20;
    [SerializeField] private float bottomYCoord = -80;
    [SerializeField] private float movementAmount= -20f;

    private Vector2 newPos;
    private RectTransform[] rectTransforms;    
    #endregion

    // Use this for initialization
    void Start ()
    {
		// Set the static reference
		currentStreamInfo = this;
        // Set all of the texts to ""
        // Set the rect transform array, so that I don't need to
        // Get the component all the time
        rectTransforms = new RectTransform[infoObjects.Length];

        // Loop through and set hte rect transform array
        for(int i = 0; i < rectTransforms.Length; i++)
        {
            // Set this array to the rect transform component of the info objects
            rectTransforms[i] = infoObjects[i].GetComponent<RectTransform>();
            infoObjects[i].ClearText();
        }
	}


    /// <summary>
    /// Set the information of the one that is on top 
    /// Move all the other objects down
    /// </summary>
    /// <param name="newInfoObject">The object that we want to tell the player about</param>
    public void AddInfo(Source_Packet newInfoObject)
    {
        if (newInfoObject.sourceIpInt == -1 || newInfoObject.destIpInt == -1 || newInfoObject.runtime_timestamp == null)
        {
            return;
        }
            // I need to move all of the objects down by a certain amount
            for (int i = 0; i < rectTransforms.Length; i++)
        {
            // Get a reference to the variable
            newPos = rectTransforms[i].anchoredPosition;

            // Alter the value
            newPos.y += movementAmount;

            // If the new position is past the bottom, then just move
            // this object to the top
            if(newPos.y < bottomYCoord)
            {
                newPos.y = topYCoord;
                // Change the value of this one, because it is the most recent
                infoObjects[i].SetText(newInfoObject);
            }

            // Set the transform component
            rectTransforms[i].anchoredPosition = newPos;
        }
    }

    /// <summary>
    /// Set the information of the one that is on top 
    /// Move all the other objects down
    /// </summary>
    /// <param name="newInfoObject">The object that we want to tell the player about</param>
    public void AddInfo(Source newFilebeatObj)
    {
        if(newFilebeatObj.sourceIpInt == -1 || newFilebeatObj.destIpInt == -1 || newFilebeatObj.runtime_timestamp == null)
        {
            return;
        }

        // I need to move all of the objects down by a certain amount
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            // Get a reference to the variable
            newPos = rectTransforms[i].anchoredPosition;

            // Alter the value
            newPos.y += movementAmount;

            // If the new position is past the bottom, then just move
            // this object to the top
            if (newPos.y < bottomYCoord)
            {
                newPos.y = topYCoord;
                // Change the value of this one, because it is the most recent
                infoObjects[i].SetText(newFilebeatObj);
            }

            // Set the transform component
            rectTransforms[i].anchoredPosition = newPos;
        }
    }

    /// <summary>
    /// Checks the top hits for what is the best
    /// </summary>
    public void CheckTopHits(string ipInteger, int numHits)
    {
        int newScore = numHits;
        string newName = ipInteger;
        string oldName = "";
        int oldScore = 0;

        // If I don't have this IP, then I dont care
        // Set the UI now to what we have
        for (int i = 0; i < _leadboardDisplaySize; i++)
        {
            // If this key exists then keep checking it
            if (PlayerPrefs.HasKey(i + "HScoreName"))
            {
                // If the number of hits that we got is greater then
                // what is currently in spot 'i' of the leaderboard
                if (newScore > PlayerPrefs.GetInt(i + "HScore"))
                {
                    // Store the old values in temp variables
                    oldScore = PlayerPrefs.GetInt(i + "HScore");
                    oldName = PlayerPrefs.GetString(i + "HScoreName");

                    // Set the current spot 'i' to the values that are new
                    PlayerPrefs.SetInt(i + "HScore", newScore);
                    PlayerPrefs.SetString(i + "HScoreName", newName);

                    // Set the 'new' temp variables to the old ones
                    newScore = oldScore;
                    newName = oldName;
                }
            }
            // This sport in the leaderbaord does not exist yet! We need to add it
            else
            {
                // Set this value to the IP address that we know. 
                // Now spot 'i' in the leaderboard is the IP address
                PlayerPrefs.SetString(i + "HScoreName", ipInteger);

                // Set the value of spot 'i' in the leaderboard to
                // the number of hits that we have
                PlayerPrefs.SetInt(i + "HScore", numHits);

                // We are done So let's break out of this loop
                break;
            }

        }

        // Set the UI now to what we have
        for(int i = 0; i < leaderboardItems.Length; i++)
        {
            leaderboardItems[i].SetText(PlayerPrefs.GetString(i + "HScoreName"), PlayerPrefs.GetInt( i + "HScore"));
        }
        //Debug.Log("The highest score is: " + PlayerPrefs.GetString(0 + "HScoreName") + "  " + PlayerPrefs.GetInt(0 + "HScore"));
    }

    /// <summary>
    /// Write out the most recent query, and most recent timestamp at
    /// the end of the application
    /// </summary>
    void OnApplicationQuit()
    {
        // Clear the player prefs when we quit
        PlayerPrefs.DeleteAll();
    }
}
