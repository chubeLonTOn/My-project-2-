using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Ability/AbilityBase")]
public abstract class AbilityBase : ScriptableObject
{
    [SerializeField] private float coolDown;

    public UnityEvent onAbilityUsed; ///Used when broadcasting the Ability is used and on cooldown
    public UnityEvent obAbilityReady; ///Used when broadcasting the Ability is ready
    public UnityEvent onAbilityActivated; ///Used when broadcasting the Ability is currently activated
    public abstract void Activate(GameObject user);
    
}
