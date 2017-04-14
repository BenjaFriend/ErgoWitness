using UnityEngine;
using System.Collections;
using Valve.VR;

namespace Valve.VR.InteractionSystem
{

    public class MonitorControlls : MonoBehaviour
    {
        private Coroutine gripHintRoutine;

        // Show some hint for testing purposes
        public void TestButton( Hand hand )
        {
            if(gripHintRoutine != null)
            {
                StopCoroutine( gripHintRoutine );
            }

            gripHintRoutine = StartCoroutine( ShowGripHint( hand ) );
        }

        private IEnumerator ShowGripHint( Hand hand )
        {
            while( true )
            {
                ControllerButtonHints.ShowTextHint( hand, EVRButtonId.k_EButton_Grip, "This is a test!!" );
                yield return new WaitForSeconds( 3.0f );
                ControllerButtonHints.HideAllTextHints( hand );
            }
        }
    }
}