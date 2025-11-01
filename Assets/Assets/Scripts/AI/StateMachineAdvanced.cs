using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateMachineAdvanced : MonoBehaviour
{
    public abstract class State
    {
        protected StateMachineAdvanced _stateMachine;
        public abstract void OnEnter();
        public void Initialize(StateMachineAdvanced stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public abstract void OnUpdate();
        public abstract void OnExit();
    }
    public class WaitState : State
    {
        private float timer;
        
        public override void OnEnter()
        {
            
        }
        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= _stateMachine.WaitTime)
            {
                LookingState lookingState = new LookingState();
                _stateMachine.SwitchState(lookingState);
            }
        }
        public override void OnExit()
        {
            _stateMachine.PickNextTarget();
        }
    }
    public class MoveState : State
    {
        public override void OnEnter()
        {
            
        }
        /// <summary>
        /// <para></para>
        /// </summary>
        public override void OnUpdate()
        {
            float distance = Vector3.Distance(_stateMachine.transform.position, _stateMachine.Target.transform.position);
            if (distance > 0.001f)
            {
                var pos = _stateMachine.Target.transform.position;
                _stateMachine.transform.position = Vector3.MoveTowards(_stateMachine.transform.position , pos , _stateMachine.SpeedFactor * Time.deltaTime);
            }
            else
            {
                _stateMachine.onReachEvent?.Invoke(_stateMachine.CurrentTarget);
                _stateMachine.SwitchState(new WaitState());
            }
        }

        public override void OnExit()
        {
            
        }
    }
    public class LookingState : State
    {
        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
            var dir = (_stateMachine.Target.transform.position - _stateMachine.transform.position).normalized;
            _stateMachine.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(_stateMachine.transform.forward), Quaternion.LookRotation(dir), _stateMachine.RotateSpeed * Time.deltaTime);
            float dot = Vector3.Dot(_stateMachine.transform.forward, dir);
            if (dot >= 0.999999f)
            {
                _stateMachine.SwitchState(new MoveState());
            }
        }

        public override void OnExit()
        {
            
        }
    }
    private State _state;
    
    [SerializeField] private List<Transform> target;
    public Transform Target { get; private set; }
    private int currentTarget = 0;
    public int CurrentTarget => currentTarget;
    [SerializeField] private float speedFactor;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float pauseDuration;
    private bool PingPongForward = true;
    
    [SerializeField] private UnityEvent<int> onReachEvent;
    
    public enum AIMode
    {
        Loop,
        PingPong,
        Random
    }
    [SerializeField] private AIMode mode;
    public float WaitTime => pauseDuration; 
    public float SpeedFactor => speedFactor;
    public float RotateSpeed => rotateSpeed;
    void Start()
    {
        SwitchState(new WaitState());
    }
    private void PickNextTarget()
    {
        switch (mode)
        {
            case AIMode.Loop:
                currentTarget = (currentTarget + 1) % (target.Count);
                Target = target[currentTarget];
                Debug.Log($"Current Target : {currentTarget}");
                break;
            
            case AIMode.PingPong:
                if (PingPongForward)
                {
                    currentTarget++;
                    Target = target[currentTarget];
                    if (currentTarget == target.Count - 1)
                    {
                        PingPongForward = false;
                    }
                }
                else
                {
                    currentTarget--;
                    Target = target[currentTarget];
                    if (currentTarget == 0)
                    {
                        PingPongForward = true;
                    }
                }
                break;
            
            case AIMode.Random:
                int randomIndex = Random.Range(0, target.Count);
                int count = 0;
                while (randomIndex == currentTarget && count < 10)
                {
                    count++;
                    randomIndex = Random.Range(0, target.Count);
                }
                currentTarget = randomIndex;
                Target = target[currentTarget];
                break;
        }
    }
    void Update()
    {
        if (_state != null)
        {
            _state.OnUpdate();
        }
    }

    void SwitchState(State state)
    {
        if (_state != null)
        {
            _state.OnExit();
        }
        _state = state;
        _state.Initialize(this);
        
        if (_state != null)
        {
            _state.OnEnter();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < target.Count; i++)
        {
            Gizmos.DrawSphere(target[i].position, 0.5f);
            if (i > 0)
            {
                Gizmos.DrawLine(target[i - 1].position, target[i].position);
            }
        }
        Gizmos.DrawLine(target[^1].position, target[0].position);
    }
}
