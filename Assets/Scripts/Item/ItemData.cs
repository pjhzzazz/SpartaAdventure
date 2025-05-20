using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Equipable,
    Consumable
}

public enum ConsumableType
{
    Health,
    Stamina
}
[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType itemType;
    public Sprite icon;
    public GameObject itemPrefab;

    [Header("Stacking")] public bool canStack;
    public int maxStackAmount;
    
   
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
