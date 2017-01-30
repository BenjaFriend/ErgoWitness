using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade_UI : MonoBehaviour {

    public Text[] textItems;

    private void Start()
    {
        FadeOut();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FadeIn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            FadeOut();
        }
    }

    private void FadeOut()
    {
        for(int i = 0; i < textItems.Length; i++)
        {
            // Fade out each component
            textItems[i].CrossFadeAlpha(0f, 1f, false);
        }
    }

    private void FadeIn()
    {
        for (int i = 0; i < textItems.Length; i++)
        {
            // Fade out each component
            textItems[i].CrossFadeAlpha(1f, 1f, false);
        }
    }
}
