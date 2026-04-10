using TMPro;
using UnityEngine;

namespace NetworkTime
{
    public class TimestampDebugger : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        void Update()
        {
            text.text = $"Local Now : {TimeStamp.LocalNow.ToString("0.00")}, " +
                        $"correction : {TimeStamp.Correction.ToString("0.00")}, " +
                        $"synced now : {TimeStamp.Now.ToString("0.00")}";
        }
    }
}