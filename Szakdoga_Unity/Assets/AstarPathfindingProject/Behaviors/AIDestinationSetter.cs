using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding
{
    /** Sets the destination of an AI to the position of a specified object.
	 * This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
	 * This component will then make the AI move towards the #target set on this component.
	 *
	 * \see #Pathfinding.IAstarAI.destination
	 *
	 * \shadowimage{aidestinationsetter.png}
	 */
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
    public class AIDestinationSetter : VersionedMonoBehaviour
    {
        /** The object that the AI should move to */
        public Transform target;
        private Unit unit;
        public Transform tempTarget;
        public IAstarAI ai;

        void OnEnable()
        {
            ai = GetComponent<IAstarAI>();
            unit = GetComponent<Unit>();
            // Update the destination right before searching for a path as well.
            // This is enough in theory, but this script will also update the destination every
            // frame as the destination is used for debugging and may be used for other things by other
            // scripts as well. So it makes sense that it is up to date every frame.
            if (ai != null)
            {
                ai.onSearchPath += LateUpdate;
                ai.isStopped = true;
            }
        }

        void OnDisable()
        {
            if (ai != null) ai.onSearchPath -= LateUpdate;
        }

        void OnCollisionEnter(Collision other)
        {
            if (target == other.transform)
            {
                ai.isStopped = true;
                target = null;
            }
            else if (gameObject.GetComponent<AIDestinationSetter>().ai.remainingDistance <= 1)
            {
                ai.isStopped = true;
            }

        }

        void Start()
        {
            //StartCoroutine("AutoAttack");
        }

        //Auto Attack
        IEnumerator AutoAttack()
        {
            while (true)
            {
                if (!gameObject.GetComponent<Unit>().isGatherer && target == null && ai.isStopped)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, gameObject.GetComponent<Unit>().Range * 5, 1 << LayerMask.NameToLayer("Unit"));
                    ArrayList enemyUnits = new ArrayList();
                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        if (!hitColliders[i].gameObject.GetComponent<Unit>().Owner.Equals(Game.currentPlayer.empireName))
                        {
                            enemyUnits.Add(hitColliders[i].transform);
                        }
                    }

                    //Find Closest Enemy
                    Transform bestTarget = null;
                    float closestDistanceSqr = Mathf.Infinity;
                    Vector3 currentPosition = transform.position;
                    foreach (Transform potentialTarget in enemyUnits)
                    {
                        Vector3 directionToTarget = potentialTarget.position - currentPosition;
                        float dSqrToTarget = directionToTarget.sqrMagnitude;
                        if (dSqrToTarget < closestDistanceSqr)
                        {
                            closestDistanceSqr = dSqrToTarget;
                            bestTarget = potentialTarget;
                        }
                    }
                    target = bestTarget;
                }
                yield return null;
            }
        }

       public IEnumerator SearchDropOffPoint()
        {
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 10000, 1 << LayerMask.NameToLayer("Unit"));
            ArrayList dropOffPoints = new ArrayList();
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.GetComponent<Structure>() != null &&
                    hitColliders[i].gameObject.GetComponent<Structure>().isDropOffPoint &&
                    hitColliders[i].gameObject.GetComponent<Structure>().Owner.Equals(Game.currentPlayer.empireName))
                {
                    dropOffPoints.Add(hitColliders[i].transform);
                }
                //Find Closest Enemy
                Transform bestTarget = null;
                float closestDistanceSqr = Mathf.Infinity;
                Vector3 currentPosition = transform.position;
                foreach (Transform potentialTarget in dropOffPoints)
                {
                    Vector3 directionToTarget = potentialTarget.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget;
                    }
                }
                target = bestTarget;
            }
            yield return null;
        }

        /** Updates the AI's destination every frame */
        void LateUpdate()
        {
            if (ai != null && target != null)
            {
                ai.destination = target.transform.position;
                ai.isStopped = false;
            }
            //Set Destination or target
            if (unit.Selected && unit.isWalkable)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    ai.isStopped = false;
                    if (Common.ShiftKeysDown())
                    {
                        if (!ai.hasPath && unit.ActionsQueue.Count == 0)
                        {
                            if (ai != null) ai.destination = GameObject.Find("Game").GetComponent<Mouse>().RightClickPoint;
                        }
                        else
                        {
                            unit.ActionsQueue.Enqueue(GameObject.Find("Game").GetComponent<Mouse>().RightClickPoint);
                            Debug.Log(unit.ActionsQueue.Count);
                        }
                    }
                    else
                    {
                        if (ai != null && target != null) ai.destination = target.transform.position;
                        if (ai != null) ai.destination = GameObject.Find("Game").GetComponent<Mouse>().RightClickPoint;
                        unit.ActionsQueue.Clear();
                    }
                }
            }

            //Right Click Attack
            if (target != null && target.gameObject.layer != LayerMask.NameToLayer("Resources") && !target.gameObject.GetComponent<Unit>().Owner.Equals(Game.currentPlayer.empireName) &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= gameObject.GetComponent<Unit>().Range * 5)
            {
                ai.isStopped = true;
                if (target.gameObject.GetComponent<Unit>().currentHealth > 0)
                    gameObject.GetComponent<Unit>().AttackTarget(target);
                else
                {
                    Destroy(target.gameObject);
                    target = null;
                }
            }

            //Gather
            if (target != null && target.gameObject.layer == LayerMask.NameToLayer("Resources") &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= 20)
            {
                if (gameObject.GetComponent<Unit>().CurrentResourceAmount != gameObject.GetComponent<Unit>().MaxResourceAmount)
                {
                    ai.isStopped = true;
                    gameObject.GetComponent<Unit>().GatherTarget(target);
                }
                else
                {
                    tempTarget = target;
                    if (target.GetComponent<Structure>() == null || !target.GetComponent<Structure>().isDropOffPoint)
                    {
                        StartCoroutine("SearchDropOffPoint");
                    }
                }

            }           

            //Resource Drop
            if (target != null && target.GetComponent<Structure>() != null && target.GetComponent<Structure>().isDropOffPoint &&
                Vector3.Distance(transform.position, target.position) < 20 && target.GetComponent<Structure>().Owner.Equals(Game.currentPlayer.empireName))
            {
                ai.isStopped = true;
                if (gameObject.GetComponent<Unit>().CurrentCarriedResource == resourceType.Iridium)
                    Game.currentPlayer.iridium += gameObject.GetComponent<Unit>().CurrentResourceAmount;
                else if (gameObject.GetComponent<Unit>().CurrentCarriedResource == resourceType.Palladium)
                    Game.currentPlayer.palladium += gameObject.GetComponent<Unit>().CurrentResourceAmount;
                else Game.currentPlayer.nullElement += gameObject.GetComponent<Unit>().CurrentResourceAmount;

                gameObject.GetComponent<Unit>().CurrentCarriedResource = resourceType.None;
                gameObject.GetComponent<Unit>().CurrentResourceAmount = 0;
                target = tempTarget;
            }
        }

    }
}

