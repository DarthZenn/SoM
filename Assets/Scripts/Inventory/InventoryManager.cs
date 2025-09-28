using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //public GameManager gameManager;
    public GameObject inventoryUI;
    public InventorySlot[] slots; // Assign 9 buttons in Inspector
    public InventorySlot[] equipmentSlots;
    public TextMeshProUGUI itemDesc;
    public TextMeshProUGUI itemName;
    public GameObject useButton;
    public GameObject dropButton;
    public GameObject unequipButton;
    public GameObject dropPoint;
    public GameObject OneHandedMeleeHolder;
    public GameObject pistolHolder;

    private InventorySlot selectedSlot = null;
    private Color defaultSlotColor = Color.gray;
    public GameObject currentRHEquippedItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        inventoryUI.SetActive(false);

        foreach (var slot in slots)
        {
            Image slotColor = slot.GetComponent<Image>();
            slotColor.color = defaultSlotColor;
        }

        ClearUI();
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory") && !GameManager.Instance.interactPromptOpen
            && !GameManager.Instance.pauseMenuOpen && !GameManager.Instance.gameOverMenuOpen && !GameManager.Instance.mainMenuOpen)
        {
            GameManager.Instance.inventoryMenuOpen = !GameManager.Instance.inventoryMenuOpen; // toggle on/off

            if (GameManager.Instance.inventoryMenuOpen)
            {
                inventoryUI.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                DeselectSlot();
                HideOptions();
                inventoryUI.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    public int AddItemToInventory(ItemSO item, int quantity, int currentAmmo)
    {
        // Try to add to an existing stack
        foreach (var slot in slots)
        {
            if (slot.itemData == item && slot.quantity < item.maxStack)
            {
                int spaceLeft = item.maxStack - slot.quantity;
                int toAdd = Mathf.Min(spaceLeft, quantity);

                slot.quantity += toAdd;
                slot.quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";

                quantity -= toAdd;
                if (quantity <= 0) return 0; // no leftovers
            }
        }

        // Otherwise put into a new slot
        foreach (var slot in slots)
        {
            if (slot.itemData == null)
            {
                int toAdd = Mathf.Min(item.maxStack, quantity);
                slot.SetSlot(item, toAdd, currentAmmo);
                //Debug.Log("ammo: " + slot.currentAmmo);

                quantity -= toAdd;
                if (quantity <= 0) return 0;
            }
        }

        /*if (quantity > 0)
            Debug.Log("Inventory full! Could not add all items.");*/

        return quantity; // return leftovers
    }

    public void SelectSlot(InventorySlot slot)
    {
        // Deselect if clicked again
        if (selectedSlot == slot)
        {
            DeselectSlot();
            HideOptions();
            return;
        }

        // Reset previous slot color
        if (selectedSlot != null)
        {
            Image prevSlotColor = selectedSlot.GetComponent<Image>();
            prevSlotColor.color = defaultSlotColor;
        }

        // Select new slot
        selectedSlot = slot;

        if (slot.itemData != null)
        {
            // Highlight
            Image slotColor = slot.GetComponent<Image>();
            slotColor.color = Color.green;

            // Update UI
            itemName.text = slot.itemData.itemName;
            itemDesc.text = slot.itemData.description;

            // Check if this slot is equipment
            int equipIndex = System.Array.IndexOf(equipmentSlots, slot);
            if (equipIndex >= 0)
            {
                ShowEquipmentOptions();
                useButton.SetActive(false);

                // Reset listeners
                Button unequipBtn = unequipButton.GetComponent<Button>();
                unequipBtn.onClick.RemoveAllListeners();

                // Pass equipIndex as parameter
                int capturedIndex = equipIndex;
                unequipBtn.onClick.AddListener(() => UnequipItem(capturedIndex));
            }
            else
            {
                ShowOptions();
                unequipButton.SetActive(false);
            }
        }
        else
        {
            ClearItemInfo();
            HideOptions();
        }
    }

    public void DeselectSlot()
    {
        if (selectedSlot != null)
        {
            Image buttonImage = selectedSlot.GetComponent<Image>();
            buttonImage.color = defaultSlotColor;
        }

        selectedSlot = null;
        ClearItemInfo();
    }

    public void DropSelectedItem()
    {
        if (selectedSlot == null || selectedSlot.itemData == null) return;

        Vector3 dropPosition = dropPoint.transform.position;

        // Instantiate the item prefab at player's feet
        GameObject droppedItem = Instantiate(selectedSlot.itemData.worldPrefab, dropPosition, Quaternion.identity);

        // Apply ItemData info to the prefab
        ItemData itemDataComp = droppedItem.GetComponentInChildren<ItemData>();
        if (itemDataComp != null)
        {
            itemDataComp.item = selectedSlot.itemData;
            itemDataComp.quantityPerPickup = selectedSlot.quantity;
            itemDataComp.currentAmmo = selectedSlot.currentAmmo;
        }

        // If dropping from equipment, also destroy hand visual
        int equipIndex = System.Array.IndexOf(equipmentSlots, selectedSlot);
        if (equipIndex >= 0 && currentRHEquippedItem != null)
        {
            Destroy(currentRHEquippedItem);
            currentRHEquippedItem = null;
        }

        // Clear out the slot
        selectedSlot.ClearSlot();
        DeselectSlot();
        HideOptions();
    }

    public void UseSelectedItem()
    {
        if (selectedSlot == null || selectedSlot.itemData == null) return;

        //PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        //if (playerStats == null) return;

        if (selectedSlot.itemData.itemType == ItemType.Healing)
        {
            // Healing item logic
            HealingItemSO healingItem = selectedSlot.itemData as HealingItemSO;
            HandleHealingItem(healingItem/*, playerStats*/);
            return;
        }
        
        if (selectedSlot.itemData.itemType == ItemType.Weapon)
        {
            WeaponSO weaponItem = selectedSlot.itemData as WeaponSO;

            if (weaponItem.isTwoHanded == false)
            {
                if (weaponItem.weaponCategory == WeaponSO.WeaponCategory.Melee)
                {
                    EquipRightHand(selectedSlot, OneHandedMeleeHolder);
                }
                else if (weaponItem.weaponCategory == WeaponSO.WeaponCategory.Pistol)
                {
                    EquipRightHand(selectedSlot, pistolHolder);
                }
            }

            return;
        }
    }

    private void HandleHealingItem(HealingItemSO healingItem/*, PlayerStats playerStats*/)
    {
        if (healingItem.healType == HealingItemSO.HealType.Health)
        {
            float curHP = PlayerStats.Instance.currentHealth;
            float maxHP = PlayerStats.Instance.maxHealth;

            if (curHP < maxHP)
            {
                PlayerStats.Instance.HealHealth(healingItem.healPercentage);
                UsedItemUpdate();
            }
            else Debug.Log("HP full!");
        }
        else if (healingItem.healType == HealingItemSO.HealType.Sanity)
        {
            float curSan = PlayerStats.Instance.currentSanity;
            float maxSan = PlayerStats.Instance.maxSanity;

            if (curSan < maxSan)
            {
                PlayerStats.Instance.HealSanity(healingItem.healPercentage);
                UsedItemUpdate();
            }
            else Debug.Log("Sanity full!");
        }
    }

    private void EquipRightHand(InventorySlot fromSlot, GameObject weaponHolder)
    {
        InventorySlot rightHandSlot = equipmentSlots[0]; // index 0 = right hand
        // If there’s already a weapon equipped, unequip it first
        if (rightHandSlot.itemData != null)
        {
            itemDesc.text = "Already have a weapon equipped. Unequip that weapon first.";
            //Debug.Log("Already have a weapon equipped. Unequip first.");
            return;
        }

        // Transfer item to right hand slot
        rightHandSlot.SetSlot(fromSlot.itemData, fromSlot.quantity, fromSlot.currentAmmo);
        //Debug.Log("ammo: " + rightHandSlot.currentAmmo);

        // Spawn prefab in player's hand
        if (currentRHEquippedItem != null)
            Destroy(currentRHEquippedItem);

        currentRHEquippedItem = Instantiate(rightHandSlot.itemData.inventoryPrefab, weaponHolder.transform);
        //currentRHEquippedItem.transform.localScale = Vector3.one;  // new Vector3(1f/18.10255f, 1f/18.10255f, 1f/18.10255f); // Remember to fix this after replacing player character.
        currentRHEquippedItem.transform.localPosition = Vector3.zero;
        currentRHEquippedItem.transform.localRotation = Quaternion.identity;

        // Clear the inventory slot
        fromSlot.ClearSlot();
        DeselectSlot();
        HideOptions();

       //Debug.Log("Equipped " + rightHandSlot.itemData.itemName + " in right hand.");
    }

    public void UnequipItem(int equipIndex)
    {
        if (equipIndex < 0 || equipIndex >= equipmentSlots.Length) return;

        InventorySlot equipSlot = equipmentSlots[equipIndex];
        if (equipSlot.itemData == null) return;

        // Destroy equipped prefab if it exists
        if (currentRHEquippedItem != null)
        {
            Destroy(currentRHEquippedItem);
            currentRHEquippedItem = null;
        }

        // Find empty slot in inventory
        foreach (var slot in slots)
        {
            if (slot.itemData == null)
            {
                slot.SetSlot(equipSlot.itemData, equipSlot.quantity, equipSlot.currentAmmo);
                //Debug.Log($"Unequipped {equipSlot.itemData.itemName} from equipment slot {equipIndex}.");
                equipSlot.ClearSlot();
                DeselectSlot();
                HideOptions();
                return;
            }
        }

        itemDesc.text = "No empty slot in inventory to unequip item!";
        //Debug.Log("No empty slot in inventory to unequip item!");
    }

    public void UsedItemUpdate()
    {
        // Reduce quantity
        selectedSlot.quantity--;
        if (selectedSlot.quantity > 0)
        {
            selectedSlot.quantityText.text = selectedSlot.quantity > 1 ? selectedSlot.quantity.ToString() : "";
        }
        else
        {
            selectedSlot.ClearSlot();
            DeselectSlot();
            HideOptions();
        }
    }

    public void GunShotUpdate()
    {
        InventorySlot rightHandSlot = equipmentSlots[0]; // pistol is in right hand
        if (rightHandSlot.itemData == null) return;

        if (rightHandSlot.currentAmmo > 0)
        {
            rightHandSlot.currentAmmo--;
            Debug.Log("Ammo left: " + rightHandSlot.currentAmmo);
        }
        else
        {
            Debug.Log("No ammo left!");
        }
    }

    public int SearchAmmo(AmmoSO.AmmoCategory neededAmmo, int amountNeeded)
    {
        int ammoSupplied = 0;

        foreach (var slot in slots)
        {
            if (slot.itemData != null && slot.itemData is AmmoSO ammoSO && ammoSO.ammoCategory == neededAmmo)
            {
                int amountToTake = Mathf.Min(slot.quantity, amountNeeded - ammoSupplied);

                // Subtract from inventory
                slot.quantity -= amountToTake;
                ammoSupplied += amountToTake;

                // Update UI
                if (slot.quantity > 0)
                {
                    slot.quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
                }
                else
                {
                    slot.ClearSlot();
                }

                if (ammoSupplied >= amountNeeded)
                    break; // full mag
            }
        }

        return ammoSupplied; // could be less than requested if not enough in inventory
    }

    public void RestoreEquippedWeapon(InventorySlot equipSlot, int currentAmmo)
    {
        WeaponSO weaponItem = equipSlot.itemData as WeaponSO;
        if (weaponItem == null) return;

        GameObject holder = null;
        if (weaponItem.weaponCategory == WeaponSO.WeaponCategory.Melee)
            holder = OneHandedMeleeHolder;
        else if (weaponItem.weaponCategory == WeaponSO.WeaponCategory.Pistol)
            holder = pistolHolder;

        if (holder != null)
        {
            if (currentRHEquippedItem != null)
                Destroy(currentRHEquippedItem);

            currentRHEquippedItem = Instantiate(weaponItem.inventoryPrefab, holder.transform);
            //currentRHEquippedItem.transform.localScale = new Vector3(1f / 18.10255f, 1f / 18.10255f, 1f / 18.10255f);
            currentRHEquippedItem.transform.localPosition = Vector3.zero;
            currentRHEquippedItem.transform.localRotation = Quaternion.identity;
        }
    }

    public void ClearUI()
    {
        foreach (var slot in slots)
        {
            slot.ClearSlotUI();
        }

        foreach(var slot in equipmentSlots)
        {
            slot.ClearSlotUI();
        }

        ClearItemInfo();
    }

    public void ClearItemInfo()
    {
        itemName.text = string.Empty;
        itemDesc.text = string.Empty;
    }

    public void ClearSlots()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }
    }

    public void ClearEquipmentSlots()
    {
        foreach (var slot in equipmentSlots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }
    }

    public void ShowOptions()
    {
        useButton.SetActive(true);
        dropButton.SetActive(true);
    }

    public void ShowEquipmentOptions()
    {
        dropButton.SetActive(true);
        unequipButton.SetActive(true);
    }

    public void HideOptions()
    {
        useButton.SetActive(false);
        dropButton.SetActive(false);
        unequipButton.SetActive(false);
    }
}
