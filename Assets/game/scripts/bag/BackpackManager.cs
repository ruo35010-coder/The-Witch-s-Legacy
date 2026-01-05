// BackpackManager.cs
using System.Collections.Generic;
using UnityEngine;

public class BackpackManager : MonoBehaviour
{
    public static BackpackManager Instance;

    public List<BackpackSlot> slots = new List<BackpackSlot>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ 只接收 ItemData
    public bool AddItem(ItemData item)
    {
        if (item == null || item.icon == null)
        {
            Debug.LogError("❌ 道具或图标为空！");
            return false;
        }

        foreach (var slot in slots)
        {
            if (slot.itemData == null)
            {
                slot.SetItem(item);
                return true;
            }
        }

        Debug.Log("🎒 背包已满！");
        return false;
    }
}