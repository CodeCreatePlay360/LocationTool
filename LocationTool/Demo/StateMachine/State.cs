using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game_AI
{
    [System.Serializable]
    public abstract class State
    {
        // ** public fields **
        private string guid = "";


        // ** getter / setters ** 
        public string GUID { get { return guid; } }


        // constructor
        public State()
        {
            guid = System.Guid.NewGuid().ToString();
        }

        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();
    }
}
