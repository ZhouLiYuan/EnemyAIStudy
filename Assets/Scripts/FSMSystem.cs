using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 转换状态
/// </summary>
public enum Transition
{
    NullTransition = 0,
    LostPlayer,
    SawPlayer,
}

/// <summary>
/// 状态ID
/// </summary>
public enum StateID
{
    NullStateID = 0,
    FollowingPath,
    ChasingPlayer,
}

/// <summary>
/// 有限状态机系统中的状态
/// 每个状态都有一个字典，字典中有键值对(转换-状态),保存转换Key时，把枚举转换为int，作为key
/// 表示如果在当前状态下触发转换，那么FSM应该处于对应的状态。
/// </summary>
public abstract class FSMState
{
    protected Dictionary<int, StateID> m_Map = new Dictionary<int, StateID>();
    protected StateID stateID;
    public StateID ID { get { return stateID; } }

    /// <summary>
    /// 添加转换
    /// </summary>
    public void AddTransition(Transition trans, StateID id)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
            return;
        }

        if (id == StateID.NullStateID)
        {
            Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
            return;
        }
        int transition = (int)trans;
        if (m_Map.ContainsKey(transition))
        {
            Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() +
                           "Impossible to assign to another state");
            return;
        }

        m_Map.Add(transition, id);
    }

    /// <summary>
    /// 删除转换
    /// </summary>
    public void DeleteTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }
        int transition = (int)trans;
        if (m_Map.ContainsKey(transition))
        {
            m_Map.Remove(transition);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() +
                       " was not on the state's transition list");
    }

    /// <summary>
    /// 根据转换返回状态ID
    /// </summary>
    public StateID GetOutputState(Transition trans)
    {
        int transition = (int)trans;
        if (m_Map.ContainsKey(transition))
        {
            return m_Map[transition];
        }
        return StateID.NullStateID;
    }

    /// <summary>
    /// 用于进入状态前，设置进入的状态条件
    /// 在进入当前状态之前，FSM系统会自动调用
    /// </summary>
    public virtual void DoBeforeEntering() { }

    /// <summary>
    /// 用于离开状态时的变量重置
    /// 在更改为新状态之前，FSM系统会自动调用
    /// </summary>
    public virtual void DoBeforeLeaving() { }

    /// <summary>
    /// 用于判断是否可以转换到另一个状态,每帧都会执行
    /// </summary>
    public abstract void CheckTransition(GameObject player, GameObject npc);

    /// <summary>
    /// 控制NPC行为,每帧都会执行
    /// </summary>
    public abstract void Act(GameObject player, GameObject npc);

}


/// <summary>
/// FSMSystem类
/// 持有一个状态集合
/// </summary>
public class FSMSystem
{
    private List<FSMState> m_States;

    public StateID CurrentStateID { get; private set; }

    public FSMState CurrentState { get; private set; }

    public FSMSystem()
    {
        m_States = new List<FSMState>();
    }

    /// <summary>
    /// 添加新的状态
    /// </summary>
    public void AddState(FSMState state)
    {
        if (state == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }

        if (m_States.Count == 0)
        {
            m_States.Add(state);
            CurrentState = state;
            CurrentStateID = state.ID;
            return;
        }

        foreach (FSMState s in m_States)
        {
            if (s.ID == state.ID)
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + state.ID.ToString() +
                    " because state has already been added");
                return;
            }
        }
        m_States.Add(state);
    }

    /// <summary>
    /// 删除状态
    /// </summary>
    public void DeleteState(StateID id)
    {
        if (id == StateID.NullStateID)
        {
            Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
            return;
        }

        foreach (FSMState state in m_States)
        {
            if (state.ID == id)
            {
                m_States.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
                       ". It was not on the list of states");
    }

    /// <summary>
    /// 通过转换，改变FSM的状态
    /// </summary>
    public void PerformTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
            return;
        }
        //获取转换对应的状态ID
        StateID id = CurrentState.GetOutputState(trans);
        if (id == StateID.NullStateID)
        {
            Debug.LogError("FSM ERROR: State " + CurrentStateID.ToString() + " does not have a target state " +
                           " for transition " + trans.ToString());
            return;
        }

        // 更新当前状态ID，currentStateID		
        CurrentStateID = id;
        foreach (FSMState state in m_States)
        {
            if (state.ID == CurrentStateID)
            {
                // 离开状态时的变量重置
                CurrentState.DoBeforeLeaving();
                // 更新当前状态currentState
                CurrentState = state;
                // 进入状态前，设置进入的状态条件
                CurrentState.DoBeforeEntering();
                break;
            }
        }
    }
}