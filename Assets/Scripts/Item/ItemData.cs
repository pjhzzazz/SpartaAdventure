using System;
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
    Stamina,
    Speed
}

public enum StatType
{
    Damage,
    Speed
}
[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
    public float duration;
}
[Serializable]
public class ItemDataEquip
{
    public StatType type;
    public float value;
}
[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType itemType;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")] public bool canStack;
    public int maxStackAmount;
    
   [Header("Consumable")] public ItemDataConsumable[] consumables;

   [Header("Equip")] public GameObject equipPrefab;
   public EquipSlot equipSlot;
   public ItemDataEquip[] equips;
}
