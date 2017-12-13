using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UmbrellaDetector_v4 : MonoBehaviour {
    
    WebCamTexture texture;
    Texture2D textureGray;

    public int BucketSize = 50;

    // % de luminosité pour la détection de la led (savoir si elle est allumée ou éteinte)
    [Range(0.0f, 100.0f)]
    public float luminosityTolerance = 70.0f;

    public static Vector3 umbrellaCenter;
    private Vector3[] position;
    public GameObject cursorGameObject;

    private bool wasDetectedOnPreviousFrame = false;
    public bool isDetected;
    public static bool enableAction = false;
    public static bool isDebug;
    public bool interfaceIsDebug;

    public RectTransform totalCanvasTr;


    void Start()
    {
        isDebug = interfaceIsDebug;

        /*texture = new WebCamTexture(WebCamTexture.devices[1].name);
        texture.Play();
        textureGray = new Texture2D(texture.width, texture.height);*/     
    }


    void Update()
    {
        position = InputSwitch.get().getLasersPositions();
        isDetected = InputSwitch.get().hasPointer();

        if (!isDebug)
			cursorGameObject.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(position[0]);
        else
            cursorGameObject.GetComponent<RectTransform>().position = Input.mousePosition;

        //print("---" + testGameObject.GetComponent<RectTransform>().anchoredPosition);
        //print("prev:" + wasDetectedOnPreviousFrame + " / new: " + detected);

        if (!wasDetectedOnPreviousFrame && isDetected)
        {
            // ACTIVATION DE L'INTERACTION
            enableAction = true;         
        }

        wasDetectedOnPreviousFrame = isDetected;
    }


    // Récupération des positions approximatives
    // de la led
    private Vector2 GetLed2DPosition(int bucketSize)
    {
        Color[] pixels = texture.GetPixels();
        int bucket_width = Mathf.CeilToInt((float)texture.width / bucketSize);
        int bucket_height = Mathf.CeilToInt((float)texture.height / bucketSize);

        // Le bucket est une subdivision de l'image récupérée par la caméra
        // dans lequel on enregistre le niveau de gris (objectif : la subdivision la plus claire)
        float[,] bucket = new float[bucket_width, bucket_height];

        float tex_width = texture.width;
        float total_px = texture.width * texture.height;

        for (float x = 0; x < total_px; x++)
        {
            float py = Mathf.FloorToInt((float)x / tex_width);
            float px = x - (int)(py * tex_width);
            int bx = (int)(px / bucketSize);
            int by = (int)(py / bucketSize);            

            try
            {
                bucket[bx, by] += pixels[(int)x].grayscale;
            }
            catch
            {
                print(bx); print(bucket_width);
                print(by); print(bucket_height);
            }
            //textureGray.SetPixel((int)px, (int)py, new Color(pixels[(int)x].grayscale, pixels[(int)x].grayscale, pixels[(int)x].grayscale));
        }
        // textureGray.Apply();

        Vector2 maximumsPositions = new Vector2();
        //print(bucket.GetLength(0));
        //print(bucket.GetLength(1));

        for (int x = 0; x < bucket.GetLength(0); x++)
        {
            for (int y = 0; y < bucket.GetLength(1); y++)
            {
               
                if (maximumsPositions == Vector2.zero || bucket[(int)maximumsPositions.x, (int)maximumsPositions.y] < bucket[x, y])
                {
                    maximumsPositions = new Vector2(x, y);
                    break;
                }           
            }
        }

        //print(bucket[(int)maximumsPositions.x, (int)maximumsPositions.y]);

        Vector2 ledPositions = new Vector2();     
        ledPositions = maximumsPositions * bucketSize;
        ledPositions.x += bucketSize / 2;
        ledPositions.y += bucketSize / 2;

        if (bucket[(int)maximumsPositions.x, (int)maximumsPositions.y] < luminosityTolerance )
            return Vector2.zero;

        return ledPositions;
    }


    void OnGUI()
    {
        // DEBUG : caméra normale
        // GUI.DrawTexture(new Rect(0, 0, texture.width, texture.height), texture);

        // DEBUG : niveau de gris
        //GUI.DrawTexture(new Rect(0, 0, textureGray.width, textureGray.height), textureGray);
    }

}
