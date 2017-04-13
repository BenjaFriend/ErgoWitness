using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this as a base class for calliong a method
/// 
/// Author: Ben Hoffman
/// </summary>
public interface IVirtualButton 
{
    /// <summary>
    /// Call thsi method when we want to use the button action
    /// 
    /// Author: Ben hoffman
    /// </summary>
    void ButtonAction();

    /// <summary>
    /// Show the hover on this object, like changing the text color or something
    /// </summary>
    void ShowHover();

    /// <summary>
    /// change the text color back or something like that
    /// </summary>
    void HideHover();
}
