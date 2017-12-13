using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureScript : MonoBehaviour {

    public Texture pictureTexture;

	void Start ()
    {
        //float xRatio = GetComponent<SpriteRenderer>().sprite.rect.width * 100 / Camera.main.pixelWidth;
        float heightRatio = Camera.main.pixelHeight / GetComponent<RectTransform>().rect.height;

        //transform.localScale = new Vector3(xRatio, yRatio, 1.0f);
        //transform.position = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, -1.0f);     
    }
	
	void Update () {
		
	}

    void OnGUI()
    {
        //GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), pictureTexture);
    }
}
