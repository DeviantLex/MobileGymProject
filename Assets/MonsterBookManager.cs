using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterBookManager : MonoBehaviour
{
    public GameObject monsterInfoPrefab;
    public MonsterSelection[] monsterSelections;
    public Transform monsterPanelParent;
    void Start() {
        if(!monsterInfoPrefab || !monsterPanelParent) {
            Debug.LogError("Missing monster references! Assign them in the Inspector.");
            return;        
        }
        GenerateMonsterButton();
    }
    void GenerateMonsterButton() {
        if(monsterSelections == null || monsterSelections.Length == 0) { //In case the selection returns empty
             Debug.LogWarning("Exercise selection is empty!");
            return;
        }
        foreach(var monster in monsterSelections) {
            if(monster == null) continue;

            GameObject newMInfoButton = Instantiate(monsterInfoPrefab, monsterPanelParent);
            SetMInfoContent(newMInfoButton, monster);
        }
    }
    public void SetMInfoContent(GameObject mButtonObj, MonsterSelection monster) {
        Image monsterImage = mButtonObj.transform.Find("MonsterImage")?.GetComponent<Image>();
        if(monsterImage != null) monsterImage.sprite = monster.MonsterImage;
        
        TextMeshProUGUI monsterNAndDText = mButtonObj.transform.Find("MonsterN&D")?.GetComponent<TextMeshProUGUI>();
        if(monsterNAndDText != null) monsterNAndDText.text = monster.MonsterName;

        TextMeshProUGUI monsterReward = mButtonObj.transform.Find("MonsterReward")?.GetComponent<TextMeshProUGUI>();
        if(monsterReward != null) monsterReward.text = monster.MonsterReward;

    }
}
