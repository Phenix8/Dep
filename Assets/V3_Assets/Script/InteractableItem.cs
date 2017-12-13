using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableItem : MonoBehaviour
{
    public int id;
    public bool isDeleted = false;

    private RectTransform itemTr;
    public RectTransform cursorObjectTr;

    public AudioSource audioSource;
    public AudioClip rewardClip;
    public AudioClip soundHint;

    private Vector2 cursor_pos;
    private Vector2 my_pos;

    public void Start()
    {
        itemTr = transform.GetComponent<RectTransform>();
        isDeleted = false;
    }


    public void Update()
    {
        if (UmbrellaDetector_v3.enableAction) 
        {
            cursor_pos = cursorObjectTr.position;
            my_pos = itemTr.position;
            
            float dist = Vector2.Distance(cursor_pos, my_pos);
            //print("dist: " + dist);

            if (dist < 50)
            {
                OnPointerClick();
                UmbrellaDetector_v3.enableAction = false;
            }
        }
    }


    // Simule l'interaction dde clic lorsque la lumière est détectée
    public void OnPointerClick()
    {
                        // LE CONTROLE SUR LES  5 PREMIERS EST TEMPORAIRE
        if (!CircularMenu.isMenuOpen && !isDeleted) // && id < 5)
        {
            isDeleted = true;
            audioSource.clip = rewardClip;
            audioSource.Play();
        }
    }    

}
