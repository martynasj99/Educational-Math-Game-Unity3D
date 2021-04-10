using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty : MonoBehaviour
{
    public enum GameDifficulty
    {
        HARD,
        MEDIUM,
        EASY
    }

    public static GameDifficulty difficulty;
}
