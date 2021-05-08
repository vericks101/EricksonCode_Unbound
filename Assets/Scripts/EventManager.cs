using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Event
{
    public string eventName;
    public string trackName;
    public float eventAbundance;
    public float spawnRate;
    public int killsRequired;

    public bool itemTrigger;
    public string itemName;

    public bool specificKill;
    public GameObject specificSpawn;

    public int goldReward;
    public int experienceReward;
    public string itemReward;
}

public class EventManager : MonoBehaviour
{
    [SerializeField] private Event[] events;
    private Event currentEvent;
    public int killCounter = 0;
    private float lastEventTryTime = 0f;
    private float eventTryTimeDebuff = 120f;

    [SerializeField] private Text eventTitleText;
    public Image eventFillImage;
    [SerializeField] private Animator eventCanvasAnimator;

    public Event CurrentEvent
    {
        get { return currentEvent; }
        set { currentEvent = value; }
    }

    private void OnLevelWasLoaded()
    {
        lastEventTryTime = Time.time + eventTryTimeDebuff;
    }

    private void Update()
    {
        if (eventTitleText == null)
            eventTitleText = GameObject.Find("EventTitleText").GetComponent<Text>();
        if (eventFillImage == null)
            eventFillImage = GameObject.Find("EventStatusImage").GetComponent<Image>();
        if (eventCanvasAnimator == null)
            eventCanvasAnimator = GameObject.Find("EventCanvas").GetComponent<Animator>();

        if (Time.time > lastEventTryTime && currentEvent == null && !CurrentSceneManager.Instance.noEvents)
        {
            if (currentEvent == null)
            {
                int randomIndex = Random.Range(0, events.Length);
                float abundance = events[randomIndex].eventAbundance;
                if (Random.Range(0f, 1f) < abundance && !events[randomIndex].itemTrigger)
                {
                    StartEvent(randomIndex);
                }
            }

            lastEventTryTime = Time.time + eventTryTimeDebuff;
        }
        if (currentEvent != null)
            CheckCurrentEvent();
    }

    public void StartEvent(int i)
    {
        CurrentSceneManager.Instance.GetComponent<SpawningManager>().spawnRate = events[i].spawnRate;
        if (AudioManager.instance.currentTrack != null)
            AudioManager.instance.StopTrack(AudioManager.instance.currentTrack.name);
        AudioManager.instance.ChangeSoundTrack(events[i].trackName, CurrentSceneManager.Instance.ignoreTrackChange);
        currentEvent = events[i];

        if (currentEvent.specificSpawn)
            SpawningManager.Instance.SpawnSpecificEnemy(events[i].specificSpawn);
        eventTitleText.text = events[i].eventName;
        eventFillImage.fillAmount = 0f;
        eventCanvasAnimator.SetTrigger("FadeIn");
    }

    public void EndEvent(bool reward)
    {
        if (reward)
        {
            Player.Instance.Gold += currentEvent.goldReward;
            MenuManager.Instance.overallIncome += currentEvent.goldReward;
            Player.Instance.GetComponent<EXPManager>().UpdateCurrentExperience(currentEvent.experienceReward);
            GiveItem(currentEvent.itemReward, 1);
            Player.Instance.inventorySelect.ChangeCurrentItemText();
        }

        currentEvent = null;

        if (GameManager.Instance.GetComponent<TODManager>().isDay)
        {
            AudioManager.instance.StopTrack(AudioManager.instance.currentTrack.name);
            AudioManager.instance.ChangeSoundTrack(CurrentSceneManager.Instance.dayTrack, CurrentSceneManager.Instance.ignoreTrackChange);
        }
        else if (!GameManager.Instance.GetComponent<TODManager>().isDay)
        {
            AudioManager.instance.StopTrack(AudioManager.instance.currentTrack.name);
            AudioManager.instance.ChangeSoundTrack(CurrentSceneManager.Instance.nightTrack, CurrentSceneManager.Instance.ignoreTrackChange);
        }
        lastEventTryTime = Time.time + GameManager.Instance.eventCD;
        CurrentSceneManager.Instance.GetComponent<SpawningManager>().spawnRate = 0;
        killCounter = 0;

        eventCanvasAnimator.SetTrigger("FadeOut");
    }

    private void CheckCurrentEvent()
    {
        if (currentEvent.specificKill)
        {
            if (QuestManager.Instance.enemyKilled == currentEvent.specificSpawn.gameObject.name)
                EndEvent(true);
        }

        if (currentEvent != null && killCounter >= currentEvent.killsRequired && !currentEvent.specificKill)
            EndEvent(true);
    }

    private void GiveItem(string itemName, int amount)
    {
        itemName = itemName.Replace("_", " ");
        Item tmp = null;
        bool firstLoop = true;

        for (int i = 0; i < amount; i++)
        {
            GameObject loadedItem = Instantiate(InventoryManager.Instance.itemObject);

            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Consumeables.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Equipment.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Weapons.Find(item => item.ItemName == itemName);
            }
            if (tmp == null)
            {
                tmp = InventoryManager.Instance.ItemContainer.Materials.Find(item => item.ItemName == itemName);
            }

            if (tmp != null)
            {
                loadedItem.AddComponent<ItemScript>();
                loadedItem.GetComponent<ItemScript>().Item = tmp;
                QuestManager.Instance.itemCollected = loadedItem.GetComponent<ItemScript>().Item.ItemName;
                if (!Player.Instance.inventorySelect.AddItem(loadedItem.GetComponent<ItemScript>(), false))
                    Player.Instance.inventory.AddItem(loadedItem.GetComponent<ItemScript>(), false);
                if (firstLoop)
                    ChatManager.Instance.AddChatLine(amount + " " + itemName + "(s) added to your inventory.");
                firstLoop = false;
            }
            else
                ChatManager.Instance.AddChatLine(itemName + " isn't a valid item.");
            Destroy(loadedItem);
        }
    }
}
