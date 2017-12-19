using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerScript : MonoBehaviour
{

    public GameObject cursorObject;

    public bool isEnabled = false;
    public bool isActive = false;

    private BoxCollider2D boxColliderComponent;
    private CircleCollider2D circleColliderComponent;
    private Image imageComponent;

    private Color enabledColor = new Color(0.0f, 200.0f, 0.0f, 0.5f);
    private Color disabledColor = new Color(255.0f, 255.0f, 0.0f, 0.5f);

    private Vector2 screenCenter;


    void Start ()
    {
        boxColliderComponent = GetComponent<BoxCollider2D>();
        circleColliderComponent = GetComponent<CircleCollider2D>();
        imageComponent = GetComponent<Image>();
        screenCenter = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
    }
	

	void Update ()
    {
        CheckTriggerActivation();   

    }


    // Vérifie s'il faut activer le trigger une première fois,
    // ainsi que le dézoom ensuite si on est dans la zone d'effet
    private void CheckTriggerActivation()
    {
        // Activation la 1ère fois si on est pile sur le point
        if (!isEnabled && boxColliderComponent.OverlapPoint(cursorObject.transform.position))
        {
            print("Trigger activated !");
            isEnabled = true;

            boxColliderComponent.enabled = false;
            circleColliderComponent.enabled = true;
            //imageComponent.color = disabledColor;

            // Destruction du trigger visuel avant activation
            Transform childCamera = transform.GetChild(0);
            if (childCamera != null)
                Destroy(childCamera.gameObject);
        }

        if (isEnabled)
        { 
            // Dezoom lorsqu'on est dans le cercle d'effet (on = le centre de l'écran)
            if (circleColliderComponent.OverlapPoint(Vector2.zero))
            {
                if (!isActive)
                {
                    print("Trigger in dezoom");
                    //imageComponent.color = enabledColor;
                    isActive = true;
                }
            }
            else
            {
                //imageComponent.color = disabledColor;
                isActive = false;
            }
        }
    }

}
