using UnityEngine;

public class Cons_WolfTrap : Cons_ThrowableBase
{
    [SerializeField] GameObject _wolfTrapPrefab;
    [SerializeField] private Renderer _objectRenderer;

    private void Start()
    {
        Scrollable();
    }

    public override void StartUsing()
    {
        if (_throwAnimationIsPlaying) return;

        base.StartUsing();
    }

    public override void StopUsing()
    {
        base.StopUsing();

        StartThrowAnimation();
    }

    protected override void ThrowObject()
    {
        SpawnContext context = new(playerCharacter.OwnerClientId);
        context.floatData2 = ItemID;

        _ = Summoner.Instance.SpawnObject(_wolfTrapPrefab, hand._itemSocket.position,
            playerCharacter.transform.rotation, false, context);

        //BreakItem();
    }

    public override void Equip()
    {
        base.Equip();
        _objectRenderer.enabled = true;
        hand.animatorEventListener.OnObjectThrown += HideObject;
    }
    
    public override void UnEquip()
    {
        base.UnEquip();
        hand.animatorEventListener.OnObjectThrown -= HideObject;
    }
    
    public void HideObject()
    {
        _objectRenderer.enabled = false;
    }
}
