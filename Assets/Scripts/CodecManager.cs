using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class CodecManager : MonoBehaviour
{
    private CharacterPanel charPanel;
    private InventorySelect invSelect;
    private Inventory inv;

    private Transform terrain;
    private Transform terrainManager;
    [HideInInspector] public bool terrainLoaded = false;

    public static int profileToLoadID;
    public static string playerName;
    public static Color hairColor;
    public static Color headColor;
    public static Color bodyColor;
    public static Color feetColor;

    public static CodecManager Instance;

    void OnEnable()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        try
        {
            charPanel = FindObjectOfType<CharacterPanel>();
            invSelect = FindObjectOfType<InventorySelect>();
            inv = Player.Instance.inventory;

            terrain = FindObjectOfType<TerrainMap>().transform;
            terrainManager = FindObjectOfType<TerrainManager>().transform;
        }
        catch (NullReferenceException)
        { }

        terrainLoaded = LoadTerrain();
    }

    void OnLevelWasLoaded()
    {
        try
        {
            charPanel = FindObjectOfType<CharacterPanel>();
            invSelect = FindObjectOfType<InventorySelect>();
            inv = Player.Instance.inventory;

            terrain = FindObjectOfType<TerrainMap>().transform;
            terrainManager = FindObjectOfType<TerrainManager>().transform;
        }
        catch (NullReferenceException)
        { }

        //terrainLoaded = LoadTerrain();
    }

    public void SaveInventories()
    {
        string content = string.Empty;

        for (int i = 0; i < inv.allSlots.Count; i++)
        {
            Slot tmp = inv.allSlots[i].GetComponent<Slot>();
            if (!tmp.IsEmpty)
                content += i + "-" + tmp.CurrentItem.Item.ItemName.ToString() + "-" + tmp.Items.Count.ToString() + ";";
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Inventory.dat");

        InventoryData invData = new InventoryData();
        invData.content = content;
        invData.slots = inv.slots;
        invData.rows = inv.rows;
        //invData.invPos = new float[,,] { { { inv.transform.position.x, inv.transform.position.y, inv.transform.position.z } } };

        bf.Serialize(file, invData);
        file.Close();

        content = string.Empty;

        for (int i = 0; i < invSelect.allSlots.Count; i++)
        {
            Slot tmp = invSelect.allSlots[i].GetComponent<Slot>();
            if (!tmp.IsEmpty)
                content += i + "-" + tmp.CurrentItem.Item.ItemName.ToString() + "-" + tmp.Items.Count.ToString() + ";";
        }

        bf = new BinaryFormatter();
        file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/InventorySelect.dat");

        InventoryData invSData = new InventoryData();
        invSData.content = content;
        invSData.slots = invSelect.slots;
        invSData.rows = invSelect.rows;
        //invSData.invPos = new float[,,] { { { invSelect.transform.position.x, invSelect.transform.position.y, invSelect.transform.position.z } } };

        bf.Serialize(file, invSData);
        file.Close();

        content = string.Empty;

        for (int i = 0; i < charPanel.equipmentSlots.Length; i++)
        {
            if (!charPanel.equipmentSlots[i].IsEmpty) content += i + "-" + charPanel.equipmentSlots[i].Items.Peek().Item.ItemName + ";";
        }

        bf = new BinaryFormatter();
        file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CharPanel.dat");

        InventoryData charData = new InventoryData();
        charData.content = content;

        bf.Serialize(file, charData);
        file.Close();
    }

    public void LoadInventories()
    {
        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Inventory.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Inventory.dat", FileMode.Open);

            InventoryData invData = (InventoryData)bf.Deserialize(file);
            file.Close();

            string content = invData.content;

            if (content != string.Empty)
            {
                inv.slots = invData.slots;
                inv.rows = invData.rows;

                inv.CreateLayout();
                //inv.transform.position = new Vector3(invData.invPos[0, 0, 0], invData.invPos[0, 0, 1], invData.invPos[0, 0, 2]);

                string[] splitContent = content.Split(';');

                for (int x = 0; x < splitContent.Length - 1; x++)
                {
                    string[] splitValues = splitContent[x].Split('-');
                    int index = Int32.Parse(splitValues[0]);
                    string itemName = splitValues[1];
                    int amount = Int32.Parse(splitValues[2]);
                    Item tmp = null;

                    for (int i = 0; i < amount; i++)
                    {
                        GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Placeables.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Tools.Find(item => item.ItemName == itemName);

                        loadedItem.AddComponent<ItemScript>();
                        loadedItem.GetComponent<ItemScript>().Item = tmp;

                        inv.allSlots[index].GetComponent<Slot>().AddItem(loadedItem.GetComponent<ItemScript>());

                        Destroy(loadedItem);
                    }
                }
            }
        }

        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/InventorySelect.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/InventorySelect.dat", FileMode.Open);

            InventoryData invData = (InventoryData)bf.Deserialize(file);
            file.Close();

            string content = invData.content;

            if (content != string.Empty)
            {
                invSelect.slots = invData.slots;
                invSelect.rows = invData.rows;

                invSelect.CreateLayout();
                //invSelect.transform.position = new Vector3(invData.invPos[0, 0, 0], invData.invPos[0, 0, 1], invData.invPos[0, 0, 2]);

                string[] splitContent = content.Split(';');

                for (int x = 0; x < splitContent.Length - 1; x++)
                {
                    string[] splitValues = splitContent[x].Split('-');
                    int index = Int32.Parse(splitValues[0]);
                    string itemName = splitValues[1];
                    int amount = Int32.Parse(splitValues[2]);
                    Item tmp = null;

                    for (int i = 0; i < amount; i++)
                    {
                        GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Placeables.Find(item => item.ItemName == itemName);

                        if (tmp == null)
                            tmp = InventoryManager.Instance.ItemContainer.Tools.Find(item => item.ItemName == itemName);

                        loadedItem.AddComponent<ItemScript>();
                        loadedItem.GetComponent<ItemScript>().Item = tmp;

                        invSelect.allSlots[index].GetComponent<Slot>().AddItem(loadedItem.GetComponent<ItemScript>());

                        Destroy(loadedItem);
                    }
                }
            }
        }

        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CharPanel.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CharPanel.dat", FileMode.Open);

            InventoryData invData = (InventoryData)bf.Deserialize(file);
            file.Close();

            foreach (Slot slot in charPanel.equipmentSlots)
            {
                slot.ClearSlot();
            }

            string content = invData.content;

            string[] splitContent = content.Split(';');

            for (int i = 0; i < splitContent.Length - 1; i++)
            {
                string[] splitValues = splitContent[i].Split('-');

                int index = Int32.Parse(splitValues[0]);

                string itemName = splitValues[1];

                GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);
                loadedItem.AddComponent<ItemScript>();

                if (index == 9 || index == 10)
                {
                    loadedItem.GetComponent<ItemScript>().Item = InventoryManager.Instance.ItemContainer.Weapons.Find(x => x.ItemName == itemName);
                }
                else
                {
                    loadedItem.GetComponent<ItemScript>().Item = InventoryManager.Instance.ItemContainer.Equipment.Find(x => x.ItemName == itemName);
                }

                charPanel.equipmentSlots[index].AddItem(loadedItem.GetComponent<ItemScript>());

                Destroy(loadedItem);

                charPanel.CalcStats();
            }
        }
    }

    public void SaveTerrain()
    {
        if (terrain != null && terrainManager != null)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + terrainManager.GetComponent<TerrainManager>().worldName + ".dat");

            MapData mData = new MapData();
            mData.currentMap = terrain.GetComponent<TerrainMap>().map;
            mData.brushMap = terrain.GetComponent<TerrainMap>().brushMap;
            mData.backMap = terrain.GetComponent<TerrainMap>().backMap;
            mData.spawnPoint = string.Empty;
            mData.spawnPoint += SpawnpointManager.Instance.transform.position.x + "-";
            mData.spawnPoint += SpawnpointManager.Instance.transform.position.y + "-";
            mData.spawnPoint += SpawnpointManager.Instance.transform.position.z;

            bf.Serialize(file, mData);
            file.Close();
        }
    }

    public bool LoadTerrain()
    {
        if (terrain != null && terrainManager != null)
        {
            if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + terrainManager.GetComponent<TerrainManager>().worldName + ".dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + terrainManager.GetComponent<TerrainManager>().worldName + ".dat", FileMode.Open);

                MapData mData = (MapData)bf.Deserialize(file);
                file.Close();

                terrain.GetComponent<TerrainMap>().map = mData.currentMap;
                terrain.GetComponent<TerrainMap>().brushMap = mData.brushMap;
                terrain.GetComponent<TerrainMap>().backMap = mData.backMap;

                if (mData.spawnPoint != string.Empty)
                {
                    string[] spawnPoint = mData.spawnPoint.Split('-');
                    Vector3 spawnVector;
                    if (spawnPoint.Length == 3)
                        spawnVector = new Vector3(float.Parse(spawnPoint[0]), float.Parse(spawnPoint[1]), float.Parse(spawnPoint[2]));
                    else
                        spawnVector = new Vector3(float.Parse(spawnPoint[1]), float.Parse(spawnPoint[2]), float.Parse(spawnPoint[3]));
                    SpawnpointManager.Instance.transform.position = spawnVector;

                    if (Player.Instance != null)
                        Player.Instance.transform.position = SpawnpointManager.Instance.transform.position;
                    GameObject.FindObjectOfType<TerrainManager>().spawnSet = true;
                }

                return true;
            }
        }

        return false;
    }

    public void SaveQuestData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Quests.dat");

        QuestData qData = new QuestData();
        qData.questsCompleted = QuestManager.Instance.questCompleted;
        qData.activeQuests = QuestUIManager.Instance.ActiveQuestSerializer();

        bf.Serialize(file, qData);
        file.Close();
    }

    public void LoadQuestData()
    {
        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Quests.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Quests.dat", FileMode.Open);

            QuestData qData = (QuestData)bf.Deserialize(file);
            file.Close();

            QuestManager.Instance.questCompleted = qData.questsCompleted;
            
            for (int i = 0; i < qData.activeQuests.Length; i++)
            {
                string[] splitValues = qData.activeQuests[i].Split('-');
                string questTitle = splitValues[0];
                string rewards = splitValues[1];
                int questIndex = Int32.Parse(splitValues[2]);
                string startText = splitValues[3];
                string endText = splitValues[4];
                bool isItemQuest = Boolean.Parse(splitValues[5]);
                string targetItem = splitValues[6];
                bool isEnemyQuest = Boolean.Parse(splitValues[7]);
                string targetEnemy = splitValues[8];
                int enemiesToKill = Int32.Parse(splitValues[9]);
                int enemyKillCount = Int32.Parse(splitValues[10]);

                QuestObject tmp = QuestManager.Instance.quests[questIndex];
                QuestManager.Instance.quests[questIndex].gameObject.SetActive(true);

                tmp.questTitle = questTitle;
                tmp.rewardsText = rewards;
                tmp.startText = startText;
                tmp.endText = endText;
                tmp.isItemQuest = isItemQuest;
                tmp.targetItem = targetItem;
                tmp.isEnemyQuest = isEnemyQuest;
                tmp.targetEnemy = targetEnemy;
                tmp.enemiesToKill = enemiesToKill;
                tmp.enemyKillCount = enemyKillCount;

                QItem qItem = new QItem();
                qItem.questName = QuestManager.Instance.quests[questIndex].questTitle;
                qItem.questDesc = QuestManager.Instance.quests[questIndex].startText;
                QuestManager.Instance.quests[questIndex].qItem = qItem;

                QuestUIManager.Instance.activeQuests.Add(tmp);
                QuestUIManager.Instance.UpdateQuestUI(0, false, tmp);
                QuestUIManager.Instance.currentAtIndex++;
            }
        }
    }

    public void SavePlayerData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + Player.Instance.name + ".dat");

        PlayerData pData = new PlayerData();
        pData.curHealth = Player.Instance.health.CurrentVal;
        pData.curMana = Player.Instance.mana.CurrentVal;
        pData.curMoney = Player.Instance.gold;

        pData.curLevel = Player.Instance.GetComponent<EXPManager>().curLevel;
        pData.curExp = Player.Instance.GetComponent<EXPManager>().currentExperience;
        pData.expToNextLvl = Player.Instance.GetComponent<EXPManager>().experienceToNextLvl;

        pData.curAbilitys = new int[AbilityManager.Instance.abilityHolders.Length];
        for (int i = 0; i < AbilityManager.Instance.abilityHolders.Length; i++)
        {
            pData.curAbilitys[i] = AbilityManager.Instance.abilityHolders[i].curDropdownIndex;
        }

        pData.curTime = GameManager.Instance.GetComponent<TODManager>().curTime;
        pData.isDay = GameManager.Instance.GetComponent<TODManager>().isDay;

        pData.developmentMode = MainMenuManager.developmentMode;

        bf.Serialize(file, pData);
        file.Close();
    }

    public void LoadPlayerData()
    {
        if (MainMenuManager.characterCreated && !MainMenuManager.developmentMode)
        {
            SettingsManager.Instance.devModeToggle.gameObject.SetActive(false);
            SettingsManager.Instance.enemySpawningToggle.gameObject.SetActive(false);
        }

        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + Player.Instance.name + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + Player.Instance.name + ".dat", FileMode.Open);

            PlayerData pData = (PlayerData)bf.Deserialize(file);
            file.Close();

            Player.Instance.health.CurrentVal = pData.curHealth;
            Player.Instance.mana.CurrentVal = pData.curMana;
            Player.Instance.gold = pData.curMoney;
            Player.Instance.moneyUI.GetComponentInChildren<Text>().text = "Gold:" + Player.Instance.gold;
            VendorInventory.Instance.goldText.text = "Gold:" + Player.Instance.Gold;

            Player.Instance.GetComponent<EXPManager>().curLevel = pData.curLevel;
            Player.Instance.GetComponent<EXPManager>().curLevelText.text = Player.Instance.GetComponent<EXPManager>().curLevel.ToString();
            Player.Instance.GetComponent<EXPManager>().currentExperience = pData.curExp;
            Player.Instance.GetComponent<EXPManager>().experienceToNextLvl = pData.expToNextLvl;

            for (int i = 0; i < pData.curAbilitys.Length; i++)
            {
                //AbilityManager.Instance.abilityHolders[i].curDropdownIndex = pData.curAbilitys[i];
                AbilityManager.Instance.LoadAbilityHolder(i, pData.curAbilitys[i]);
            }

            GameManager.Instance.GetComponent<TODManager>().normalizedTime = pData.curTime;
            GameManager.Instance.GetComponent<TODManager>().isDay = pData.isDay;

            if (!pData.developmentMode)
            {
                MainMenuManager.developmentMode = false;
                SettingsManager.Instance.devModeToggle.gameObject.SetActive(false);
                SettingsManager.Instance.enemySpawningToggle.gameObject.SetActive(false);
            }
        }
    }

    public void SavePreviewData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CraftingPreview.dat");

        CraftingPreviewData cData = new CraftingPreviewData();

        foreach(KeyValuePair<string, string> previews in CraftingPreviewManager.craftingList)
        {
            cData.previews += previews.Key + ":" + previews.Value + ";";
        }

        bf.Serialize(file, cData);
        file.Close();
    }

    public void LoadPreviewData()
    {
        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CraftingPreview.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CraftingPreview.dat", FileMode.Open);

            CraftingPreviewData cData = (CraftingPreviewData)bf.Deserialize(file);
            file.Close();

            string[] splitContent = cData.previews.Split(';');

            for (int i = 0; i < splitContent.Length - 1; i++)
            {
                string[] splitsplitContent = splitContent[i].Split(':');
                CraftingPreviewManager.Instance.AddRecipe(splitsplitContent[0], splitsplitContent[1]);
                CraftingBench.remainingRecipes.Remove(splitsplitContent[1]);
            }
        }
    }

    public void SaveMenuData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Menu.dat");

        MenuData mData = new MenuData();

        mData.overallIncome = MenuManager.Instance.overallIncome;
        mData.overallDeaths = MenuManager.Instance.overallDeaths;
        mData.enemyKills = MenuManager.Instance.enemyKills;

        bf.Serialize(file, mData);
        file.Close();
    }

    public void LoadMenuData()
    {
        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Menu.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Menu.dat", FileMode.Open);

            MenuData mData = (MenuData)bf.Deserialize(file);
            file.Close();

            MenuManager.Instance.overallIncome = mData.overallIncome;
            MenuManager.Instance.overallDeaths = mData.overallDeaths;
            MenuManager.Instance.enemyKills = mData.enemyKills;
        }
    }

    public void SaveProfiles(GameObject[] charSlots)
    {
        for (int i = 0; i < charSlots.Length; i++)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + charSlots[i].GetComponent<ProfileManager>().profileName + charSlots[i].GetComponent<ProfileManager>().id + ".dat");

            ProfileData pData = new ProfileData();
            pData.name = charSlots[i].GetComponent<ProfileManager>().profileName;
            pData.id = charSlots[i].GetComponent<ProfileManager>().id;

            pData.hairValue = (int)MainMenuManager.Instance.hairSlider.value;
            pData.headValue = (int)MainMenuManager.Instance.headSlider.value;
            pData.bodyValue = (int)MainMenuManager.Instance.bodySlider.value;
            pData.feetValue = (int)MainMenuManager.Instance.feetSlider.value;

            pData.hairColorValues = charSlots[i].GetComponent<ProfileManager>().hairImage.color.r + "-" + charSlots[i].GetComponent<ProfileManager>().hairImage.color.g
                + "-" + charSlots[i].GetComponent<ProfileManager>().hairImage.color.b;
            pData.headColorValues = charSlots[i].GetComponent<ProfileManager>().headImage.color.r + "-" + charSlots[i].GetComponent<ProfileManager>().headImage.color.g
                + "-" + charSlots[i].GetComponent<ProfileManager>().headImage.color.b;
            pData.bodyColorValues = charSlots[i].GetComponent<ProfileManager>().bodyImage.color.r + "-" + charSlots[i].GetComponent<ProfileManager>().bodyImage.color.g
                + "-" + charSlots[i].GetComponent<ProfileManager>().bodyImage.color.b;
            pData.feetColorValues = charSlots[i].GetComponent<ProfileManager>().feetImage.color.r + "-" + charSlots[i].GetComponent<ProfileManager>().feetImage.color.g
                + "-" + charSlots[i].GetComponent<ProfileManager>().feetImage.color.b;

            bf.Serialize(file, pData);
            file.Close();
        }
    }

    public void LoadProfiles(GameObject[] charSlots)
    {
        for (int i = 0; i < charSlots.Length; i++)
        {
            if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Profile" + charSlots[i].GetComponent<ProfileManager>().id + ".dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Profile" + charSlots[i].GetComponent<ProfileManager>().id + ".dat", FileMode.Open);

                try
                {
                    ProfileData pData = (ProfileData)bf.Deserialize(file);
                    file.Close();

                    charSlots[i].GetComponent<ProfileManager>().profileName = pData.name;
                    charSlots[i].GetComponent<ProfileManager>().id = pData.id;
                    profileToLoadID = pData.id;
                    charSlots[i].GetComponentInChildren<Text>().text = pData.name;

                    if (GameManager.Instance == null)
                    {
                        MainMenuManager.Instance.hairImage.sprite = MainMenuManager.Instance.GetComponent<CharacterSpriteManager>().hairSprites[pData.hairValue].sprite;
                        MainMenuManager.Instance.headImage.sprite = MainMenuManager.Instance.GetComponent<CharacterSpriteManager>().headSprites[pData.headValue].sprite;
                        MainMenuManager.Instance.bodyImage.sprite = MainMenuManager.Instance.GetComponent<CharacterSpriteManager>().bodySprites[pData.bodyValue].sprite;
                        MainMenuManager.Instance.feetImage.sprite = MainMenuManager.Instance.GetComponent<CharacterSpriteManager>().feetSprites[pData.feetValue].sprite;
                    }

                    if (GameManager.Instance != null)
                    {
                        for (int ith = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites.Length; i++)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[ith].spriteRenderer.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().hairSprites[pData.hairValue].sprite;
                        for (int ith = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites.Length; i++)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[ith].spriteRenderer.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().headSprites[pData.headValue].sprite;
                        for (int ith = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites.Length; i++)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[ith].spriteRenderer.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().bodySprites[pData.bodyValue].sprite;
                        for (int ith = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites.Length; i++)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[ith].spriteRenderer.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().feetSprites[pData.feetValue].sprite;
                    }

                    string[] splitContent = pData.hairColorValues.Split('-');
                    hairColor = new Color(float.Parse(splitContent[0]), float.Parse(splitContent[1]), float.Parse(splitContent[2]));
                    splitContent = pData.headColorValues.Split('-');
                    headColor = new Color(float.Parse(splitContent[0]), float.Parse(splitContent[1]), float.Parse(splitContent[2]));
                    splitContent = pData.bodyColorValues.Split('-');
                    bodyColor = new Color(float.Parse(splitContent[0]), float.Parse(splitContent[1]), float.Parse(splitContent[2]));
                    splitContent = pData.feetColorValues.Split('-');
                    feetColor = new Color(float.Parse(splitContent[0]), float.Parse(splitContent[1]), float.Parse(splitContent[2]));

                    MainMenuManager.Instance.hairImage.color = hairColor;
                    MainMenuManager.Instance.headImage.color = headColor;
                    MainMenuManager.Instance.bodyImage.color = bodyColor;
                    MainMenuManager.Instance.feetImage.color = feetColor;
                }
                catch (Exception e)
                {
                    if (e is EndOfStreamException || e is IndexOutOfRangeException)
                        MainMenuManager.Instance.OpenDeleteMenu();
                }
            }
        }
    }

    public void SaveProfile(string profileName, int id)
    {
        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Profile" + id + ".dat"))
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/Profile" + CodecManager.profileToLoadID);
            foreach (FileInfo fileToDelete in di.GetFiles())
            {
                fileToDelete.Delete();
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Profile" + id + ".dat");

        ProfileData pData = new ProfileData();
        pData.name = profileName;
        pData.id = id;

        pData.hairValue = (int)MainMenuManager.Instance.hairSlider.value;
        pData.headValue = (int)MainMenuManager.Instance.headSlider.value;
        pData.bodyValue = (int)MainMenuManager.Instance.bodySlider.value;
        pData.feetValue = (int)MainMenuManager.Instance.feetSlider.value;

        GameObject profileManager = GameObject.Find("ProfileSlot01");
        pData.hairColorValues = profileManager.GetComponent<ProfileManager>().hairImage.color.r + "-" + profileManager.GetComponent<ProfileManager>().hairImage.color.g
                + "-" + profileManager.GetComponent<ProfileManager>().hairImage.color.b;
        pData.headColorValues = profileManager.GetComponent<ProfileManager>().headImage.color.r + "-" + profileManager.GetComponent<ProfileManager>().headImage.color.g
            + "-" + profileManager.GetComponent<ProfileManager>().headImage.color.b;
        pData.bodyColorValues = profileManager.GetComponent<ProfileManager>().bodyImage.color.r + "-" + profileManager.GetComponent<ProfileManager>().bodyImage.color.g
            + "-" + profileManager.GetComponent<ProfileManager>().bodyImage.color.b;
        pData.feetColorValues = profileManager.GetComponent<ProfileManager>().feetImage.color.r + "-" + profileManager.GetComponent<ProfileManager>().feetImage.color.g
            + "-" + profileManager.GetComponent<ProfileManager>().feetImage.color.b;

        bf.Serialize(file, pData);
        file.Close();
    }

    public bool LoadProfile(string profileName, int id)
    {
        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/Profile" + id + ".dat"))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + profileName + id + ".dat", FileMode.Open);
                ProfileData pData = (ProfileData)bf.Deserialize(file);
                file.Close();

                profileName = pData.name;
                profileToLoadID = pData.id;
                playerName = profileName;

                if (GameManager.Instance == null)
                {
                    MainMenuManager.Instance.hairImage.sprite = MainMenuManager.Instance.GetComponent<CharacterSpriteManager>().hairSprites[pData.hairValue].sprite;
                    MainMenuManager.Instance.headImage.sprite = MainMenuManager.Instance.GetComponent<CharacterSpriteManager>().headSprites[pData.headValue].sprite;
                    MainMenuManager.Instance.bodyImage.sprite = MainMenuManager.Instance.GetComponent<CharacterSpriteManager>().bodySprites[pData.bodyValue].sprite;
                    MainMenuManager.Instance.feetImage.sprite = MainMenuManager.Instance.GetComponent<CharacterSpriteManager>().feetSprites[pData.feetValue].sprite;
                }

                if (GameManager.Instance != null)
                {
                    for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites.Length; i++)
                    {
                        if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer != null)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().hairSprites[pData.hairValue].sprite;
                        if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].sprite != null)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].sprite.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().hairSprites[pData.hairValue].sprite;
                    }
                    for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites.Length; i++)
                    {
                        if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer != null)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curHeadSprites[i].spriteRenderer.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().headSprites[pData.headValue].sprite;
                        if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].sprite != null)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].sprite.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().hairSprites[pData.hairValue].sprite;
                    }
                    for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites.Length; i++)
                    {
                        if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer != null)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curBodySprites[i].spriteRenderer.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().bodySprites[pData.bodyValue].sprite;
                        if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].sprite != null)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].sprite.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().hairSprites[pData.hairValue].sprite;
                    }
                    for (int i = 0; i < GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites.Length; i++)
                    {
                        if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].spriteRenderer != null)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curFeetSprites[i].spriteRenderer.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().feetSprites[pData.feetValue].sprite;
                        if (GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].sprite != null)
                            GameManager.Instance.GetComponent<CharacterSpriteManager>().curHairSprites[i].sprite.sprite = GameManager.Instance.GetComponent<CharacterSpriteManager>().hairSprites[pData.hairValue].sprite;
                    }
                }

                string[] splitContent = pData.hairColorValues.Split('-');
                hairColor = new Color(float.Parse(splitContent[0]), float.Parse(splitContent[1]), float.Parse(splitContent[2]));
                splitContent = pData.headColorValues.Split('-');
                headColor = new Color(float.Parse(splitContent[0]), float.Parse(splitContent[1]), float.Parse(splitContent[2]));
                splitContent = pData.bodyColorValues.Split('-');
                bodyColor = new Color(float.Parse(splitContent[0]), float.Parse(splitContent[1]), float.Parse(splitContent[2]));
                splitContent = pData.feetColorValues.Split('-');
                feetColor = new Color(float.Parse(splitContent[0]), float.Parse(splitContent[1]), float.Parse(splitContent[2]));

                return true;
            }
            catch(Exception e)
            {
                if (e is EndOfStreamException || e is IOException)
                    return false;
            }
        }

        return false;
    }

    public void DeleteProfile(int id)
    {
        MainMenuManager.Instance.OpenDeleteMenu();
    }

    public void SaveCharacterTreeData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CharTree.dat");

        CharacterTreeData cData = new CharacterTreeData();
        cData.skillPoints = SkillTreeManager.Instance.skillPoints;

        for (int i = 0; i < SkillTreeManager.Instance.skillTrees[0].nodes.Length; i++)
        {
            cData.treeData += SkillTreeManager.Instance.skillTrees[0].nodes[i].hasSkill + "-";
        }
        cData.treeDataNodes = SkillTreeManager.Instance.skillTrees[0].acuiredNodes;
        for (int i = 0; i < SkillTreeManager.Instance.skillTrees[1].nodes.Length; i++)
        {
            cData.treeData1 += SkillTreeManager.Instance.skillTrees[1].nodes[i].hasSkill + "-";
        }
        cData.treeDataNodes1 = SkillTreeManager.Instance.skillTrees[1].acuiredNodes;
        for (int i = 0; i < SkillTreeManager.Instance.skillTrees[2].nodes.Length; i++)
        {
            cData.treeData2 += SkillTreeManager.Instance.skillTrees[2].nodes[i].hasSkill + "-";
        }
        cData.treeDataNodes2 = SkillTreeManager.Instance.skillTrees[2].acuiredNodes;

        bf.Serialize(file, cData);
        file.Close();
    }

    public void LoadCharacterTreeData()
    {
        if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CharTree.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/CharTree.dat", FileMode.Open);

            CharacterTreeData cData = (CharacterTreeData)bf.Deserialize(file);
            file.Close();

            SkillTreeManager.Instance.skillPoints = cData.skillPoints;
            SkillTreeManager.Instance.AdjustCurrentPoints();

            string[] splitValues = cData.treeData.Split('-');
            for (int i = 0; i < splitValues.Length; i++)
            {
                if (i < SkillTreeManager.Instance.skillTrees[0].nodes.Length)
                {
                    SkillTreeManager.Instance.skillTrees[0].nodes[i].hasSkill = Boolean.Parse(splitValues[i]);
                    if (SkillTreeManager.Instance.skillTrees[0].nodes[i].hasSkill)
                        SkillTreeManager.Instance.skillTrees[0].nodes[i].TriggerNode();
                }

            }
            SkillTreeManager.Instance.skillTrees[0].acuiredNodes = cData.treeDataNodes;
            splitValues = cData.treeData1.Split('-');
            for (int i = 0; i < splitValues.Length; i++)
            {
                if (i < SkillTreeManager.Instance.skillTrees[1].nodes.Length)
                {
                    SkillTreeManager.Instance.skillTrees[1].nodes[i].hasSkill = Boolean.Parse(splitValues[i]);
                    if (SkillTreeManager.Instance.skillTrees[1].nodes[i].hasSkill)
                        SkillTreeManager.Instance.skillTrees[1].nodes[i].TriggerNode();
                }
            }
            SkillTreeManager.Instance.skillTrees[1].acuiredNodes = cData.treeDataNodes1;
            splitValues = cData.treeData2.Split('-');
            for (int i = 0; i < splitValues.Length; i++)
            {
                if (i < SkillTreeManager.Instance.skillTrees[2].nodes.Length)
                {
                    SkillTreeManager.Instance.skillTrees[2].nodes[i].hasSkill = Boolean.Parse(splitValues[i]);
                    if (SkillTreeManager.Instance.skillTrees[2].nodes[i].hasSkill)
                        SkillTreeManager.Instance.skillTrees[2].nodes[i].TriggerNode();
                }
            }
            SkillTreeManager.Instance.skillTrees[2].acuiredNodes = cData.treeDataNodes2;
        }
    }

    public void SaveChestData(ChestInventory linkedInventory, List<Stack<ItemScript>> allSlots, GameObject go, bool rewardsCall, string chestRewards, string contentName)
    {
        if (TerrainManager.Instance != null)
        {
            string goNameWithoutSpaces = string.Empty;
            if (go != null)
                goNameWithoutSpaces = go.name.Replace(":", string.Empty);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;
            if (!rewardsCall)
                file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + goNameWithoutSpaces + TerrainManager.Instance.worldName + "content" + ".dat");
            else
                file = File.Create(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + contentName + TerrainManager.Instance.worldName + "content" + ".dat");

            ChestData cData = new ChestData();

            string content = string.Empty;
            if (!rewardsCall)
            {
                for (int i = 0; i < allSlots.Count; i++)
                {
                    if (allSlots[i] != null && allSlots[i].Count > 0)
                        content += i + "-" + allSlots[i].Peek().Item.ItemName + "-" + allSlots[i].Count.ToString() + ";";
                }
            }

            if (linkedInventory == null || linkedInventory.gameObject.name != "VendorInventory")
            {
                if (!rewardsCall)
                    cData.content = content;
                else
                    cData.content = chestRewards;
            }

            //Debug.Log("saved: " + cData.content);

            bf.Serialize(file, cData);
            file.Close();
        }
    }

    public string LoadChestData(GameObject go)
    {
        if (TerrainManager.Instance != null)
        {
            string goNameWithoutSpaces = go.name.Replace(":", string.Empty);
            if (File.Exists(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + goNameWithoutSpaces + TerrainManager.Instance.worldName + "content" + ".dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/Profile" + profileToLoadID + "/" + goNameWithoutSpaces + TerrainManager.Instance.worldName + "content" + ".dat", FileMode.Open);

                ChestData cData = (ChestData)bf.Deserialize(file);
                file.Close();

                return cData.content;
            }
        }
        return string.Empty;
    }
}

