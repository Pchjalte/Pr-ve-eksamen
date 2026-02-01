using UnityEngine;
using Photon.Pun;

public abstract class AbilityBase : MonoBehaviourPun {

    protected bool isReady = true;

    public virtual void Initialize() { }
    public virtual void OnAbilityPressed() { }
}