using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GroupName { Alpha, Bravo, Charlie, Delta, Echo, Foxtrot, Golf, Hotel }

public class DynamicArmyManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject botPrefab;
    public int botAmount;
    public float spawnRadius = 3.0f;

    [Tooltip("List of locations where squads will spawn. The system will cycle through these.")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Squad Settings")]
    [Range(1, 10)]
    public int botsPerSquad = 4;

    private List<BotAgent> allBots = new List<BotAgent>();
    private Dictionary<GroupName, List<BotAgent>> groups = new Dictionary<GroupName, List<BotAgent>>();

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points assigned to DynamicArmyManager!");
            return;
        }
        else
        {
            InitializeArmy(botAmount);
        }
    }

    private void InitializeArmy(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Spawn far away or disabled initially if needed, but here we'll let AssignToGroups move them
            GameObject obj = Instantiate(botPrefab, Vector3.zero, Quaternion.identity);
            BotAgent bot = obj.GetComponent<BotAgent>();
            bot.botID = i;
            allBots.Add(bot);
        }

        AssignToGroups(allBots, new List<GroupName> { GroupName.Alpha, GroupName.Bravo, GroupName.Charlie, GroupName.Delta });
    }

    /// <summary>
    /// Overhauled logic: Distributes bots into groups and squads.
    /// Each squad is assigned a single spawn point, but every bot within that squad 
    /// receives its own unique random offset.
    /// </summary>
    public void AssignToGroups(List<BotAgent> botsToAssign, List<GroupName> targetGroups)
    {
        foreach (var group in groups.Values) group.Clear();

        int botsPerGroup = botsToAssign.Count / targetGroups.Count;
        int currentSpawnIndex = 0;

        for (int g = 0; g < targetGroups.Count; g++)
        {
            GroupName groupName = targetGroups[g];
            if (!groups.ContainsKey(groupName)) groups[groupName] = new List<BotAgent>();

            var groupBots = botsToAssign.Skip(g * botsPerGroup).Take(botsPerGroup).ToList();
            if (g == targetGroups.Count - 1) groupBots = botsToAssign.Skip(g * botsPerGroup).ToList();

            for (int i = 0; i < groupBots.Count; i++)
            {
                BotAgent bot = groupBots[i];
                bot.currentGroup = groupName;

                // Determine squad ID based on flexible botsPerSquad setting
                int squadID = i / botsPerSquad;
                bot.squadID = squadID;

                groups[groupName].Add(bot);
                bot.gameObject.name = $"Bot_{bot.currentGroup}_S{bot.squadID}_ID{bot.botID}";

                // SQUAD POSITIONING LOGIC:
                // Check if this is the start of a new squad
                if (i % botsPerSquad == 0)
                {
                    // Pick the next spawn point in the cycle
                    currentSpawnIndex = currentSpawnIndex % spawnPoints.Count;
                }

                // INDIVIDUAL RANDOMIZATION:
                // Every bot gets a unique offset regardless of squad
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                randomOffset.y = 0; // Keep on flat ground

                // Set position: Base Squad Point + Individual Random Offset
                bot.transform.position = spawnPoints[currentSpawnIndex].position + randomOffset;

                // Move to next spawn point only after the FULL squad is processed
                if ((i + 1) % botsPerSquad == 0 || i == groupBots.Count - 1)
                {
                    currentSpawnIndex++;
                }
            }
        }
        Debug.Log($"Assigned {botsToAssign.Count} bots into {targetGroups.Count} groups with {botsPerSquad} bots per squad.");
    }

    public void CommandGroup(GroupName groupName, Vector3 destination)
    {
        if (groups.ContainsKey(groupName))
        {
            foreach (var bot in groups[groupName]) bot.MoveTo(destination);
        }
    }

    public void CommandSquad(GroupName groupName, int squadID, Vector3 destination)
    {
        if (groups.ContainsKey(groupName))
        {
            var squadBots = groups[groupName].Where(b => b.squadID == squadID);
            foreach (var bot in squadBots) bot.MoveTo(destination);
        }
    }

    [ContextMenu("Expand Groups")]
    public void ExpandGroups()
    {
        var targetGroups = new List<GroupName> {
            GroupName.Alpha, GroupName.Bravo, GroupName.Charlie,
            GroupName.Delta, GroupName.Echo, GroupName.Foxtrot, GroupName.Golf
        };
        AssignToGroups(allBots, targetGroups);
    }
}