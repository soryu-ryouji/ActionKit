using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionKit
{
    public void Play(ActionContainer container)
    {
        container.Play();
    }
}

public class ActionKitUpdateTrigger : MonoBehaviour
{
    public System.Action OnUpdate = () => { };

    void Update()
    {
        OnUpdate();
    }
}

public class ActionPlayer
{
    public void RegisterUpdate(System.Action onUpdate)
    {
        var trigger = new GameObject(nameof(ActionKitUpdateTrigger)).AddComponent<ActionKitUpdateTrigger>();
        trigger.OnUpdate += onUpdate;
    }

    public void Play(BaseAction action)
    {
        action.Execute();
    }
}

public class BaseAction
{
    public virtual void Execute()
    {
    }
}

public class CallbackAction : BaseAction
{
    private System.Action m_Callback;

    public CallbackAction(System.Action callback)
    {
        m_Callback = callback;
    }

    public override void Execute()
    {
        m_Callback();
    }
}

public abstract class ActionContainer
{
    public abstract void Play();

    public abstract void Update();
}

public class Sequence : ActionContainer
{
    private List<BaseAction> m_Actions = new();
    private ActionPlayer m_Player;

    public override void Play()
    {
        m_Player = new ActionPlayer();
        m_Player.RegisterUpdate(Update);
    }

    public override void Update()
    {
        if (m_Actions.Any())
        {
            var action = m_Actions.First();
            m_Player.Play(action);
            m_Actions.RemoveAt(0);
        }
    }

    public void AddAction(BaseAction action)
    {
        m_Actions.Add(action);
    }
}

public class Spawn : ActionContainer
{
    private List<BaseAction> m_Actions = new();


    private ActionPlayer m_Player = null;

    public override void Play()
    {
        m_Player = new ActionPlayer();

        m_Player.RegisterUpdate(Update);
    }

    public override void Update()
    {
        foreach (var lxAction in m_Actions)
        {
            m_Player.Play(lxAction);
        }

        m_Actions.Clear();
    }

    public void AddAction(BaseAction action)
    {
        m_Actions.Add(action);
    }
}

public class Timeline : ActionContainer
{
    private List<Timepoint> m_Timepoints = new List<Timepoint>();

    private ActionPlayer m_Player = null;

    private float m_StartSeconds = 0;

    public override void Play()
    {
        m_Player = new ActionPlayer();
        m_Player.RegisterUpdate(Update);
        m_StartSeconds = Time.time;
    }

    public override void Update()
    {
        var currentSeconds = Time.time - m_StartSeconds;

        var availabelTimepoints = m_Timepoints.Where(item => item.StartSeconds < currentSeconds).ToArray();

        foreach (var timepoint in availabelTimepoints)
        {
            m_Player.Play(timepoint.Action);
        }

        foreach (var availabelTimepoint in availabelTimepoints)
        {
            m_Timepoints.Remove(availabelTimepoint);
        }
    }

    public void AddAction(float startSeconds, BaseAction action)
    {
        m_Timepoints.Add(new Timepoint()
        {
            StartSeconds = startSeconds,
            Action = action
        });
    }
}
public class Timepoint
{
    public float StartSeconds;

    public BaseAction Action;
}

public class Timer
{
    public float CurrentSeconds;

    private float m_StartSeconds;

    public void Start()
    {
        m_StartSeconds = Time.time;
    }

    public void CalculateTimeData()
    {
        CurrentSeconds = Time.time - m_StartSeconds;
    }
}