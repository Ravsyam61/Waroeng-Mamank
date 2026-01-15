using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    public static class GameProgress
    {
        public static bool cookingMinigameFinished = false;

        public static int currentStage = 1; // default mulai dari stage 1


        public static bool stage1Unlocked = true;  // default terbuka
        public static bool stage2Unlocked = false;
        public static bool stage3Unlocked = false;

        public static bool stage1Completed = false;
        public static bool stage2Completed = false;

        public static void ResetProgress()
        {
            cookingMinigameFinished = false;
            stage1Unlocked = true;
            stage2Unlocked = false;
            stage3Unlocked = false;
            stage1Completed = false;
            stage2Completed = false;
        }
    }

}
