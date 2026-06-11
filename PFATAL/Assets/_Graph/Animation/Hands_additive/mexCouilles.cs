using UnityEditor;
using UnityEngine;

public class mexCouilles : MonoBehaviour
{
    public AnimationClip clip;
    public AnimationClip refClip;
    public float time;
    [ContextMenu("ahhh")]
    void Ahhhh()
    {
        AnimationUtility.SetAdditiveReferencePose(clip, refClip, time);
        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssetIfDirty(clip);
    }
}
