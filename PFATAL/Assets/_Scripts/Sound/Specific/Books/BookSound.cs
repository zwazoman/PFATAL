using UnityEngine;

public class BookSound<T> : ItemSound<T> where T : Cons_BookBase
{
    protected override void LinkEvents()
    {
        base.LinkEvents();
        main.OnStopUsing += PlayCastSound;
    }

    protected override void EquipLink()
    {
        base.EquipLink();
        //main.hand.animatorEventListener.OnAnimationFinished += PlayCloseSound;
    }

    protected override void UnEquipLink()
    {
        base.UnEquipLink();
        //main.hand.animatorEventListener.OnAnimationFinished -= PlayCloseSound;
    }

    void PlayCastSound() => AudioManager.Instance.PlayOneShot(Sounds.BookSpellCast);

    void PlayCloseSound() => AudioManager.Instance.PlayOneShot(Sounds.BookClose);
}
