using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionKit
{
    private ActionPlayer m_ActionPlayer = new();

    private Calendar m_Calendar = new();

    private Timer m_Timer = new();

    public void AddAction(float beginSeconds, BaseAction action)
    {
        m_Calendar.Record(new CalendarItem()
        {
            StartSeconds = beginSeconds,
            Action = action
        });
    }

    public void Play()
    {
        m_Timer.Start();
        m_Timer.CalculateTimeData();

        var trigger = new GameObject(nameof(ActionKitUpdateTrigger)).AddComponent<ActionKitUpdateTrigger>();

        trigger.OnUpdate += () =>
        {
            m_Timer.CalculateTimeData();

            var calendarItems = m_Calendar.GetAvailableCalendarItems(m_Timer.CurrentSeconds);

            foreach (var calendarItem in calendarItems)
            {
                m_ActionPlayer.Play(calendarItem.Action);
                m_Calendar.FinishCalendarItem(calendarItem);
            }
        };
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
    public CallbackAction(System.Action callback)
    {
        m_Callback = callback;
    }

    private System.Action m_Callback;

    public override void Execute()
    {
        m_Callback();
    }
}

public class Calendar
{
    private List<CalendarItem> m_CalendarItems = new List<CalendarItem>();

    public void Record(CalendarItem item)
    {
        m_CalendarItems.Add(item);
    }

    public void FinishCalendarItem(CalendarItem item)
    {
        m_CalendarItems.Remove(item);
    }

    public List<CalendarItem> GetAvailableCalendarItems(float currentSeconds)
    {
        return m_CalendarItems.Where(item => item.StartSeconds < currentSeconds).ToList();
    }
}

public class CalendarItem
{
    public float StartSeconds;

    public BaseAction Action;
}

public class Timer
{
    public float CurrentSeconds;

    private float mStartSeconds;

    public void Start()
    {
        mStartSeconds = Time.time;
    }

    public void CalculateTimeData()
    {
        CurrentSeconds = Time.time - mStartSeconds;
    }
}