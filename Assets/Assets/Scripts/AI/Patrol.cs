using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class AIMovement : MonoBehaviour
{
    
    public UnityEvent<int> onReachTarget;
    
    [SerializeField] private List<Transform> target;
    public Transform Target { get; private set; }
    private int currentTarget = 0;
    
    [SerializeField] private float speedFactor;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float pauseDuration;
    private float _waitingTime;
    public bool isTheSame { get; private set; } = false;
    public bool isPressed { get; set; } = false;
    private bool PingPongForward = true;
    public enum AIMode
    {
        Loop,
        PingPong,
        Random
    }
    [SerializeField] private AIMode mode;
    public enum States
    {
        Waiting,
        LookAtTheNextWaypoint,
        MoveToTheWaypoint
    }
    [SerializeField] private States states;
    
    private void Update()
    {
        Behavior();
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
    /// <summary>
    /// <para>if dot = 1 it facing the same direction</para>
    /// <para>if dot = 0 it facing the 90 angle of the direction</para>
    /// <para>if dot = -1 it's the opposite</para>
    /// </summary>
    private void Behavior()
    {
        switch (states)
        {
            case States.Waiting:
                _waitingTime += Time.deltaTime;
                if (_waitingTime >= pauseDuration)
                {
                    _waitingTime = 0;
                    PickNextTarget();
                    states = States.LookAtTheNextWaypoint;
                }
                break;
            
            case States.LookAtTheNextWaypoint:
                float dot = Vector3.Dot(transform.forward, (Target.transform.position - transform.position).normalized);
                if (dot < 0.9999999)
                {
                    Debug.Log($"Current dot:{dot}");
                    Turning();
                }
                else 
                {
                    states = States.MoveToTheWaypoint;
                }
                break;
            
            case  States.MoveToTheWaypoint:
                float distance = Vector3.Distance(transform.position, Target.transform.position);
                if (distance > 0.001f)
                {
                    Movement();
                }
                else
                {
                    onReachTarget?.Invoke(currentTarget);
                    states = States.Waiting;
                }
                break;
        }
    }
    void Movement()
    {
            var pos = Target.transform.position;
            transform.position = Vector3.MoveTowards(transform.position , pos , speedFactor * Time.deltaTime);
    }
    void Turning()
    {
        var dir = (Target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(transform.forward), Quaternion.LookRotation(dir), rotateSpeed * Time.deltaTime);
    }
    void OnDrawGizmos()
    {
        if (isPressed)
        {
            for (int i = 0; i < target.Count - 1; i++)
            {
                Vector3 currentPos = target[i].position;
                Vector3 nextPos = target[i + 1].position;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(currentPos , nextPos);
                Gizmos.DrawSphere(currentPos, 0.5f);
                Gizmos.DrawLine(target[^1].position, target[0].position);
                Gizmos.DrawSphere(target[^1].position, 0.5f);
            }
        }
        else
        {
            return;
        }
    }
    
}

