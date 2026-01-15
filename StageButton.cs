using UnityEngine;
using UnityEngine.UI;

public class StageButtonManager : MonoBehaviour
{
    public Button stage1Button;
    public Button stage2Button;
    public Button stage3Button;

    public Image stage2LockImage;
    public Image stage3LockImage;

    void Start()
    {
        stage1Button.interactable = LevelProgress.GameProgress.stage1Unlocked;
        stage2Button.interactable = LevelProgress.GameProgress.stage2Unlocked;
        stage3Button.interactable = LevelProgress.GameProgress.stage3Unlocked;

        stage2LockImage.gameObject.SetActive(!LevelProgress.GameProgress.stage2Unlocked);
        stage3LockImage.gameObject.SetActive(!LevelProgress.GameProgress.stage3Unlocked);
    }
}
