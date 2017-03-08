using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour {

    public Text ipText;
    public Text countText;

    private void Start()
    {
        ipText.text = "";
        countText.text = "";
    }

    /// <summary>
    /// Sets the text of the UI elements
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="count"></param>
    public void SetText(string ip, int count)
    {
        try
        {
            ipText.text = ip;
            countText.text = count.ToString();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }


}
