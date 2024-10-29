using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;
    public int quantityPerHit = 2;
    public int capacity = 10;

    public void Gather(Vector3 hitPoint, Vector3 hitNormal, ResourceToolType toolType)
    {
        if (IsToolCompatibleWithLayer(toolType, gameObject.layer))
        {
            for (int i = 0; i < quantityPerHit; i++)
            {
                if (capacity <= 0) break;

                capacity -= 1;

                Vector3 dropPosition = hitPoint + hitNormal * 0.5f + Vector3.up * 0.2f;
                Instantiate(itemToGive.dropPrefab, dropPosition, Quaternion.LookRotation(hitNormal, Vector3.up));
            }

            if (capacity <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("This tool cannot gather this resource.");
        }
    }

    private bool IsToolCompatibleWithLayer(ResourceToolType toolType, int resourceLayer)
    {
        int treeLayer = LayerMask.NameToLayer("Tree");
        int rockLayer = LayerMask.NameToLayer("Rock");

        if (toolType == ResourceToolType.Tree && resourceLayer == treeLayer) return true;
        if (toolType == ResourceToolType.Rock && resourceLayer == rockLayer) return true;
        if (toolType == ResourceToolType.Both && (resourceLayer == treeLayer || resourceLayer == rockLayer)) return true;

        return false;
    }
}