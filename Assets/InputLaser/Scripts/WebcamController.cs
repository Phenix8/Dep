using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamController : MonoBehaviour {

    WebCamTexture texture;
    Texture2D textureGray;

    public int BucketSize = 50;

    public static Vector3 umbrellaCenter;
    

    void Start () {
        texture = new WebCamTexture(WebCamTexture.devices[1].name);
        texture.Play();
        textureGray = new Texture2D(texture.width, texture.height);
	}


    void Update()
    {
        umbrellaCenter = GetUmbrellaCenter(); // en pixels
        umbrellaCenter.x /= texture.width;
        umbrellaCenter.y /= texture.height;
        umbrellaCenter.x = Mathf.Abs(1 - umbrellaCenter.x);
        print(umbrellaCenter);

        // on passe du viewport caméra à une position world
        umbrellaCenter.z = 1;
        umbrellaCenter = Camera.main.ViewportToWorldPoint(umbrellaCenter);
    }


    // Récupération des positions approximatives
    // des 3 leds
    private Vector2[] getLed2DPosition(int bucketSize)
    {
        Color[] pixels = texture.GetPixels();
        int bucket_width = texture.width / bucketSize;
        int bucket_height = texture.height / bucketSize;
        float[,] bucket = new float[bucket_width, bucket_height];

        for (int x=0; x<texture.width * texture.height; x++)
        {
            int py = (int)(x / texture.width);
            int px = x - py * texture.width;
            int bx = px / bucketSize;
            int by = py / bucketSize;
            

            try {
                bucket[bx, by] += pixels[x].grayscale;
            } catch
            {
                print(bx);print(bucket_width);
                print(by); print(bucket_height);
            }
            textureGray.SetPixel(px, py, new Color(pixels[x].grayscale, pixels[x].grayscale, pixels[x].grayscale));
        }

        textureGray.Apply();

        Vector2[] maximumsPositions = new Vector2[3];

        for (int x = 0; x < bucket.GetLength(0); x++)
        {
            for (int y = 0; y < bucket.GetLength(1); y++)
            {
                for (int i = 0; i < maximumsPositions.Length; i++)
                {
                    if (maximumsPositions[i] == Vector2.zero || bucket[(int)maximumsPositions[i].x, (int)maximumsPositions[i].y] < bucket[x, y])
                    {
                        maximumsPositions[i] = new Vector2(x, y);
                        break;
                    }
                }
            }
        }

        Vector2[] ledPositions = new Vector2[maximumsPositions.Length];

        for (int i=0; i<ledPositions.Length; i++)
        {
            ledPositions[i] = maximumsPositions[i] * bucketSize;
            ledPositions[i].x += bucketSize / 2;
            ledPositions[i].y += bucketSize / 2;
        }

        return ledPositions;
    }


    // Récupère les coordonnées du centre 
    // du parapluie
    public Vector2 GetUmbrellaCenter()
    {
        Vector2[] ledPositions = getLed2DPosition(BucketSize);
        //return ledPositions[0];
       // print(ledPositions[0] + "|" + ledPositions[1] + "|" + ledPositions[2]);

        if (ledPositions.Length < 3)
            return new Vector2();

        float AB = Vector2.Distance(ledPositions[0], ledPositions[1]);
        float BC = Vector2.Distance(ledPositions[1], ledPositions[2]);
        float CA = Vector2.Distance(ledPositions[2], ledPositions[0]);

        // AB diamètre
        if (AB > BC && AB > CA)           
            return new Vector2((ledPositions[0].x + ledPositions[1].x)/2, (ledPositions[0].y + ledPositions[1].y) / 2); 
                      
        // BC diamètre
        else if (BC > CA)           
            return new Vector2((ledPositions[1].x + ledPositions[2].x) / 2, (ledPositions[1].y + ledPositions[2].y) / 2);

        // CA diamètre
        else
            return new Vector2((ledPositions[2].x + ledPositions[0].x) / 2, (ledPositions[2].y + ledPositions[0].y) / 2);     
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, textureGray.width, textureGray.height), textureGray);
    }

}
