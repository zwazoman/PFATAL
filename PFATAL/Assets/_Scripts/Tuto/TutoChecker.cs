using UnityEngine;

public class TutoChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Hand hand;
    [SerializeField] private TutoPanel crossbowPanel;
    [SerializeField] private TutoPanel tomahawkPanel;
    [SerializeField] private TutoPanel swordPanel;

    private void Start()
    {
        hand.OnEquipCrossbow += CheckCrossbow;
        hand.OnEquipTomahawk += CheckTomahawk;
        hand.OnEquipHammer += CheckSword;
    }

    private void OnDestroy()
    {
        hand.OnEquipCrossbow -= CheckCrossbow;
        hand.OnEquipTomahawk -= CheckTomahawk;
        hand.OnEquipHammer -= CheckSword;
    }

    void CheckCrossbow(Crossbow crossbow)
    {
        CheckAndShowTutorial(
            "TUTO_CROSSBOW",
            crossbowPanel
        );
    }

    void CheckTomahawk(Tomahawk tomahawk)
    {
        CheckAndShowTutorial(
            "TUTO_TOMAHAWK",
            tomahawkPanel
        );
    }

    void CheckSword(Sword sword)
    {
        CheckAndShowTutorial(
            "TUTO_SWORD",
            swordPanel
        );
    }

    void CheckAndShowTutorial(string key, TutoPanel panel)
    {
        if (PlayerPrefs.GetInt(key) == 3)
            return;

        int x = PlayerPrefs.GetInt(key);

        PlayerPrefs.SetInt(key, x + 1);
        PlayerPrefs.Save();

        panel.ShowPanel();
    }
}