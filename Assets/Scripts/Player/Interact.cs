using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interact : MonoBehaviour
{
    public static Interact Instance;

    public GameObject interactPrompt;
    public TextMeshProUGUI prompt;

    [SerializeField] private ItemData itemInRange;
    [SerializeField] private string itemName;
    [SerializeField] private int itemQuantity;
    [SerializeField] private int itemCurrentAmmo;
    [SerializeField] private InteractObjectData objectInRange;
    //[SerializeField] private bool hasAreaChanger = false;
    [SerializeField] private bool hasSceneChanger = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && !GameManager.Instance.pauseMenuOpen
            && !GameManager.Instance.gameOverMenuOpen && !GameManager.Instance.gameOverMenuOpen)
        {
            // Case: prompt already open -> close it
            if (GameManager.Instance.interactPromptOpen)
            {
                HidePrompt();
                GameManager.Instance.interactPromptOpen = false;
                return;
            }

            // Case: trying to pick up item
            if (itemInRange != null && !GameManager.Instance.inventoryMenuOpen)
            {
                itemName = itemInRange.item.itemName;
                itemQuantity = itemInRange.quantityPerPickup;
                itemCurrentAmmo = itemInRange.currentAmmo;

                //Debug.Log("ammo: " + itemCurrentAmmo);

                int leftover = InventoryManager.Instance.AddItemToInventory(itemInRange.item, itemQuantity, itemCurrentAmmo);

                if (leftover == itemQuantity)
                {
                    // nothing picked up -> inventory full
                    ShowPrompt("Inventory is full!", -1);
                    GameManager.Instance.interactPromptOpen = true;
                }
                else if (leftover <= 0)
                {
                    // picked up everything
                    ShowPrompt(itemName, itemQuantity);
                    Destroy(itemInRange.gameObject);
                    itemInRange = null;
                    GameManager.Instance.interactPromptOpen = true;
                }
                else
                {
                    // partial pickup
                    int pickedUp = itemQuantity - leftover;
                    ShowPrompt(itemName, pickedUp);
                    itemInRange.quantityPerPickup = leftover;
                    GameManager.Instance.interactPromptOpen = true;
                }

                itemName = null;
                itemQuantity = 0;
                itemCurrentAmmo = 0;
            }
            else if (objectInRange != null && !GameManager.Instance.inventoryMenuOpen)
            {
                string prompt = objectInRange.prompt;
                ShowPrompt(prompt, -1);
                GameManager.Instance.interactPromptOpen = true;
            }

            /*if (hasAreaChanger == true && GameManager.Instance.areaToAreaID != null)
            {
                hasAreaChanger = false;
                GameManager.Instance.TeleportPlayer();
            }*/
            if (hasSceneChanger == true && GameManager.Instance.sceneToLoad != null)
            {
                hasSceneChanger = false;
                SceneManager.LoadSceneAsync(GameManager.Instance.sceneToLoad);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            objectInRange = other.GetComponent<InteractObjectData>();
        }
        /*else if (other.CompareTag("AreaChanger"))
        {
            hasAreaChanger = true;
            GameManager.Instance.areaToAreaID = other.GetComponent<AreaChanger>().areaToAreaID;
        }*/
        else if (other.CompareTag("SceneChanger"))
        {
            hasSceneChanger = true;
            GameManager.Instance.sceneToLoad = other.GetComponent<SceneChanger>().sceneToLoad;
            GameManager.Instance.sceneToSceneID = other.GetComponent<SceneChanger>().sceneToSceneID;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            itemInRange = other.GetComponent<ItemData>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            itemInRange = null;
        }
        else if (other.CompareTag("Interactable"))
        {
            objectInRange = null;
        }
        /*else if (other.CompareTag("AreaChanger"))
        {
            hasAreaChanger = false;
            GameManager.Instance.areaToAreaID = null;
        }*/
        else if (other.CompareTag("SceneChanger"))
        {
            hasSceneChanger = false;
            GameManager.Instance.sceneToLoad = null;
            GameManager.Instance.sceneToSceneID = null;
        }
    }

    public void ShowPrompt(string name, int quantity)
    {
        interactPrompt.SetActive(true);

        if (quantity > 0)
            prompt.text = "You have picked up [" + name + "] x" + quantity + ".";
        else
            prompt.text = name; // e.g. "Inventory is full!"

        Time.timeScale = 0;
    }

    public void HidePrompt()
    {
        interactPrompt.SetActive(false);
        prompt.text = "";
        Time.timeScale = 1;
    }

    public void ResetInteract()
    {
        itemInRange = null;
        itemName = null;
        itemQuantity = 0;
        itemCurrentAmmo = 0;
        objectInRange = null;
        hasSceneChanger = false;
    }
}
