using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour {

    public GameObject MonsterPrefab;
    public GameObject clicIndicator;
    private List<GameObject> monstersList = new List<GameObject>();

    public List<Transform> spawnSpots;
    private int spawnIndex = 0;
    public static int monsterNb = 0;

    private float lastTimeGeneration;
    public float generationDelay = 4.0f;

    private bool isMonsterGenerated = false;

    
	void Start () {
        lastTimeGeneration = Time.time;
        clicIndicator.SetActive(false);
    }
	
	void Update ()
    {
        /*      1 CLIC = DESTRUCTION DU MONSTRE AFFICHé

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            clicIndicator.SetActive(true);
            if (monsterNb > 0)
            { 
                GameObject monsterToDestroy = monstersList[monstersList.Count - 1];
                monstersList.RemoveAt(monstersList.Count - 1);
                Destroy(monsterToDestroy);
                monsterNb--;
                isMonsterGenerated = false;
            }
        }
        else
            clicIndicator.SetActive(false);

        */

        //CheckLasers();
        GenerateMonster();
        CheckContactWithUmbrella();
	}


    // Fonction de génération d'un monstre
    private void GenerateMonster()
    {
        if (Time.time - lastTimeGeneration >= generationDelay)
        {
            lastTimeGeneration = Time.time;
            spawnIndex = (spawnIndex >= spawnSpots.Count - 1) ? 0 : spawnIndex + 1;
            if (monsterNb >= spawnSpots.Count || isMonsterGenerated)
                return;

            GameObject generatedMonster = Instantiate(MonsterPrefab);
            generatedMonster.transform.position = spawnSpots[spawnIndex].position;
            monstersList.Add(generatedMonster);

            monsterNb++;
            isMonsterGenerated = true;
        }
    }

    // Vérifie si le monstra est en contact approximatif
    // avec le centre du parapluie
    private void CheckContactWithUmbrella()
    {
        

        RaycastHit hit;
        Vector3 dir = WebcamController.umbrellaCenter - Camera.main.transform.position;
        if (Physics.Raycast(Camera.main.transform.position, dir, out hit))
        {
            Destroy(hit.transform.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(WebcamController.umbrellaCenter, 0.1f);
    }
    
}
