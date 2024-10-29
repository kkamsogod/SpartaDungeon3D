using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public Button button;
    public Image icon;
    public TextMeshProUGUI quatityText;
    public TextMeshProUGUI equipText;
    private Outline outline;

    public UIInventory inventory;

    public int index;
    public bool equipped;
    public int quantity;
    public int inventoryIndex;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;
        equipText.gameObject.SetActive(equipped);

        UpdateOutline();
    }

    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
        equipText.gameObject.SetActive(false);

        UpdateOutline();
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }

    public void UpdateOutline()
    {
        if (outline != null)
        {
            outline.enabled = (item != null && inventory.selectedItemIndex == index);
        }
    }
}