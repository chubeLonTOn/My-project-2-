using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class Animal : MonoBehaviour
{
    public abstract class States
    {
        protected Animal Animal;
        public abstract void OnEnter();
        /// <summary>
        /// Auto assign a current instance
        /// </summary>
        /// <param name="animal"></param>
        public void Initialize(Animal animal)
        {
            Animal = animal;
        }
        
        public abstract void OnUpdate();
        public abstract void OnExit();
    }

    public class WaitStage : States
    {
        private float timer;
        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= Animal.waitingDuration)
            {
                Animal.SwitchState(new LookingState());
            }
        }
        public override void OnExit()
        {
            Animal.PickingTarget();
        }
    }

    public class LookingState : States
    {
        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
            var direction = (Animal._target.transform.position - Animal.transform.position).normalized;
            Animal.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(Animal.transform.forward), Quaternion.LookRotation(direction), Animal.rotateSpeed * Time.deltaTime);
            float dot = Vector3.Dot(Animal.transform.forward, direction);
            if (dot >= 0.999988f)
            {
                Animal.SwitchState(new MovingState());
            }
        }

        public override void OnExit()
        {
            
        }
    }

    public class MovingState : States
    {
        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
            float distance = Vector3.Distance(Animal.transform.position , Animal.Target.transform.position);
            if (distance >= 0.00001f)
            {
                Animal.transform.position = Vector3.MoveTowards(Animal.transform.position, Animal.Target.transform.position, Animal.speed * Time.deltaTime);
            }
            else
            {
                Animal.SwitchState(new WaitStage());
            }
        }
        
        public override void OnExit()
        {
            
        }
    }
    
    public enum PickingTargetState
    {
        Wandering,
        LookingForFood,
        LookingForWater,
    }
    public PickingTargetState targetPicking;
    public void PickingTarget()
    {
        switch (targetPicking)
        {
            case PickingTargetState.Wandering:
                int spawnRange = Random.Range(5, 20);
                int counter = 0;
                Debug.Log($"Last Range Value : {lastRangeValue}");
                while (spawnRange == lastRangeValue && counter <= 10)
                {
                    counter++;
                    spawnRange = Random.Range(5, 20);
                        
                }
                lastRangeValue = spawnRange;
                Debug.Log($"Current Range : {spawnRange}");
                float randomX = Random.Range(transform.position.x - spawnRange, transform.position.x + spawnRange);
                float randomZ = Random.Range(transform.position.z - spawnRange, transform.position.z + spawnRange);
                var targetPos = new Vector3(randomX, transform.position.y, randomZ);
                Target.position = targetPos;
                if ( hunger <= 25 )
                {
                    targetPicking = PickingTargetState.LookingForFood;
                }
                break;
            
            case PickingTargetState.LookingForFood:
                Debug.LogError("This line does run");
                _food = FindObjectOfType<Food>()?.gameObject;
                Target.position = _food.transform.position;
                if (hunger >= 25)
                {
                    targetPicking = PickingTargetState.Wandering;
                }
                break;
        }
    }
    /// <summary>
    /// Assign States stuffs
    /// </summary>
    private States _state;
    private Food food;
    
    [Header("Movement")] 
    [SerializeField] float speed;
    [SerializeField] private float rotateSpeed;
    
    [Header("Animals Stats")]
    [SerializeField] private float health = 100;
    [SerializeField] public float hunger = 100;
    [SerializeField] public float thirst = 100;
    [SerializeField] private float urgetobreed;
    [SerializeField] private float waitingDuration;
    [SerializeField] private Transform _target;
    private GameObject _food;
    
    private int lastRangeValue;
    public Transform Target
    {
        get => _target; 
        set =>  _target = value;
    }

    public float Health
    {
        get => health;
        set => health = value;
    }
    
    void Start()
    {
        SwitchState(new WaitStage());    
    }
    void Update()
    {
        StatsUpdate();
        if (_state != null)
        {
            _state.OnUpdate();
        }
        
    }
    void StatsUpdate()
    {
        hunger = hunger >= 0.00001 ? hunger -= 0.5f * Time.deltaTime : health;
        //thirst = thirst >= 0.00001 ? thirst -= Time.deltaTime : thirst;
        urgetobreed = urgetobreed <= 99.99999f ?  urgetobreed += Time.deltaTime : urgetobreed;
    }
    void SwitchState(States states)
    {
        if (_state != null)
        {
            _state.OnExit();
        }
        _state = states;
        _state.Initialize(this);
        if (_state != null)
        {
            _state.OnEnter();
        }
        _state = states;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position , Target.transform.position);
    }
}

