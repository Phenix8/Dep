using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaEmulatorMask_v4 : MonoBehaviour {

	public GameObject UmbrellaMask;

	public float MaxSizeZoom;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		UmbrellaMask.transform.position = Input.mousePosition;

		if (Input.GetAxis ("Mouse ScrollWheel") > 0f && UmbrellaMask.transform.localScale.x < MaxSizeZoom)
			UmbrellaMask.transform.localScale = UmbrellaMask.transform.localScale *= 1.05f;

		if (Input.GetAxis ("Mouse ScrollWheel") < 0f && UmbrellaMask.transform.localScale.x > 1f)
			UmbrellaMask.transform.localScale = UmbrellaMask.transform.localScale *= 0.95f;

	}
}
