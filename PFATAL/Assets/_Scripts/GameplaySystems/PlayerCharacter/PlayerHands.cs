using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHands : MonoBehaviour
{
    public Hand leftHand;
    public Hand rightHand;

    [HideInInspector] public Hand[] hands = { null, null };

    private void Awake()
    {
        hands[0] = leftHand;
        hands[1] = rightHand;
    }

    public bool TryEquipItem(ItemInfo itemInfo)
    {
        foreach(Hand hand in hands)
        {
            if(hand.type == itemInfo.itemType)
            {
                if (hand.TryPickupItem(itemInfo))
                    return true;
            }
        }
        return false;
    }

    #region Inputs

    public void UseRight(InputAction.CallbackContext ctx)
    {
        if (rightHand.equippedItem == null)
            return;

        if (ctx.started)
            rightHand.equippedItem.StartUsing();
        if (ctx.canceled)
            rightHand.equippedItem.StopUsing();
    }

    public void UseLeft(InputAction.CallbackContext ctx)
    {
        if (leftHand.equippedItem == null)
            return;

        if (ctx.started)
            leftHand.equippedItem.StartUsing();
        if (ctx.canceled)
            leftHand.equippedItem.StopUsing();
    }

    public void DropLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            leftHand.DropEquippedtem();
        }
    }

    public void SwitchEquip(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 value = ctx.ReadValue<Vector2>();
            if (value.y == 1)
                leftHand.ScrollEquippedItem(false);
            else if (value.y == -1)
                leftHand.ScrollEquippedItem(true);
        }
    }

    #endregion
}
