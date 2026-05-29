using UnityEngine;

public class TutoChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Hand hand;
    [SerializeField] private TutoPanel crossbowPanel;
    [SerializeField] private TutoPanel tomahawkPanel;
    [SerializeField] private TutoPanel hammerPanel;

    private void Start()
    {
        hand.OnEquipCrossbow += CheckCrossbow;
        hand.OnEquipTomahawk += CheckTomahawk;
        hand.OnEquipHammer += CheckHammer;
    }

    private void OnDestroy()
    {
        hand.OnEquipCrossbow -= CheckCrossbow;
        hand.OnEquipTomahawk -= CheckTomahawk;
        hand.OnEquipHammer -= CheckHammer;
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

    void CheckHammer(Sword hammer)
    {
        CheckAndShowTutorial(
            "TUTO_HAMMER",
            hammerPanel
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