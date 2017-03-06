using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script will take in a data object, and display the
/// informatino in the streaming information UI like
/// Norse.
/// </summary>
public class StreamingInfo_UI : MonoBehaviour {

    #region Fields
    public SetInfo[] infoObjects;

    public float topYCoord = -20;
    public float bottomYCoord = -80;
    public float movementAmount= -20f;

    private Vector2 newPos;
    private RectTransform[] rectTransforms;
    #endregion

    // Use this for initialization
    void Start ()
    {
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
        // I need to move all of the objects down by a certain amount
        for(int i = 0; i < rectTransforms.Length; i++)
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
}
