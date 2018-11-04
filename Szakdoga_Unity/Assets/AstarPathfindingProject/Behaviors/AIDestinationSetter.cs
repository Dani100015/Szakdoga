using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

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
        Mouse mouse;

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
            mouse = GameObject.Find("Game").GetComponent<Mouse>();
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
                Debug.Log(target);
            }
            yield return null;
        }

        /** Updates the AI's destination every frame */
        void LateUpdate()
        {
            //Ha van célpont beállítva, menjen a célpont pozíciójához
            if (ai != null && target != null)
            {
                ai.destination = target.transform.position;
                ai.isStopped = false;
            }

            //Célhely kijelölése
            if (unit.Selected && unit.isWalkable && unit.Owner == Game.currentPlayer.empireName)
            {
                //Debug.Log(Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().enemies.Count);
                if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && !GUISetup.GhostActive)
                {
                    ai.isStopped = false;
                    //Ha le van nyomva a shift, akkor a sorhoz szeretnénk adni
                    if (Common.ShiftKeysDown())
                    {
                        //Ha a cselekvési sor üres, csak küldjük a célhelyre
                        if (!ai.hasPath && unit.ActionsQueue.Count == 0)
                        {
                            if (ai != null) ai.destination = mouse.RightClickPoint;
                        }
                        //Ha nem, adjuk hozzá a sorhoz a jobb klikk helyét
                        else
                        {
                            unit.ActionsQueue.Enqueue(mouse.RightClickPoint);
                        }
                    }
                    //Ha nincs lenyomva, akkor nem szeretnénk hozzáadni, valamint kiüríteni a sort ha nem üres
                    else
                    {
                        if (ai != null && target != null) ai.destination = target.transform.position;
                        if (ai != null) ai.destination = mouse.RightClickPoint;
                        unit.ActionsQueue.Clear();
                    }
                }
            }

            if (ai.isStopped && unit.ActionsQueue.Count >= 1)
            {
                if (unit.ActionsQueue.Peek() is Vector3)
                    ai.destination = (Vector3)unit.ActionsQueue.Dequeue();
                else
                {
                    if (target == null)
                    {
                        gameObject.GetComponent<AIDestinationSetter>().target = (Transform)unit.ActionsQueue.Dequeue();
                        unit.ActionsQueue.Dequeue();
                    }
                }

                ai.isStopped = false;
            }

            //Jobb Klikk Támadás
            if (target != null && target.gameObject.layer != LayerMask.NameToLayer("Resources") && 
                Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().enemies.Contains(Game.players.Where(x => x.empireName.Equals(target.GetComponent<Unit>().Owner)).SingleOrDefault()) &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= unit.Range * 5)
            {
                //Ha a célpont lõtávon belül van, megáll és megtámadja a célt
                ai.isStopped = true;
                //Csak akkor támadja ha van élete
                if (target.gameObject.GetComponent<Unit>().currentHealth > 0)
                    unit.AttackTarget(target);
                //Különben elpusztul a cél
                else
                {
                    if (Game.players.Where(x => x.empireName.Equals(target.GetComponent<Unit>().Owner)).SingleOrDefault().units.Contains(target.gameObject))
                        Game.players.Where(x => x.empireName.Equals(target.GetComponent<Unit>().Owner)).SingleOrDefault().units.Remove(target.gameObject);
                    Destroy(target.gameObject);
                    target = null;
                }
            }

            //Gyûjtögetés
            if (target != null && target.gameObject.layer == LayerMask.NameToLayer("Resources") &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= 20)
            {
                //Ha a jelenleg cipelt nyersanyag nem éri el a kapacitást, akkor gyûjt a célpontból
                if (unit.CurrentResourceAmount != unit.MaxResourceAmount)
                {
                    ai.isStopped = true;
                    unit.GatherTarget(target);
                }
                //Ha tele van, megkeresi a legközelebbi leadópontot
                else
                {
                    tempTarget = target;
                    if (target.GetComponent<Structure>() == null || !target.GetComponent<Structure>().isDropOffPoint)
                    {
                        StartCoroutine("SearchDropOffPoint");
                        Debug.Log("Keresek");
                    }
                }

            }

            //Nyersanyag leadás
            if (target != null && target.GetComponent<Structure>() != null && target.GetComponent<Structure>().isDropOffPoint &&
                Vector3.Distance(transform.position, target.position) < 20 && target.GetComponent<Structure>().Owner.Equals(Game.currentPlayer.empireName))
            {
                //Cipelt nyersanyag raktárhoz való hozzáadása
                ai.isStopped = true;
                if (unit.CurrentCarriedResource == resourceType.Iridium)
                    Game.currentPlayer.Iridium += unit.CurrentResourceAmount;
                else if (unit.CurrentCarriedResource == resourceType.Palladium)
                    Game.currentPlayer.Palladium += unit.CurrentResourceAmount;
                else Game.currentPlayer.NullElement += unit.CurrentResourceAmount;

                unit.CurrentCarriedResource = resourceType.None;
                unit.CurrentResourceAmount = 0;
                target = tempTarget;
            }
        }
    }
}

