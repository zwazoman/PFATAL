using _scripts.PlayerCharacter;
using _scripts.PlayerCharacter.StateMachine.States;
using _Scripts.StateMachine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using state = PlayerCharacterNetworkStateMachineCallback.PlayerStateEnum;

public class PlayerHands : MonoBehaviour
{
    [SerializeField] PlayerCharacter _playerCharacter;

    public Hand leftHand;
    public Hand rightHand;

    [HideInInspector] public Hand[] hands = { null, null };

    private void Awake()
    {
        hands[0] = leftHand;
        hands[1] = rightHand;
    }

    private void Start()
    {
        _playerCharacter.stateMachine.OnStateChanged += StateChanged_Callback;
    }

    void StateChanged_Callback(StateBase<PlayerCharacter> previousState, StateBase<PlayerCharacter> nextState)
    {
        if (previousState is Pst_Dead && nextState is Pst_Alive)
        {
            print("equip random weapon");
            TryEquipRandomWeapon();
        }
        if (nextState is Pst_Alive && rightHand.equippedItem == null)
        {
            print("equip random weapon");
            TryEquipRandomWeapon();
        }
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

    public bool TryEquipRandomCons()
    {
        return TryEquipRandomItem("Cons");
    }

    public bool TryEquipRandomWeapon()
    {
        return TryEquipRandomItem("WP");
    }

    bool TryEquipRandomItem(string condition = "")
    {
        List<GameObject> prefabs = new();
        foreach (GameObject prefab in _playerCharacter.HandsItemVisuals.itemPrefabs)
        {
            if (prefab.name.Contains(condition))
                prefabs.Add(prefab);
        }

        ItemInfo info = new(ItemType.Weapon, prefabs.PickRandom());
        return TryEquipItem(info);
    }

    public void ClearHands()
    {
        foreach (Hand hand in hands)
        {
            hand.ClearInventory();
        }
    }

    #region Inputs

    public void UseRight(InputAction.CallbackContext ctx)
    {
        if (rightHand.equippedItem == null)
            return;

        if (ctx.started)
            rightHand.equippedItem.StartUsing();
        if (ctx.canceled && rightHand.equippedItem.isUsing)
            rightHand.equippedItem.StopUsing();
    }

    public void UseLeft(InputAction.CallbackContext ctx)
    {
        if (leftHand.equippedItem == null)
            return;

        if (ctx.started)
            leftHand.equippedItem.StartUsing();
        if (ctx.canceled && leftHand.equippedItem.isUsing)
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
