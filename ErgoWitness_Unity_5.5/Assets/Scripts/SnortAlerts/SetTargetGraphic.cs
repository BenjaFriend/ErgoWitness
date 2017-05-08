using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTargetGraphic : Toggle {

    public Image myTargetGraphic;

	// Use this for initialization
	void OnEnable() {
        targetGraphic = myTargetGraphic;

    }
	
	
}