[Serializable]
class ProfileData
{
    public string name;
    public int id;

    public string hairColorValues;
    public string headColorValues;
    public string bodyColorValues;
    public string feetColorValues;

    public int hairValue;
    public int headValue;
    public int bodyValue;
    public int feetValue;
}

[Serializable]
class MapData
{
    public int[,] currentMap;
    public int[,] brushMap;
    public int[,] backMap;
    public string spawnPoint;
}

[Serializable]
class PlayerData
{
    public float curHealth;
    public float curMana;
    public int curMoney;

    public int curLevel;
    public int curExp;
    public int expToNextLvl;

    public int[] curAbilitys;

    public float curTime;
    public bool isDay;

    public bool developmentMode;
}

[Serializable]
class InventoryData
{
    public string content;
    public int slots;
    public int rows;
}

[Serializable]
class QuestData
{
    public bool[] questsCompleted;
    public string[] activeQuests;
}

[Serializable]
class MenuData
{
    public int overallIncome;
    public int enemyKills;
    public int overallDeaths;
}

[Serializable]
class CharacterTreeData
{
    public int skillPoints;

    public string treeData;
    public int treeDataNodes;

    public string treeData1;
    public int treeDataNodes1;

    public string treeData2;
    public int treeDataNodes2;
}

[Serializable]
class CraftingPreviewData
{
    public string previews;
}

[Serializable]
class ChestData
{
    public string content;
}