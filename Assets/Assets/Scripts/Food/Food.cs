using UnityEngine;

namespace Assets.Scripts.Food
{
    public class Food : MonoBehaviour
    {
        [SerializeField] private float filling = 50;
        public float Filling => filling;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Rabbit"))
            {
                Destroy(gameObject);
            }
        }
    }
}
