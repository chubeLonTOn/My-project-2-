using UnityEngine;

[CreateAssetMenu(menuName = "Ability/DashAbility")]
public class DashAbility : AbilityBase
{
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashSpeed;
    
    public override void Activate(GameObject user)
    {
        
    }
}

