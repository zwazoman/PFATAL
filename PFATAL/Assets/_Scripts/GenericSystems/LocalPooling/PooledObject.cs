using System.Collections;
using System.Collections.Generic;
using _Scripts.Pooling;
using UnityEngine;

/// <summary>
/// ce component permet � un objet de se souvenir de la pool de laquelle il vient, ainsi que de son index dedans.
/// il a �galement une m�thode pour d�sactiver l'objet et le renvoyer dans la pool.
/// </summary>
public class PooledObject : MonoBehaviour
{
    public static implicit operator GameObject(PooledObject o) => o.gameObject;
    
    //ces variables sont g�r�es automatiquement par la pool � laquelle appartient l'objet.
    public bool IsInPool = true;
    public int Index;
    public Pool Pool;

    List<Behaviour> _enabledBehavioursAtStart = new();

    //r�active tous les components activ�s de base et inversement
    private void OnInstantiatedByPool()
    {
        foreach (Behaviour comp in GetComponents<Behaviour>())
        {
            if(comp.enabled) _enabledBehavioursAtStart.Add(comp);
        }
    }
    void OnPulledFromPool()
    {
        foreach (Behaviour comp in GetComponents<Behaviour>())
        {
            comp.enabled = _enabledBehavioursAtStart.Contains(comp);
        }
    }

    /// <summary>
    /// � utiliser � la place de Destroy(gameObject)
    /// </summary>
    public void GoBackIntoPool()
    {
        if(!IsInPool) Pool.PutObjectBackInPool(this);
    }
    public void GoBackIntoPool_Delayed(float delay)
    {
        StartCoroutine(C_GoBackIntoPool_Delayed(delay));
    }

    IEnumerator C_GoBackIntoPool_Delayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Application.isPlaying)
            if (!IsInPool) Pool.PutObjectBackInPool(this);
    }

}
