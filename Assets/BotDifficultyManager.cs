using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDifficultyManager : MonoBehaviour
{
    [SerializeField] Bot bot;
    [SerializeField] int selectedDifficulty;
    [SerializeField] BotStats[] botDifficulties;

    private void Start()
    {
        var newStats = botDifficulties[selectedDifficulty];
        bot.SetStats(newStats);
    }
}
