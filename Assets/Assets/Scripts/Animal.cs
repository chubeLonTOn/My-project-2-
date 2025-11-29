using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
        private float _timer;
        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer >= Animal.waitingDuration)
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
                int spawnRange = Random.Range(15, 25);
                int counter = 0;
                while (spawnRange == lastRangeValue && counter <= 10)
                {
                    counter++;
                    spawnRange = Random.Range(15, 25);
                        
                }
                lastRangeValue = spawnRange;
                float randomX = Random.Range(-spawnRange, spawnRange) + transform.position.x;
                float randomZ = Random.Range(-spawnRange, spawnRange) + transform.position.z;
                var targetPos = new Vector3(randomX, transform.position.y, randomZ);
                Target.position = targetPos;
                if ( Hunger <= 25 )
                {
                    targetPicking = PickingTargetState.LookingForFood;
                }
                break;
            
            case PickingTargetState.LookingForFood:
                FindFood();
                break;
        }
    }
    
    private States _state;
    
    [Header("Movement")] 
    [SerializeField] float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float jumpValue;
    [SerializeField] private AnimationCurve jumpFactor;
    
    [Header("Animals Stats")]
    [SerializeField] private float health = 100;
    [SerializeField] public float Hunger = 100;
    [SerializeField] private float _hungerFindFoodPoint = 25;
    [SerializeField] public float thirst = 100;
    [SerializeField] private float urgetobreed;
    [SerializeField] private float waitingDuration;
    [SerializeField] private Transform _target;
    
    private List<Food> _lstfood = new  List<Food>();
    private GameObject _food;
    
    //public UnityEvent onReachEvent;
    
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
        Hunger = Hunger >= 0.00001 ? Hunger -= 0.5f * Time.deltaTime : Hunger = 0;
        //thirst = thirst >= 0.00001 ? thirst -= Time.deltaTime : thirst;
        urgetobreed = urgetobreed <= 99.99999f ?  urgetobreed += Time.deltaTime : urgetobreed = 100;
        if (Hunger == 0)
        {
            Health = Health >= 0.00001 ? Health -= 0.5f * Time.deltaTime : Health = 0;
        } else 
        {
            Health = Health <= 99.99999f ?  Health += Time.deltaTime : Health = 100;
        }
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
    
    private void FindFood()
    {
        GameObject foodSource = FindAnyObjectByType<Food>()?.gameObject;
        _food = foodSource;
        if (_food != null) Target.position = _food.transform.position;
        Debug.LogError("Cant found food source");
        int count = 0;
        while (Hunger >= _hungerFindFoodPoint || count <= 5)
        {
            count++;
            targetPicking = PickingTargetState.Wandering;
        }
        count = 0;
    }
    
    public void Eating(Food food)
    {
        if (!food) return;
        Hunger += food.Filling;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_target.transform.position, 0.1f);
        Gizmos.DrawLine(transform.position , Target.transform.position);
    }
}

