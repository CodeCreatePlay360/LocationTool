using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game_AI
{
    [System.Serializable]
    public class StateMachine
    {
        // ** public fields **
        [SerializeField] private State currentState = null;
        [SerializeField] private State globalState = null;


        // ** getter / setters ** 
        public State CurrentState { get { return currentState; } }
        public State GlobalState { get { return globalState; } }


        // constructor
        public StateMachine(State global)
        {
            globalState = global;

            // this can be null, but that's OK
            if (globalState != null)
                globalState.Enter();
        }

        public void SwitchState(State newState)
        {
            // make sure new state is not null
            if(newState == null)
            {
                Debug.Log("Cannot set StateMachine State to null");
                return;
            }

            // this is for first time when current state will be null
            if(currentState == null)
            {
                newState.Enter();
                currentState = newState;
                return;
            }

            // make sure we have a new state
            if(newState.GUID == currentState.GUID)
                return;

            // also make sure new state is not the global state
            if (globalState != null && newState.GUID == globalState.GUID)
                return;

            // exit current state
            currentState.Exit();

            // enter new state
            newState.Enter();

            // set current state to new state
            currentState = newState;
        }

        public void Update()
        {
            if (globalState != null)
                globalState.Execute();

            if(currentState != null)
                currentState.Execute();
        }
    }
}