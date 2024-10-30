using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [System.Serializable]
    public class BoxItem
    {
        public ItemData itemData;
        public int itemCount;
    }

    public BoxItem[] itemsToGive;
    public UIInventory uiInventory;

    public bool destroyAfterUse = true;

    private bool isPlayerNearby = false;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            GiveItemsToPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player is near the box.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player left the box area.");
        }
    }

    private void GiveItemsToPlayer()
    {
        if (uiInventory != null)
        {
            foreach (var boxItem in itemsToGive)
            {
                uiInventory.AddItem(boxItem.itemData, boxItem.itemCount);
            }

            if (destroyAfterUse)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("UIInventory reference is not assigned.");
        }
    }
}
