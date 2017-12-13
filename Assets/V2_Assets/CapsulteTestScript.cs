using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsulteTestScript : MonoBehaviour {

    Rigidbody rb;

    Vector3 positionAdding = new Vector3();
    Random r = new Random();

    public float multiplicator = 0.5f;
    

    // Update is called once per frame
    void Update ()
    {

        positionAdding = new Vector3(0.0f, Mathf.Cos(Time.time) * multiplicator, 0.0f);
        gameObject.transform.position += positionAdding;

    }
}
