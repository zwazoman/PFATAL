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
    }

    protected override void UnEquipLink()
    {
        base.UnEquipLink();

        main.hand.animatorEventListener.OnSwordHitboxActivated -= PlayAttackSound;
        main.OnDashStarted -= PLayDashSound;
        main.OnSmallAttackStarted -= PlayAnticipationSound;
    }

    void PlayAttackSound()
    {
        if (main.isDashing)
        {
            return;
        }

        if (attack1)
            AudioManager.Instance.PlayOneShot(Sounds.SwordWhoosh);
        else
            AudioManager.Instance.PlayOneShot(Sounds.SwordWhoosh2);

        attack1 = !attack1;
    }
    void PlayAnticipationSound() { if (!main.isDashing) AudioManager.Instance.PlayOneShot(Sounds.SwordAnticipation); }
    void PLayDashSound() => AudioManager.Instance.PlayOneShot(Sounds.SwordDash);
}
