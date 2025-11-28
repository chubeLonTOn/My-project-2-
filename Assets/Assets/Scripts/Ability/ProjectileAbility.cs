using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Ability/ProjectileAbility")]
public class ProjectileAbility : AbilityBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int numberOfProjectiles;

    public override void Activate(GameObject user)
    {
        for (int i = 0; i <= numberOfProjectiles; i++)
        {
            Instantiate(projectilePrefab, user.transform.position , user.transform.rotation);
        }
    }
}
