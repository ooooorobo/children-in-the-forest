using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public sealed class ItemIO : MonoBehaviour {

    public GameObject ObjMan;

    public static void SaveData()
    {
        List<GameObject> slots = ObjManager.objManager.inventory.slots;

        XmlDocument XmlDoc = new XmlDocument();
        XmlElement child = XmlDoc.CreateElement("Child");
        XmlDoc.AppendChild(child);

        Slot slot;
        int i = 0;

        while (i < slots.Count)
        {
            if ((slot = slots[i].GetComponent<Slot>()).isSlots())
            {
                // i번 인덱스에 아이템 있을 동안
                Item item = slot.ItemReturn();
                XmlElement setting = XmlDoc.CreateElement("Item");

                setting.SetAttribute("name", item.name);
                setting.SetAttribute("ID", item.ID);
                setting.SetAttribute("des", item.des);
                setting.SetAttribute("tool", item.tool.ToString());
                setting.SetAttribute("effect", item.effect);
                setting.SetAttribute("slotNum", i.ToString());

                switch(item.type)
                {
                    case Item.ItemType.food:
                        setting.SetAttribute("type", "food");
                        break;
                    case Item.ItemType.material:
                        setting.SetAttribute("type", "material");
                        break;
                    case Item.ItemType.memo:
                        setting.SetAttribute("type", "memo");
                        break;
                    case Item.ItemType.tool:
                        setting.SetAttribute("type", "tool");
                        break;
                }

                child.AppendChild(setting);
            }

            i++;
        }

        XmlDoc.Save(Application.dataPath + "/Save/InventoryData.xml");
    }

    public static Item[] LoadData()
    {
        Item[] items = new Item[18];

        if (!System.IO.File.Exists(Application.dataPath + "/Save/InventoryData.xml"))
        {
            return null;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(Application.dataPath + "/Save/InventoryData.xml");
        XmlElement child = xmlDoc["Child"];

        foreach (XmlElement itemElement in child.ChildNodes)
        {
            Item item = new Item();

            item.name = itemElement.GetAttribute("name");
            item.ID = itemElement.GetAttribute("ID");
            item.des = itemElement.GetAttribute("des");
            item.effect = itemElement.GetAttribute("effect");
            item.tool = itemElement.GetAttribute("tool");
            item.sprite = DataManager.dataManager.findSprite(item.ID);
            //item.type = itemElement.GetAttribute("type"); switch - case 문 돌려서 적용시키기

            items[System.Convert.ToInt32(itemElement.GetAttribute("slotNum"))] = item;
        }

        return items;
    }
}
