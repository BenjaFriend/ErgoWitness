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
    private Dictionary<string, int> _topThree;
    private IEnumerator currentCheck;
    private bool isRunning;

    private bool isShowing = true;

    public bool IsShowing { get { return isShowing; } set { isShowing = value; } }
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

        // Initalize the dictionary
        _topThree = new Dictionary<string, int>();
    }

    /// <summary>
    /// Set the information of the one that is on top 
    /// Move all the other objects down
    /// </summary>
    /// <param name="newInfoObject">The object that we want to tell the player about</param>
    public void AddInfo(Source_Packet newInfoObject)
    {
        if(newInfoObject.destIpInt == 0 || newInfoObject.sourceIpInt == 0)
        {
            return;
        }

        if (!isShowing)
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
        if (newFilebeatObj.destIpInt == 0 || newFilebeatObj.sourceIpInt == 0)
        {
            return;
        }

        if (!isShowing)
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


    public void CheckTop(string ipAddr, int count)
    {
        // If I am not currently checking, then yea check
        if(isRunning)
        {
            // Do nothing
            return;
        }
        else
        {
            currentCheck = CheckTopHits(ipAddr, count);
            StartCoroutine(currentCheck);
        }
    }

    /// <summary>
    /// Checks the top hits for this
    /// </summary>
    /// <param name="ipAddr"></param>
    /// <param name="count"></param>
    public IEnumerator CheckTopHits(string ipAddr, int count)
    {
        isRunning = true;
        // If I already have this IP, then I want to change the value at this spot, 
        // to the updated count

        // If this count not greater then the last one we have, then discard it
        if(_topThree.Count >= _leadboardDisplaySize && count < _topThree.Values.Last())
        {
            yield break;
        }

        if (_topThree.ContainsKey(ipAddr))
        {
            _topThree[ipAddr] = count;
        }
        else
        {
            _topThree.Add(ipAddr, count);           
        }

        // Sort the dictionary and put it back on itself, with the integer value being on top
        _topThree = _topThree.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        // Print out the top 3 items
        if (_topThree.Keys.Count >= leaderboardItems.Length)
        {
            for (int i = 0; i < leaderboardItems.Length; i++)
            {
                leaderboardItems[i].SetText(
                    _topThree.Keys.ElementAt(_topThree.Keys.Count - i - 1),
                    _topThree.Values.ElementAt(_topThree.Values.Count - i - 1));
                // Wait till the next frame
                yield return null;
            }
        }          

        if(_topThree.Count > _leadboardDisplaySize + 5)
        {
            // Remove the last item
            _topThree.Remove(_topThree.Keys.Last());
        }

        isRunning = false;
    }
}
