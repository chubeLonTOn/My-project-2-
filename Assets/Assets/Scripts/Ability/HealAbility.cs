using UnityEngine;

[CreateAssetMenu(menuName = "Ability/HealAbility")]
public class HealAbility : AbilityBase
{
    [SerializeField] private float _healingNumber;
    
    private PlayerAbilityControler _player;
    
    public override void Activate(GameObject user)
    {
        
    }
}
