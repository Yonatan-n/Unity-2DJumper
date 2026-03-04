using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchievementScreen : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private AchievementRow rowPrefab;
    [SerializeField] private Button backToMenu;

    private void Start() => StartCoroutine(PopulateNextFrame());

    private IEnumerator PopulateNextFrame()
    {
        yield return null; // wait one frame for AchievementManager.Awake to finish
        backToMenu.onClick.AddListener(BackToMenu);
        Populate();
    }

    void BackToMenu()
    {
        SceneLoader.Instance.LoadSceneByName("MainMenu");
    }

    private void Populate()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var def in AchievementManager.Instance.GetAllDefinitions())
        {
            var data = AchievementManager.Instance.GetData(def.id);
            Debug.Log("Row: " + (def == null ? "DEF IS NULL" : def.id) + " | data: " + (data == null ? "DATA IS NULL" : "ok"));
            var row = Instantiate(rowPrefab, content);
            row.Populate(def, data);
        }
    }
}