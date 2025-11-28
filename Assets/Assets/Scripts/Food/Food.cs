using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Food : MonoBehaviour
{
    [SerializeField] private float filling = 50;
    [FormerlySerializedAs("_animal")] [SerializeField] private Rabbit animal;
    public float Filling => filling;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rabbit"))
        {
            animal.Eating(this);
            StartCoroutine(DestroyObject());
        }
    }
    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

