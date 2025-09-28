using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantityText;
    public TMP_Text currentAmmoText;

    public ItemSO itemData;
    public int quantity;
    public int currentAmmo;

    public void SetSlot(ItemSO newItem, int newQuantity, int newCurrentAmmo)
    {
        itemData = newItem;
        quantity = newQuantity;
        currentAmmo = newCurrentAmmo;

        if (itemData != null)
        {
            icon.sprite = itemData.icon;
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
            if (currentAmmoText != null)
            {
                currentAmmoText.text = currentAmmo > 0 ? currentAmmo.ToString() : "";
            }
            else
            {
                return;
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        itemData = null;
        quantity = 0;
        currentAmmo = 0;
        ClearSlotUI();
    }

    public void ClearSlotUI()
    {
        icon.sprite = null;
        quantityText.text = "";
        if (currentAmmoText != null)
        {
            currentAmmoText.text = "";
        }
        else
        {
            return;
        }
    }

    public void OnSlotClicked()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.SelectSlot(this);
        }
    }

    public void CurrentGunAmmoUpdate()
    {
        if (currentAmmoText != null)
        {
            currentAmmoText.text = currentAmmo > 0 ? currentAmmo.ToString() : "0";
        }
    }
}
