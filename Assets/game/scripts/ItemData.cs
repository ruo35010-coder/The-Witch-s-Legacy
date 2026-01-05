// ItemData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    public new string name;   
    public string description; // 显示名称，如“翠绿树叶”
    public Sprite icon;            // UI 图标
    public GameObject prefab;      // 可选：3D模型（用于展示）
    public string itemId;          // 唯一ID，如 "leaf_green"
}