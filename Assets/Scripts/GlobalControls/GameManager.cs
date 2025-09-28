using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject pauseMenu;
    public GameObject gameOverMenu;

    [Header("PersistenObjects")]
    public GameObject[] persistentObjects;

    public string sceneToLoad;
    public string sceneToSceneID;
    public string areaToAreaID;

    public bool mainMenuOpen = false;
    public bool inventoryMenuOpen = false;
    public bool pauseMenuOpen = false;
    public bool gameOverMenuOpen = false;
    public bool interactPromptOpen = false;

    public List<InventorySlotData> savedInventory;
    public List<InventorySlotData> savedEquipment;

    public string saveFilePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    private void Awake()
    {
        if (Instance != null)
        {
            CleanUpAndDestroy();
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
        }
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!pauseMenuOpen && !inventoryMenuOpen && !interactPromptOpen && !gameOverMenuOpen && !mainMenuOpen)
            {
                pauseMenuOpen = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    private void MarkPersistentObjects()
    {
        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }

        Destroy(gameObject);
    }

    public void ResumeButton()
    {
        pauseMenuOpen = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void SaveButton()
    {
        SaveData data = new SaveData();
        data.currentScene = SceneManager.GetActiveScene().buildIndex;
        data.playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        data.playerCurrentHealth = PlayerStats.Instance.currentHealth;
        data.playerCurrentSanity = PlayerStats.Instance.currentSanity;

        ClearSavedData();

        foreach (var slot in InventoryManager.Instance.slots)
        {
            if (slot.itemData != null)
            {
                InventorySlotData slotData = new InventorySlotData
                {
                    itemID = slot.itemData.itemID,
                    quantity = slot.quantity,
                    currentAmmo = slot.currentAmmo
                };
                savedInventory.Add(slotData);
            }
        }
        data.inventoryData = savedInventory;

        foreach (var slot in InventoryManager.Instance.equipmentSlots)
        {
            if (slot.itemData != null)
            {
                InventorySlotData slotData = new InventorySlotData
                {
                    itemID = slot.itemData.itemID,
                    quantity = slot.quantity,
                    currentAmmo = slot.currentAmmo
                };
                savedEquipment.Add(slotData);
            }
        }
        data.equipmentData = savedEquipment;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        ResumeButton();
    }

    public void LoadButton()
    {
        if (!File.Exists(saveFilePath)) return;

        string json = File.ReadAllText(saveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        ItemSODatabase db = Resources.Load<ItemSODatabase>("ItemSO Database");

        int sceneIndex = data.currentScene;

        // Restore player stats
        PlayerStats.Instance.SetHealth(data.playerCurrentHealth);
        PlayerStats.Instance.SetSanity(data.playerCurrentSanity);

        GameObject.FindGameObjectWithTag("Player").transform.position = data.playerPosition;

        // Assign inventories from save data
        savedInventory = data.inventoryData;
        savedEquipment = data.equipmentData;

        // Clear UI before restoring
        InventoryManager.Instance.ClearUI();

        InventoryManager.Instance.ClearSlots();
        // Restore normal inventory
        for (int i = 0; i < savedInventory.Count; i++)
        {
            var slotData = savedInventory[i];
            if (!string.IsNullOrEmpty(slotData.itemID))
            {
                ItemSO itemData = db.GetItem(slotData.itemID);
                if (itemData != null)
                {
                    InventoryManager.Instance.slots[i]
                        .SetSlot(itemData, slotData.quantity, slotData.currentAmmo);
                }
            }
        }

        InventoryManager.Instance.ClearEquipmentSlots();
        // Restore equipment
        for (int i = 0; i < savedEquipment.Count; i++)
        {
            var slotData = savedEquipment[i];
            if (!string.IsNullOrEmpty(slotData.itemID))
            {
                ItemSO itemData = db.GetItem(slotData.itemID);
                if (itemData != null)
                {
                    InventoryManager.Instance.equipmentSlots[i]
                        .SetSlot(itemData, slotData.quantity, slotData.currentAmmo);

                    // If weapon, spawn prefab into hand
                    if (itemData.itemType == ItemType.Weapon)
                    {
                        InventoryManager.Instance.RestoreEquippedWeapon(
                            InventoryManager.Instance.equipmentSlots[i],
                            slotData.currentAmmo
                        );
                    }
                }
            }
        }

        ClearSavedData();
        pauseMenuOpen = false;
        pauseMenu.SetActive(false);
        gameOverMenuOpen = false;
        gameOverMenu.SetActive(false);

        PlayerStats.Instance.ResetTimers();
        LightDetector.Instance.ResetLights();
        Interact.Instance.ResetInteract();

        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(sceneIndex);
        ResumeButton();
    }

    public void SettingsButton()
    {

    }

    public void QuitButton()
    {
        InventoryManager.Instance.ClearUI();

        if (InventoryManager.Instance.currentRHEquippedItem != null)
        {
        Destroy(InventoryManager.Instance.currentRHEquippedItem);
        InventoryManager.Instance.currentRHEquippedItem = null;
        }

        InventoryManager.Instance.ClearSlots();
        InventoryManager.Instance.ClearEquipmentSlots();

        ClearSavedData();

        pauseMenuOpen = false;
        pauseMenu.SetActive(false);
        gameOverMenuOpen = false;
        gameOverMenu.SetActive(false);

        PlayerStats.Instance.ResetPlayerStats();
        LightDetector.Instance.ResetLights();
        Interact.Instance.ResetInteract();

        sceneToSceneID = "any_mainmenu";
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ShowGameOverMenu()
    {

    }

    public void TeleportPlayer()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("AreaSpawnPoint"))
        {
            if (obj.GetComponent<PlayerAreaSpawnPoint>().spawnID == areaToAreaID)
            {
                areaToAreaID = null;

                player.transform.position = obj.transform.position;
                player.transform.rotation = obj.transform.rotation;
            }
        }
    }

    public void ClearSceneData()
    {
        sceneToLoad = null;
        sceneToSceneID = null;
    }

    public void ClearSavedData()
    {
        savedEquipment.Clear();
        savedInventory.Clear();
    }

    [System.Serializable]
    class SaveData
    {
        public int currentScene;
        public Vector3 playerPosition;
        public float playerCurrentHealth;
        public float playerCurrentSanity;
        public List<InventorySlotData> inventoryData = new List<InventorySlotData>();
        public List<InventorySlotData> equipmentData = new List<InventorySlotData>();
    }

    [System.Serializable]
    public class InventorySlotData
    {
        public string itemID;
        public int quantity;
        public int currentAmmo;
    }
}
