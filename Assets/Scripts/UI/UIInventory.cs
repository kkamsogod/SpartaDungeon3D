using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] inventorySlots;
    public ItemSlot[] quickSlots;

    public GameObject inventoryCanvas;
    public GameObject quickSlotCanvas;
    public Transform inventorySlotPanel;
    public Transform quickSlotPanel;
    public Transform dropPosition;

    [Header("Select Item")]
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    private PlayerController controller;
    private PlayerCondition condition;
    private ItemEffectHandler effectHandler;

    ItemData selectedItem;
    public int selectedItemIndex = 0;

    int curEquipIndex;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        effectHandler = CharacterManager.Instance.Player.effectHandler;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        if (inventorySlotPanel != null)
        {
            inventorySlots = new ItemSlot[inventorySlotPanel.childCount];
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i] = inventorySlotPanel.GetChild(i).GetComponent<ItemSlot>();
                inventorySlots[i].index = i;
                inventorySlots[i].inventory = this;
            }
        }

        if (quickSlotPanel != null)
        {
            quickSlots = new ItemSlot[quickSlotPanel.childCount];
            for (int i = 0; i < quickSlots.Length; i++)
            {
                quickSlots[i] = quickSlotPanel.GetChild(i).GetComponent<ItemSlot>();
                quickSlots[i].index = i;
                quickSlots[i].inventory = this;
            }
        }

        inventoryCanvas.SetActive(false);
        quickSlotCanvas.SetActive(true);

        ClearSelectedItemWindow();
        UpdateInventorySlots();
        UpdateQuickSlots();
    }

    void Update()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseQuickSlotItem(i);
            }
        }
    }

    void ClearSelectedItemWindow()
    {
        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryCanvas.SetActive(false);
            quickSlotCanvas.SetActive(true);
        }
        else
        {
            inventoryCanvas.SetActive(true);
            quickSlotCanvas.SetActive(false);
        }
    }

    public bool IsOpen()
    {
        return inventoryCanvas.activeInHierarchy;
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        if (data == null)
        {
            Debug.LogError("ItemData is null.");
            return;
        }

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateInventorySlots();
                UpdateQuickSlots();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateInventorySlots();
            UpdateQuickSlots();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    void UpdateInventorySlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item != null)
            {
                inventorySlots[i].Set();
            }
            else
            {
                inventorySlots[i].Clear();
            }
        }
    }

    void UpdateQuickSlots()
    {
        int quickSlotIndex = 0;

        for (int i = 0; i < quickSlots.Length; i++)
        {
            quickSlots[i].Clear();
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item != null && inventorySlots[i].item.type != ItemType.Resource)
            {
                if (quickSlotIndex >= quickSlots.Length)
                {
                    Debug.LogWarning("No available quick slots to assign.");
                    break;
                }
                quickSlots[quickSlotIndex].item = inventorySlots[i].item;
                quickSlots[quickSlotIndex].quantity = inventorySlots[i].quantity;
                quickSlots[quickSlotIndex].equipped = inventorySlots[i].equipped;
                quickSlots[quickSlotIndex].inventoryIndex = i;
                quickSlots[quickSlotIndex].Set();
                quickSlotIndex++;
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item == data && inventorySlots[i].quantity < data.maxStackAmount)
            {
                return inventorySlots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item == null)
            {
                return inventorySlots[i];
            }
        }
        return null;
    }

    void UseQuickSlotItem(int index)
    {
        if (quickSlots == null || index >= quickSlots.Length || quickSlots[index].item == null)
        {
            Debug.LogWarning("Invalid quick slot index or item is null.");
            return;
        }

        selectedItem = quickSlots[index].item;
        selectedItemIndex = quickSlots[index].inventoryIndex;

        if (selectedItem.type == ItemType.Consumable)
        {
            OnUseButton();
        }
        else if (selectedItem.type == ItemType.Equipable)
        {
            if (quickSlots[index].equipped)
            {
                OnUnEquipButton();
            }
            else
            {
                OnEquipButton();
            }
        }
    }

    void ThrowItem(ItemData data)
    {
        if (dropPosition == null)
        {
            Debug.LogError("Drop position is not assigned.");
            return;
        }
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (inventorySlots == null || index >= inventorySlots.Length || inventorySlots[index].item == null)
        {
            Debug.LogWarning("Invalid slot index or item is null.");
            return;
        }

        selectedItem = inventorySlots[index].item;
        selectedItemIndex = index;

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !inventorySlots[index].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && inventorySlots[index].equipped);
        dropButton.SetActive(true);

        foreach (var slot in inventorySlots)
        {
            slot.UpdateOutline();
        }
    }

    public void OnUseButton()
    {
        if (selectedItem == null || selectedItem.type != ItemType.Consumable) return;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            switch (selectedItem.consumables[i].type)
            {
                case ConsumableType.Health:
                    condition.Heal(selectedItem.consumables[i].value);
                    break;
                case ConsumableType.Hunger:
                    condition.Eat(selectedItem.consumables[i].value);
                    break;
                case ConsumableType.Stamina:
                    condition.StaminaUp(selectedItem.consumables[i].value);
                    break;
                case ConsumableType.SpeedBoost:
                    CharacterManager.Instance.Player.effectHandler.StartSpeedBoost(selectedItem.consumables[i].value, selectedItem.consumables[i].duration);
                    break;
                case ConsumableType.DoubleJump:
                    CharacterManager.Instance.Player.effectHandler.StartDoubleJump(selectedItem.consumables[i].duration);
                    break;
                case ConsumableType.InfiniteStamina:
                    CharacterManager.Instance.Player.effectHandler.StartInfiniteStamina(selectedItem.consumables[i].duration);
                    break;
            }
        }

        RemoveSelectedItem();
    }

    public void OnDropButton()
    {
        if (selectedItem == null) return;
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        if (selectedItem == null || selectedItemIndex < 0) return;

        ItemSlot slotToRemove = inventorySlots[selectedItemIndex];

        slotToRemove.quantity--;

        if (slotToRemove.quantity <= 0)
        {
            selectedItem = null;
            slotToRemove.item = null;
            slotToRemove.equipped = false;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        UpdateInventorySlots();
        UpdateQuickSlots();
    }

    public void OnEquipButton()
    {
        if (selectedItem == null || selectedItem.type != ItemType.Equipable) return;

        if (inventorySlots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        inventorySlots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateInventorySlots();
        UpdateQuickSlots();

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        if (index < 0 || index >= inventorySlots.Length) return;

        inventorySlots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateInventorySlots();
        UpdateQuickSlots();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
}