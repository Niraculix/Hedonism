using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemAttribute
{
    public enum ItemType {MaxHp_Int,LHealRate_Float,DDrainRate_Float,DDashCount_Int,LDashCount_Int,LAttackDMG_Int,DAttackCooldown_Float,DDashReset_Bool,LParryLechPercent_Float}
    public ItemType type;

    public bool boolValue;
    public float floatValue;
    public int intValue;
}

[Serializable]
public class Item
{
    public string item_name;
    public Texture2D sprite;
    public List<ItemAttribute> attributes = new List<ItemAttribute>();
}

