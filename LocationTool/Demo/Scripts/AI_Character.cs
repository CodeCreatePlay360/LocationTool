using UnityEngine;
using CodeCreatePlay.LocationTool;
using UnityEngine.AI;
using AI_States;


[RequireComponent(typeof(NavMeshAgent))]
public class AI_Character : MonoBehaviour
{

    [System.Serializable]
    public class Stats
    {
        [Header("Stats")]
        public float stamina = 100;

        [Header("Stat modifiers")]
        [Tooltip("Stamina")]
        public float fatigued_ = 6f;
    }

    [HideInInspector] public Game_AI.StateMachine stateMachine = default;
    public Rest restState = null;
    public Wander wander = null;

    public Stats stats = new ();

    public NavMeshAgent agent = null;
    [HideInInspector] public LT_Globals lt_globals = null;
    [HideInInspector] public LocationBase currentLocation = null;
    [HideInInspector] public Vector3 currentDest;
    

    private void Start()
    {
        // init state machine 
        stateMachine = new Game_AI.StateMachine(null);

        // and AI character's states
        restState = new Rest(this);
        wander = new Wander(this);

        var lt_globals_go = GameObject.FindGameObjectWithTag("LT_Globals");
        agent = GetComponent<NavMeshAgent>();

        if (lt_globals_go)
            lt_globals = lt_globals_go.GetComponent<LT_Globals>();

        // get a random location of type CampFire, at least one location of type CampFire must exist in scene 
        currentLocation = lt_globals.GetRandomLocation(LocationCategory.CampFire);

        // get a random destination in this location
        currentDest = currentLocation.GetRandomDestination();

        // set agent's destination to current destination
        agent.SetDestination(currentDest);

        /*
        // get all locations
        var allLocations = lt_globals.Locations;

        // get a random location of category, returns null if none found
        currentLocation = lt_globals.GetRandomLocation(LocationCategory.Area);

        // get a location by name, returns null if none found
        currentLocation = lt_globals.GetLocation("Inn");

        // get all destinations in a location
        var allDestinations = currentLocation.destinations;

        // get a random destination
        currentDest = currentLocation.GetRandomDestination();
        */

        // stateMachine.SwitchState(restState);
    }

    private void Update()
    {
        // update state machine every frame.
        // stateMachine.Update(); 
        // velocity = agent.velocity;
    }
}
