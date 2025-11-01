using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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
            Animal =  animal;
        }
        
        public abstract void OnUpdate();
        public abstract void OnExit();
    }

    public class Waiting : States
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
            
        }
    }

    public class LookingState : States
    {
        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
            var direction = (Animal.Target.transform.position - Animal.transform.position).normalized;
            Animal.transform.rotation = Quaternion.Slerp(Animal.transform.rotation, Quaternion.LookRotation(direction), Animal.rotateSpeed * Time.deltaTime);
            float dot = Vector3.Dot(Animal.transform.forward, direction);
            if (dot > 0.999988f)
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
                Animal.transform.position = Vector3.MoveTowards(Animal.transform.position, Animal.Target.transform.position, Time.deltaTime);
            }
            else
            {
                Animal.SwitchState(new Waiting());
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
                if (hunger <= 25)
                {
                    targetPicking = PickingTargetState.LookingForFood;
                }
                else if (thirst <= 25)
                {
                    targetPicking = PickingTargetState.LookingForWater;
                }
                else
                {
                    targetPicking = PickingTargetState.Wandering;
                }
                break;
            
            case PickingTargetState.LookingForFood:
                break;
            
            case PickingTargetState.LookingForWater:
                break;
        }
    }
    
    [SerializeField] private PickingTargetState pickingState;
    [Header("Movement")] 
    [SerializeField] float speed;
    
    [Header("Target Spawn Range")] 
    [SerializeField] float spawnRange;
    Vector3 _targetPos;

    [Header("Animals Stats")]
    [SerializeField] private float health = 100;
    [SerializeField] public float hunger = 100;
    [SerializeField] public float thirst = 100;
    [SerializeField] private float urgetobreed;
    [SerializeField] private float waitingDuration;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Transform _target;
    private States _state;
    public Transform Target
    {
        get =>  _target;
        private set => _target = value;
    }
    public float Health
    {
        get => health;
        set => health = value;
    }
    private Food food;
    
    void Start()
    {
        SwitchState(new Waiting());    
    }
    void Update()
    {
        if (_state != null)
        {
            _state.OnUpdate();
        }
        StatsUpdate();
    }
    void StatsUpdate()
    {
        hunger = hunger >= 0.00001 ? hunger -= 0.5f * Time.deltaTime : health;
        thirst = thirst >= 0.00001 ? thirst -= Time.deltaTime : thirst;
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
}

