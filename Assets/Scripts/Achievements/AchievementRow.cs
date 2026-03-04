using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AchievementRow : MonoBehaviour
{
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Color lockedTint = new Color(0.4f, 0.4f, 0.4f);

    public void Populate(AchievementDefinition def, AchievementData data)
    {
        nameText.text = def.displayName;
        descriptionText.text = def.description;

        icon.sprite = def.icon == null ? defaultIcon : def.icon;
        icon.color = data.isUnlocked ? Color.white : lockedTint;

        dateText.text = data.isUnlocked ? $"Unlocked {data.unlockDate}" : "";
        dateText.gameObject.SetActive(data.isUnlocked);

        bool showProgress = def.IsProgressBased && !data.isUnlocked;
        progressBar.gameObject.SetActive(showProgress);
        progressText.gameObject.SetActive(showProgress);
        if (showProgress)
        {
            progressBar.value = (float)data.progress / def.progressTarget;
            progressText.text = $"{data.progress} / {def.progressTarget}";
        }
    }
}

