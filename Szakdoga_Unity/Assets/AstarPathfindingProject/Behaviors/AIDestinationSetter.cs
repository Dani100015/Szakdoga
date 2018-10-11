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

        public IAstarAI ai;

        void OnEnable()
        {
            ai = GetComponent<IAstarAI>();
            unit = GetComponent<Unit>();
            // Update the destination right before searching for a path as well.
            // This is enough in theory, but this script will also update the destination every
            // frame as the destination is used for debugging and may be used for other things by other
            // scripts as well. So it makes sense that it is up to date every frame.
            if (ai != null) ai.onSearchPath += LateUpdate;
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
            else if (gameObject.GetComponent<AIDestinationSetter>().ai.remainingDistance <=1)
            {
                ai.isStopped = true;
            }

        }

        /** Updates the AI's destination every frame */
        void LateUpdate()
        {
            if (ai != null && target != null)
            {
                ai.destination = target.transform.position;
                ai.isStopped = false;
            }
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
                        Debug.Log(ai.destination);
                    }
                }
            }
            if (target != null && target.gameObject.GetComponent<Unit>().Owner != 0 &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= gameObject.GetComponent<Unit>().Range * 5)
            {
                Debug.Log("Beléptem");
                ai.isStopped = true;
                if (target.gameObject.GetComponent<Unit>().currentHealth > 0)
                    gameObject.GetComponent<Unit>().AttackTarget(target);
                else
                {
                    Destroy(target.gameObject);
                    target = null;
                }
            }

            if (target != null && target.gameObject.layer == LayerMask.NameToLayer("Resources") &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= 100)
            {
                Debug.Log("Gyûjtök");
                ai.isStopped = true;
                
            }

        }

    }
}

