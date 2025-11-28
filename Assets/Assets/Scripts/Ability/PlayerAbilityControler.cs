using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbilityControler : MonoBehaviour
{
    [SerializeField] private List<AbilityBase> abilities = new List<AbilityBase>();
    private Action<AbilityBase> _onAbilityCoolDown;

    private Dictionary<string, float> _coolDownList = new Dictionary<string, float>();
    
    private void TryActivateAbility(int index)
    {
        
    }
}
