using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeCreatePlay.LocationTool;


namespace AI_States
{
    public class Work : Base
    {
        public Work(AI_Character character):base(character)
        {
        }

        public override void Enter()
        {
            character.currentLocation = character.lt_globals.GetLocation("Home_01");
            character.currentDest = character.currentLocation.GetRandomDestination();
            character.agent.SetDestination(character.currentDest);

        }

        public override void Execute()
        {
            if(Vector3.Distance(character.transform.position, character.currentDest) < 1.25f)
            {
                character.currentLocation = character.lt_globals.GetLocation("Home_01");
                character.currentDest = character.currentLocation.GetRandomDestination();
                character.agent.SetDestination(character.currentDest);
            }
        }

        public override void Exit()
        {
        }
    }
}
