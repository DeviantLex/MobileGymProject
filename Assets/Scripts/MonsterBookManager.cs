using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class MonsterBookManager : MonoBehaviour
{
    public GameObject monsterInfoPrefab;
    public MonsterSelection[] monsterSelections;
    public Transform monsterPanelParent;
    public Image monsterDescriptionImage;
    public TextMeshProUGUI monsterDescritionText;
    public PanelManager panelManager;
    public int desciptionMonsterPanelIndex;
    public int monsterButtonPanelIndex;

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

        Button monsterInfoButton = mButtonObj.transform.Find("EnemyInfoButton")?.GetComponent<Button>();
        if (monsterInfoButton != null) {
            monsterInfoButton.onClick.AddListener(() => ShowEnemyDesription(monster.MonsterDescription, monster.MonsterImage));
        }

    }

    void ShowEnemyDesription(string enemyDescrition, Sprite monsterSprite) {
        monsterDescritionText.text = enemyDescrition;
        monsterDescriptionImage.sprite = monsterSprite;
        panelManager.OpenPanel(desciptionMonsterPanelIndex);
        panelManager.ClosePanel(monsterButtonPanelIndex);
    }   
}
