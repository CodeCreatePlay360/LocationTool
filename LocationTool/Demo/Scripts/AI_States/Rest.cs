using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeCreatePlay.LocationTool;


namespace AI_States
{
    public class Rest : Base
    {
        public Rest(AI_Character character) : base(character)
        {
        }

        public override void Enter()
        {
            // rest at campfire
            character.currentLocation = character.lt_globals.GetRandomLocation(LocationCategory.CampFire);
            character.currentDest = character.currentLocation.GetRandomDestination();
            character.agent.SetDestination(character.currentDest);
        }

        public override void Execute()
        {
            // wait until agent has stopped moving
            if (character.agent.velocity.magnitude > 0)
                return;

            character.stats.stamina += 10f * Time.deltaTime;

            if (character.stats.stamina >= 100)
            {
                character.stats.stamina = 100;

                // if recovered switch to wander state
                character.stateMachine.SwitchState(character.wander);
            }
        }

        public override void Exit()
        {
        }
    }
}
