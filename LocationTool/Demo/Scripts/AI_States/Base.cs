using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI_States
{
    public class Base : Game_AI.State
    {
        public AI_Character character = null;


        public Base(AI_Character character)
        {
            this.character = character;
        }

        public override void Enter()
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}
