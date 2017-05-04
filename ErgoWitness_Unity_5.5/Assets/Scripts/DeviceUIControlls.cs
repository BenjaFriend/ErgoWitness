using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Ben Hoffman
/// This is so that each device has the same basic functionality
/// of being able to fade something in and out, and then for each
/// device it can have individual UI elements to set. 
/// </summary>
public abstract class DeviceUIControlls : MonoBehaviour {

    private Animator anim;
    public Image backgroundImage;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public virtual void SetValue() { }

    public void FadeOut(Text[] infoitems)
    {
        anim.SetTrigger("FadeIn");
    }

    public void FadeIn(Text[] infoItems)
    {
        anim.SetTrigger("FadeOut");
    }
}
