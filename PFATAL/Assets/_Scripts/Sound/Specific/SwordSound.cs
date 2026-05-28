using Unity.VisualScripting;
using UnityEngine;

public class SwordSound : ItemSound<Sword>
{
    bool attack1;

    protected override void EquipLink()
    {
        base.EquipLink();

        main.hand.animatorEventListener.OnSwordHitboxActivated += PlayAttackSound;
        main.OnDashStarted += PLayDashSound;
        main.OnSmallAttackStarted += PlayAnticipationSound;
        main.OnStartCharging += PlayChargeSound;
    }

    protected override void UnEquipLink()
    {
        base.UnEquipLink();

        main.hand.animatorEventListener.OnSwordHitboxActivated -= PlayAttackSound;
        main.OnDashStarted -= PLayDashSound;
        main.OnSmallAttackStarted -= PlayAnticipationSound;
        main.OnStartCharging -= PlayChargeSound;
    }

    void PlayAttackSound()
    {
        if (main.isDashing)
        {
            return;
        }

        int whooshValue;

        if (attack1)
            whooshValue = 0;
        else
            whooshValue = 1;

        AudioManager.Instance.PlayOnlineOneShots(Sounds.SwordWhoosh, Sounds.SwordWhoosh3D, transform.position, "SwordWhooshes", whooshValue);


        attack1 = !attack1;
    }
    void PlayAnticipationSound() { if (!main.isDashing) AudioManager.Instance.PlayOneShot(Sounds.SwordAnticipation); }
    void PLayDashSound() => AudioManager.Instance.PlayOneShot(Sounds.SwordDash);

    void PlayChargeSound() => AudioManager.Instance.PlayOneShot(Sounds.SwordCharge);
}
