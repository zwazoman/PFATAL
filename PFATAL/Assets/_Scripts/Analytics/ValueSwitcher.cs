using _scripts.PlayerCharacter;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ValueSwitcher : NetworkBehaviour
{
    public GameObject cheatPanel;
    public PlayerCharacter playerCharacter;
    public List<PlayerCharacter> characters = new();

    [Header("Player")]
    public TMP_InputField hpText;
    public TMP_InputField movementSpeedText;
    public TMP_InputField lookSensitivityText;
    public TMP_InputField jumpForceText;
    public TMP_InputField gravityText;

    private void Awake()
    {
        //TryGetComponent(out playerCharacter);
    }

    void Start()
    {
        //await Task.Delay(500);

        /*foreach (GameObject playerObject in HeatMapServerAnalitics.instance.Players)
        {
            playerObject.TryGetComponent(out PlayerCharacter character);
            characters.Add(character);
        }*/
    }

    public void OnToggleCheatPanel(InputAction.CallbackContext context)
    {
        if (!playerCharacter.IsOwner) return;

        if (context.started)
        {

            Debug.Log("CONNARD");

            if (cheatPanel.activeSelf)
            {
                cheatPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                cheatPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }

        /*if (context.canceled)
        {
            
        }*/
    }

    public void Test()
    {
        Debug.Log("Test");
        SwitchHpRpc();
    }

    //Player
    //HP
    //Movement speed
    //Look sensitivity
    //Jump force
    //Gravity

    [Rpc(SendTo.Everyone)]
    public void SwitchHpRpc()
    {
        Debug.Log("Sala Switching Hp");
        
        foreach (PlayerCharacter character in characters)
        {
            character.health.SetMaxHP(float.Parse(hpText.text));
            Debug.LogError($"Hp switched to {float.Parse(hpText.text)}");
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchMovementSpeedRpc()
    {
        Debug.LogError("Switching Movement Speed");

        foreach (PlayerCharacter character in characters)
        {
            character.movement.globalMovespeedMultiplyer = float.Parse(movementSpeedText.text);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchLookSensitivityRpc()
    {
        Debug.LogError("Switching Look Sensitivity");
        foreach (PlayerCharacter character in characters)
        {
            character.TryGetComponent(out CharacterAiming aiming);

            aiming.Sensitivity = float.Parse(lookSensitivityText.text);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchJumpForceRpc()
    {
        Debug.LogError("Switching Jump Force");
        foreach (PlayerCharacter character in characters)
        {
            character.stateMachine.s_Jumping.ChangeJumpForce(float.Parse(jumpForceText.text));
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchGravityRpc()
    {
        Debug.LogError("Switching Gravity");
        foreach (PlayerCharacter character in characters)
        {
            character.physics.ChangeGravityStrenght(float.Parse(gravityText.text));
        }
    }

    //Hammer/Sword
    //Hammer/Sword damage
    //Hammer/Sword attack speed
    //Hammer/Sword range/hitbox size
    //Hammer/Sword charge time
    //Hammer/Sword dash force
    //Hammer/Sword dash reload time

    [Rpc(SendTo.Everyone)]
    public void SwitchHammerDamageRpc()
    {
        Debug.LogError("Switching Hammer Damage");
        //Hammer.ChangeDamage(10);
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchHammerAttackSpeedRpc()
    {
        Debug.LogError("Switching Hammer Attack Speed");
        //Hammer();
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchHammerRangeRpc()
    {
        Debug.LogError("Switching Hammer Range");
        //PlayerCharacter.SwitchHammerRange();
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchChargeTimeRpc()
    {
        Debug.LogError("Switching Hammer Charge Time");
        //PlayerCharacter.SwitchHammerChargeTime();
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchDashForceRpc()
    {
        Debug.LogError("Switching Hammer Dash Force");
        //PlayerCharacter.SwitchHammerDashForce();
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchDashReloadTimeRpc()
    {
        Debug.LogError("Switching Hammer Dash Reload Time");
        //PlayerCharacter.SwitchHammerDashReloadTime();
    }

    //Crossbow
    //Crossbow damage
    //Crossbow reload time
    //Crossbow projectile speed
    //Crossbow charge time

    [Rpc(SendTo.Everyone)]
    public void SwitchCrossbowDamageRpc()
    {
        Debug.LogError("Switching Crossbow Damage");
        //PlayerCharacter.SwitchCrossbowDamage();
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchCrossbowReloadTimeRpc()
    {
        Debug.LogError("Switching Crossbow Reload Time");
        //PlayerCharacter.SwitchCrossbowReloadTime();
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchProjectileSpeedRpc()
    {
        Debug.LogError("Switching Crossbow Projectile Speed");
        //PlayerCharacter.SwitchCrossbowProjectileSpeed();
    }

    [Rpc(SendTo.Everyone)]
    public void SwitchCrossbowChargeTimeRpc()
    {
        Debug.LogError("Switching Crossbow Charge Time");
        //PlayerCharacter.SwitchCrossbowChargeTime();
    }

    //Tomahawk
    //Tomahawk damage
    //Tomahawk attack speed
    //Tomahawk ammo
    //Tomahawk explosion radius
    //Tomahawk grab reload time
    //Tomahawk grab force

    //Bomb
    //Bomb damage
    //Bomb explosion radius
    //Bomb explosion delay

    //Tp
    //Tp range

    //Tornade
    //Tornade fly time
    //Tornade thrw force

    //Poison
    //Poison damage
    //Poison duration
    //Poison radius

    //Ground slam
    //Ground slam damage

    //Trap
    //Trap damage
    //Trap trigger radius
    //Trap duration


}
