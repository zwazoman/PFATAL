using System.Collections;
using UnityEngine;

public class TutoChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Hand hand;
    [SerializeField] private TutoPanel crossbowPanel;
    [SerializeField] private TutoPanel tomahawkPanel;
    [SerializeField] private TutoPanel swordPanel;

    private bool swordsUsed = false;
    private bool tomahawksUsed = false;
    private bool crossbowsUsed = false;

    /*private void Awake()
    {
        PlayerPrefs.DeleteKey("TUTO_CROSSBOW");
        PlayerPrefs.DeleteKey("TUTO_TOMAHAWK");
        PlayerPrefs.DeleteKey("TUTO_SWORD");
    }*/

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
        if (crossbowsUsed)
            return;

        CheckAndShowTutorial(
            "TUTO_CROSSBOW",
            crossbowPanel
        );

        crossbowsUsed = true;
    }

    void CheckTomahawk(Tomahawk tomahawk)
    {
        if (tomahawksUsed)
            return;

        CheckAndShowTutorial(
            "TUTO_TOMAHAWK",
            tomahawkPanel
        );

        tomahawksUsed = true;
    }

    void CheckSword(Sword sword)
    {
        if (swordsUsed)
            return;

        CheckAndShowTutorial(
            "TUTO_SWORD",
            swordPanel
        );

        swordsUsed = true;
    }

    void CheckAndShowTutorial(string key, TutoPanel panel)
    {
        if (PlayerPrefs.GetInt(key) == 3)
            return;

        int x = PlayerPrefs.GetInt(key);

        PlayerPrefs.SetInt(key, x + 1);
        PlayerPrefs.Save();

        panel.ShowPanel();
        StartCoroutine(WaitAndClosePanel(panel));
    }

    IEnumerator WaitAndClosePanel(TutoPanel panel)
    {
        yield return new WaitForSeconds(5f);
        panel.ClosePanel();
    }
}