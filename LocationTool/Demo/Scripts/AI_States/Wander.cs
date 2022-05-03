using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeCreatePlay.LocationTool;


namespace AI_States
{
    public class Wander : Base
    {
        public Wander(AI_Character character) : base(character)
        {
        }

        public override void Enter()
        {
            // go to a random point of interest
            character.currentLocation = character.lt_globals.GetRandomLocation(LocationCategory.PointOfInterest);
            character.currentDest = character.currentLocation.GetRandomDestination();
            character.agent.SetDestination(character.currentDest);
            Debug.Log("wander state");
        }

        public override void Execute()
        {
            if(character.agent.remainingDistance < 1.2f)
            {
                character.currentLocation = character.lt_globals.GetRandomLocation(LocationCategory.PointOfInterest);
                character.currentDest = character.currentLocation.GetRandomDestination();
                character.agent.SetDestination(character.currentDest);
            }
            else
            {
                character.stats.stamina -= 10f * Time.deltaTime;

                if (character.stats.stamina <= 20)
                {
                    character.stats.stamina = 20;

                    // if recovered switch to wander state
                    character.stateMachine.SwitchState(character.restState);
                }
            }
        }

        public override void Exit()
        {
        }
    }
}
