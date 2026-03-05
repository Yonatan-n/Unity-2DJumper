using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Animator))]
public class AchievementPopup : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    private Animator animator;
    private static readonly int ShowTrigger = Animator.StringToHash("Show");

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Show(AchievementDefinition def)
    {
        icon.sprite = def.icon;
        nameText.text = def.displayName;
        descriptionText.text = def.description;
        animator.ResetTrigger(ShowTrigger);
        animator.SetTrigger(ShowTrigger);
        AudioManager.Instance.AchievementUnlocked();
    }
}

