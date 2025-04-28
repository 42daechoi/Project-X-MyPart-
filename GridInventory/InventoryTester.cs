using System;
using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public static InventoryTester Instance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject itemObject = GameObject.Find("Item");

            if (itemObject != null)
            {
                itemObject.GetComponent<FarmableItem>().Pickup();
            }
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            GameObject itemObject = GameObject.Find("Item2");

            if (itemObject != null)
            {
                itemObject.GetComponent<FarmableItem>().Pickup();
            }
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            GameObject itemObject = GameObject.Find("Item3");

            if (itemObject != null)
            {
                itemObject.GetComponent<FarmableItem>().Pickup();
            }
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            GameObject itemObject = GameObject.Find("Item4");

            if (itemObject != null)
            {
                itemObject.GetComponent<FarmableItem>().Pickup();
            }
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            Inventory.Instance.ExpandInventory();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            EventBus.OnInteractMerchant?.Invoke("Gear", true);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            EventBus.OnInteractMerchant?.Invoke("Consumable", false);
        }
    }
}
