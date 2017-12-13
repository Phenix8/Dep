using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour {


    public GameObject cursorImage;
	

	void Update ()
    {
        if (cursorImage != null)
            cursorImage.transform.position = Input.mousePosition;
	}

}
