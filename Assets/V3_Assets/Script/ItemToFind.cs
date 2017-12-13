using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToFind {

    // Identifiant unique
    public int Id
    {
        get;
        set;
    }

    // Nom de l'objet
    public string Name
    {
        get;
        set;
    }

    // Nom de la ressource sprite
    public string ImagePath
    {
        get;
        set;
    }

    // Constructeur avec paramètres
    public ItemToFind(int id, string name, string path)
    {
        this.Id = id;
        this.Name = name;
        this.ImagePath = path;
    }

}
