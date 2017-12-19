using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDetector : MonoBehaviour {

    public GameObject cursorGameObject;

	void Update ()
    {
        if (UmbrellaDetector_v3.isDebug)
        {
            cursorGameObject.GetComponent<RectTransform>().anchoredPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // - new Vector3(Screen.width/2, Screen.height/2, 0.0f));
        }
    }
}
