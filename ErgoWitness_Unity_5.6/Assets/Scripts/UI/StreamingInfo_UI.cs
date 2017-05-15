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
	//public static StreamingInfo_UI currentStreamInfo;

    public SetInfo[] infoObjects;

    //[SerializeField]
    //private int _leadboardDisplaySize = 3;

    [SerializeField] private float topYCoord = -20;
    [SerializeField] private float bottomYCoord = -80;
    [SerializeField] private float movementAmount= -20f;

    private Vector2 newPos;
    private RectTransform[] rectTransforms;

    private bool isShowing;

    public bool IsShowing { get { return isShowing; } set { isShowing = value; } }
    #endregion

    /// <summary>
    /// Set the static reference to this object and make sure that there is only one in 
    /// the current scene. Get all the rect transforms what we may need.
    ///  Initalize the  
    /// </summary>
    void Start ()
    {
        // Make sure tha thtis is the only one of these objects in the scene
        /*if (currentStreamInfo == null)
        {
            // Set the currenent referece
            currentStreamInfo = this;
        }
        else if (currentStreamInfo != this)
            Destroy(gameObject);*/

        // Get the UI components all the time
        rectTransforms = new RectTransform[infoObjects.Length];

        // Loop through and set hte rect transform array, and get the components that I need
        for (int i = 0; i < rectTransforms.Length; i++)
        {
            // Set this array to the rect transform component of the info objects
            rectTransforms[i] = infoObjects[i].GetComponent<RectTransform>();
        }

        isShowing = false;
    }

    /// <summary>
    /// Set the information of the one that is on top 
    /// Move all the other objects down
    /// </summary>
    /// <param name="newInfoObject">The object that we want to tell the player about</param>
    public void AddInfo(Source_Packet newInfoObject)
    {
        // Return if we are not showing
        if (!isShowing)
        {
            return;
        }

        if (newInfoObject.destIpInt == 0 || newInfoObject.sourceIpInt == 0)
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
    /// 
    /// Author: Ben Hoffman
    /// </summary>
    /// <param name="newInfoObject">The object that we want to tell the player about</param>
    public void AddInfo(Source newFilebeatObj)
    {        
        // Make sure that this object is valid to show, and has the proper information
        if (!isShowing || newFilebeatObj.destIpInt == 0 || newFilebeatObj.sourceIpInt == 0)
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
                if(infoObjects[i] == null)
                {
                    break;
                }
                // Change the value of this one, because it is the most recent
                infoObjects[i].SetText(newFilebeatObj);
            }

            // Set the transform component
            rectTransforms[i].anchoredPosition = newPos;
        }
    }
    

}
