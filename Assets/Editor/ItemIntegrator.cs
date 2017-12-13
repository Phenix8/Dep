using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemIntegrator : EditorWindow
{

    private bool groupEnabled;
    private UnityEngine.Object itemSpriteToLoad;
    private UnityEngine.Object rewardSoundToLoad;
    private UnityEngine.Object hintSoundToLoad;
    private string itemName;


    // GameObject et script de la scène
    GameObject itemsInScene;
    GameObject itemPrefab;
    GameObject cursorObject;
    ItemsManager itemManager;


    [MenuItem("Window/Item integrator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ItemIntegrator));
    }

    void OnGUI()
    {
        LoadGameObjects();
        LoadGUIlayouts();

        // Clic sur le bouton d'ajout
        if (GUILayout.Button("Load item"))
        {
            if (itemSpriteToLoad == null && hintSoundToLoad == null)           
                ShowNotification(new GUIContent("Nor sprite nor sound entered."));                

            else if (itemSpriteToLoad != null && hintSoundToLoad != null)
                ShowNotification(new GUIContent("Select a sprite OR a sound to add."));

            else if (!LoadGameObjects())
                ShowNotification(new GUIContent("Impossible to get objects from scene."));

            else if (rewardSoundToLoad == null)
                ShowNotification(new GUIContent("No reward sound entered."));

            else {

                // CHARGEMENT DES ITEMS
                string name = (string.IsNullOrEmpty(itemName)) ? (itemSpriteToLoad == null ? hintSoundToLoad.name : itemSpriteToLoad.name) : itemName;
                int id = itemManager.itemsInScene.Count - 1;

                GameObject itemObject = CreateGameObject(id, name);

                if (itemObject == null)
                {
                    ShowNotification(new GUIContent("Impossible to create GameObject."));
                    return;
                }

                AddItemToScene(itemObject);

                ShowNotification(new GUIContent("Item loaded succesfully."));
            }
        }
    }

    // Récupère les objets de la scène. Indique en retour si les objets
    // ont bien été trouvés ou non
    private bool LoadGameObjects()
    {
        itemsInScene = GameObject.Find("Items_canvas");
        GameObject itemManagerObject = GameObject.Find("CicularMenu");

        if (itemManagerObject == null)
            return false;

        itemManager = itemManagerObject.GetComponent<ItemsManager>();
        itemPrefab = Resources.Load<GameObject>("Prefab/item_prefab");
        cursorObject = GameObject.Find("cursor");

        return (itemManager != null && itemsInScene != null && itemPrefab != null);
    }


    // Charge les éléments graphiques
    private void LoadGUIlayouts()
    {
        GUILayout.Label("Item informations", EditorStyles.boldLabel);

        // Groupe de l'item à ajouter
        EditorGUILayout.BeginVertical();
        
            itemSpriteToLoad = EditorGUILayout.ObjectField("Item sprite : ", itemSpriteToLoad, typeof(Sprite), false);
            EditorGUILayout.Space();     
            itemName = EditorGUILayout.TextField("Item name (optional) : ", itemName);
            EditorGUILayout.Space();
            rewardSoundToLoad = EditorGUILayout.ObjectField("Reward sound : ", rewardSoundToLoad, typeof(AudioClip), false);
            EditorGUILayout.Space();
            hintSoundToLoad = EditorGUILayout.ObjectField("Sound hint (optional) : ", hintSoundToLoad, typeof(AudioClip), false);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
    }


    // Crée l'objet dans la scène et l'ajoute à la liste
    private GameObject CreateGameObject(int id, string name)
    {
        try { 
            GameObject itemFromPrefab = Instantiate(itemPrefab) as GameObject;
            InteractableItem scriptComponent = itemFromPrefab.GetComponent<InteractableItem>();
            Image imageComponent = itemFromPrefab.GetComponent<Image>();

            itemFromPrefab.name = name + "_item";
            scriptComponent.id = id;
            scriptComponent.cursorObjectTr = (RectTransform)cursorObject.transform;
            scriptComponent.audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
            scriptComponent.rewardClip = (AudioClip)rewardSoundToLoad;

            // Cas d'un indice image
            if (itemSpriteToLoad != null)
            {
                imageComponent.sprite = Resources.Load<Sprite>(itemSpriteToLoad.name);
            
                if (imageComponent.sprite == null)
                    imageComponent.sprite = Resources.Load<Sprite>("Indices\\"+itemSpriteToLoad.name);

                if (imageComponent.sprite == null)
                    imageComponent.sprite = Resources.Load<Sprite>("Silhouette\\" + itemSpriteToLoad.name);
            }
            // cas d'un indice sonore
            else
            {
                imageComponent.sprite = Resources.Load<Sprite>("sound_pictogram");
                scriptComponent.soundHint = (AudioClip)hintSoundToLoad;
            }

            itemFromPrefab.transform.SetParent(itemsInScene.transform);
            itemFromPrefab.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 0.0f, 0.0f);

            return itemFromPrefab;
        }
        catch(Exception e)
        {
            Debug.Log("Error : " + e.InnerException + "\n" + e.StackTrace);
            return null;
        }
    }

    // Enlève les éléments null de la liste et ajoute celui
    // envoyé en paramètre
    private void AddItemToScene(GameObject itemObject)
    {
        List<GameObject> itemsToRemove = new List<GameObject>();

        foreach(GameObject itemInScene in itemManager.itemsInScene)        
            if (itemInScene == null)
                itemsToRemove.Add(itemInScene);

        itemsToRemove.ForEach(x => itemManager.itemsInScene.Remove(x));

        itemManager.itemsInScene.Add(itemObject);
    }

}