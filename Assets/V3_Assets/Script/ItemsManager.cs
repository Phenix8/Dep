using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviour {

    public static List<ItemToFind> itemsList = new List<ItemToFind>();
    private List<ItemToFind> itemsToDelete = new List<ItemToFind>();

    public List<GameObject> UI_itemsList;
    public List<GameObject> itemsInScene;


    private GameObject deletedItem;
    private GameObject currentUIimage;


    void Start ()
    {
        LoadItems();
        UpdateItemSlots(true);
	}
	
	void Update ()
    {
        CheckItemDeletion();
        // print("ItemList count = " + itemsList.Count());
	}


    // Affiche les image correspondant à chaque item dans les slots UI
    private void UpdateItemSlots(bool isInit)
    {
        IEnumerable<ItemToFind> subItemList = itemsList.Where(x => x.Id < 5);
        // Remet la liste à zéro si la liste doit être rafraichie (= ce n'est pas l'initialisation)
        if (!isInit)
        {
            foreach (GameObject UI_item in UI_itemsList)
            {
                UI_item.GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Empty);
                UI_item.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
            }
        }

        foreach (ItemToFind item in subItemList)
        {
            currentUIimage = UI_itemsList.FirstOrDefault(x => x.name.Contains(item.Id.ToString()));
            if (currentUIimage == null || currentUIimage.GetComponent<Image>() == null)
                continue;

            LoadSprite(currentUIimage, item.ImagePath);
            currentUIimage.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
        }
    }


    // Charge dans le composant 'image' du GameObject passé en 
    // paramètres l'image de chemin 'spirtePath'
    private void LoadSprite(GameObject imageObject, string spritePath)
    {
        Image image = imageObject.GetComponent<Image>();

        imageObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePath);

        if (image.sprite == null)
            image.sprite = Resources.Load<Sprite>("Indices\\" + spritePath);

        if (image.sprite == null)
            image.sprite = Resources.Load<Sprite>("Silhouette\\" + spritePath);

        if (image.sprite == null)
            Debug.LogError("Sprite not loaded");
    }

    // Vérifie s'il faut MàJ l'UI d'un item trouvé
    // en supprimant l'objet trouvé
    private void CheckItemDeletion()
    {
        itemsToDelete = new List<ItemToFind>();

        // Supprime les objets dans la liste si on a cliqué dessus une première fois
        foreach (ItemToFind item in itemsList)
        {
            deletedItem = itemsInScene.FirstOrDefault(x => x.GetComponent<InteractableItem>().id.Equals(item.Id));
            if (deletedItem == null || deletedItem.GetComponent<InteractableItem>().isDeleted)
                itemsToDelete.Add(item);          
        }

        // MàJ des IDs et de l'UI
        if (itemsToDelete.Count() != 0)
        {
            print("Item deleted : " + itemsToDelete.FirstOrDefault().Name);

            for(int i=0; i< itemsToDelete.Count(); i++)            
                itemsList.Remove(itemsToDelete.ElementAt(i));     
                     
            UpdateItemSlots(false);
        }   
    }


    // Charge à partir de la liste de GameObject les items
    private void LoadItems()
    {
        print("There are " + itemsInScene.Count + " items in scene to load.");

        Image itemImage;
        InteractableItem itemScript;

        foreach (GameObject item in itemsInScene)
        {
            if (item == null)
            {
                Debug.LogError("impossible to get one of item in scene.");
                continue;
            }

            itemImage = item.GetComponent<Image>();
            itemScript = item.GetComponent<InteractableItem>();

            if (itemImage == null || string.IsNullOrEmpty(itemImage.sprite.name))
            {
                Debug.LogError("Impossible to get image from object : " + item.name);
                continue;
            }
            else if (itemScript == null)
            {
                Debug.LogError("Impossible to get InteractibleItem script from item : " + item.name);
                continue;
            }

            itemsList.Add(new ItemToFind(itemScript.id, item.name, itemImage.sprite.name));           
            itemImage.color = new Color(255.0f, 255.0f, 255.0f, 0.0f);

            print(item.name +" (id:" + itemScript.id + ") added");
        }
    }

}
