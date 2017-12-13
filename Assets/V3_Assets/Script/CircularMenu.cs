using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularMenu : MonoBehaviour
{

    public static bool isMenuOpen = false;
    
    private Vector2 FromVector = new Vector2(0.5f, 1.0f);
    private Vector2 CenterCircle = new Vector2(0.5f, 0.5f);
    private Vector2 ToVector;

    public int MenuItems;
    public int CurrentMenuItem;
    private int OldMenuItem;

    private bool areButtonsInitialized = false;

    public Transform CursorObjectTr;

    public AudioSource audioSource;

    public List<MenuButton> buttons = new List<MenuButton>();

    public List<GameObject> objectsInScene;


	void Start ()
    {
        MenuItems = buttons.Count;
        CurrentMenuItem = OldMenuItem = 0;       
    }
	

	void Update ()
    {
        if (!areButtonsInitialized)
        {
            InitButtons();
            areButtonsInitialized = true;
        }

        isMenuOpen = ! (transform.localPosition.y > 0.0f);
        
        GetCurrentMenuItem();

        if (isMenuOpen && ((UmbrellaDetector_v3.isDebug && Input.GetMouseButtonDown(0)) || UmbrellaDetector_v3.enableAction))
        {
            ButtonAction();
            UmbrellaDetector_v3.enableAction = false;
        }

        // Menu caché ou affiché
        if (Input.GetKeyDown("m"))
            transform.localPosition = new Vector3(0.0f, (isMenuOpen) ? 500.0f : 0.0f, 0.0f);        
	}


    // Initialisation des boutons (couleur, type d'objet et image/son)
    private void InitButtons()
    {
        objectsInScene = GetComponent<ItemsManager>().itemsInScene;

        for (int i=0; i<buttons.Count; i++)
        {
            MenuButton currentButton = buttons[i];

            if (currentButton.name.Contains("empty"))
                continue;

            currentButton.sceneImage.color = currentButton.normalColor;
            currentButton.sceneImage.sprite = objectsInScene[i].GetComponent<Image>().sprite;           

            if (objectsInScene[i].GetComponent<InteractableItem>().soundHint == null)
                currentButton.type = HintType.picture;          
            else
            {
                currentButton.soundHint = objectsInScene[i].GetComponent<InteractableItem>().soundHint;
                currentButton.type = HintType.sound;
            }

            print(objectsInScene[i].name + " loaded");
        }
    }


    // Récupère le boutton sur lequel la souris est
    private void GetCurrentMenuItem()
    {
        // CursorPosition = new Vector2(CursorObjectTr.position.x, CursorObjectTr.position.y);
        // ToVector = new Vector2(CursorPosition.x/Screen.width, CursorPosition.y/Screen.height);

        if (UmbrellaDetector_v3.isDebug)
            ToVector = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y  / Screen.height);
        else
            ToVector = new Vector2(CursorObjectTr.position.x / Screen.width, CursorObjectTr.position.y / Screen.height);

        float angle = ((Mathf.Atan2(FromVector.y - CenterCircle.y, FromVector.x - CenterCircle.x) 
                     - Mathf.Atan2(ToVector.y - CenterCircle.y, ToVector.x - CenterCircle.x)) + Mathf.PI/6)
                     * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360;

        CurrentMenuItem = (int)(angle / (360 / MenuItems));

        if (CurrentMenuItem != OldMenuItem)
        {
            if (buttons[OldMenuItem].sceneImage.sprite != null)
                buttons[OldMenuItem].sceneImage.color = buttons[OldMenuItem].normalColor;

            OldMenuItem = CurrentMenuItem;

            if (buttons[CurrentMenuItem].sceneImage.sprite != null)
                buttons[CurrentMenuItem].sceneImage.color = buttons[CurrentMenuItem].highlightColor;
        }
    }


    // Actions effectuée à l'appui sur un des items du menu
    private void ButtonAction()
    {
        if (buttons[CurrentMenuItem].sceneImage.sprite == null)
            return;

        buttons[CurrentMenuItem].sceneImage.color = buttons[CurrentMenuItem].pressedColor;
        CheckSoundActivation(CurrentMenuItem);
    }


    // Joue le son s'il s'agit d'un indice sonore
    private void CheckSoundActivation(int itemMenu)
    {
        if (buttons[itemMenu].type.Equals(HintType.sound) && buttons[itemMenu].soundHint != null && audioSource != null)
        {
            audioSource.clip = buttons[itemMenu].soundHint;
            audioSource.Play();
        }
    }

}


[System.Serializable]
public class MenuButton
{
    public string name;
    public Image sceneImage;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.grey;
    public Color pressedColor = Color.gray;
    public HintType type;
    public AudioClip soundHint;
}


// Énumération du type d'indice (image du tableau, ombre de l'objet ou son descriptif)
public enum HintType
{
    none = 0,
    picture = 1,
    shadow = 2,
    sound = 3,
}
