using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UmbrellaDetector_v3 : MonoBehaviour {
    
    WebCamTexture texture;
    Texture2D textureGray;

    public int BucketSize = 50;

    // % de luminosité pour la détection de la led (savoir si elle est allumée ou éteinte)
    [Range(0.0f, 100.0f)]
    public float tolerance = 75.0f;

    public static Vector3 umbrellaCenter;
    private Vector3[] position;
    private List<Vector2> previousPositions;
    public GameObject cursorGameObject;

    private Vector2 cursorOffset;

    private bool wasDetectedOnPreviousFrame = false;
    public bool isDetected = false;
    public static bool enableAction = false;
    public static bool isDebug;
    public bool interfaceIsDebug;

    public RectTransform globalCanvasTr;

    public GameObject debugMask;


    void Start()
    {
        isDebug = interfaceIsDebug;
        cursorOffset = new Vector3(globalCanvasTr.rect.width / 2.0f, globalCanvasTr.rect.height/2.0f);
        previousPositions = new List<Vector2>();

        if (debugMask != null)
            debugMask.SetActive(isDebug);

        /* DEBUG
        texture = new WebCamTexture(WebCamTexture.devices[1].name);
        texture.Play();
        textureGray = new Texture2D(texture.width, texture.height);
        */
    }


    void Update()
    {
        position = InputSwitch.get().getLasersPositions();
        isDetected = InputSwitch.get().hasPointer();

        if (!isDebug)
            cursorGameObject.GetComponent<RectTransform>().anchoredPosition = GetLedPositionWithTolerance();

        // ACTIVATION DE L'INTERACTION
        if (!wasDetectedOnPreviousFrame && isDetected)
            enableAction = true;         
        
        wasDetectedOnPreviousFrame = isDetected;      
    }


    // Contrôle si la nouvelle position détectée est valide
    // (dans les limites de l'écran + non-bloquée sur les bords)
    private Vector2 GetLedPositionWithTolerance()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(position[0]);
        Vector2 posInCanvas = new Vector2(
            screenPos.x * globalCanvasTr.rect.width / Screen.width,
            screenPos.y * globalCanvasTr.rect.height / Screen.height
            );

        Vector2 newPos = posInCanvas - cursorOffset;

        // Enregistrement des 10 dernières positions et vérification que le curseur n'est pas bloqué sur un bord de l'écran
        if (previousPositions.Count >= 30)
            previousPositions.Remove(previousPositions.First());        

        previousPositions.Add(newPos);

        if (newPos.x < globalCanvasTr.rect.xMin
            || newPos.x > globalCanvasTr.rect.xMax
            || newPos.y < globalCanvasTr.rect.yMin
            || newPos.y > globalCanvasTr.rect.yMax
            || newPos.Equals(previousPositions.First()))
        {
            print("New position refused.");
            return Vector2.zero;
        }
        return newPos;
    }

   
    void OnGUI()
    {
        // DEBUG : caméra normale
        // GUI.DrawTexture(new Rect(0, 0, texture.width, texture.height), texture);

        // DEBUG : niveau de gris
        //GUI.DrawTexture(new Rect(0, 0, textureGray.width, textureGray.height), textureGray);
    }

}
