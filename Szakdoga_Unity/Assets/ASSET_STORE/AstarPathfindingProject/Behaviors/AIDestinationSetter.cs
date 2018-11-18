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
        public bool isAttacking, isGathering, isRepairing, repairStarted;

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
            mouse = GameObject.Find("Game").GetComponent<Mouse>();
            StartCoroutine("AutoAttack");
        }

        //Auto Attack
        IEnumerator AutoAttack()
        {
            while (true)
            {
                if (!gameObject.GetComponent<Unit>().isGatherer && target == null && (ai.isStopped || unit.aMove))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, gameObject.GetComponent<Unit>().Range * 5, 1 << LayerMask.NameToLayer("Unit"));
                    ArrayList enemyUnits = new ArrayList();
                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        if (hitColliders[i].gameObject.GetComponent<Unit>() != null &&
                            !hitColliders[i].gameObject.GetComponent<Unit>().Owner.Equals(unit.Owner))
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
                    yield return null;
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
                    hitColliders[i].gameObject.GetComponent<Structure>().Owner.Equals(Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().empireName))
                {
                    dropOffPoints.Add(hitColliders[i].transform);
                }
                //Find Closest Target
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
            //Ha van célpont beállítva, menjen a célpont pozíciójához
            if (ai != null && target != null && !unit.HoldPosition)
            {
                ai.destination = target.transform.position;
                ai.isStopped = false;
            }

            #region Idle egységek
            //Az épp munkát nem végzõ egységek eltárolása
            if (unit.isGatherer)
            {
                if (ai.isStopped && !isGathering && !isRepairing)
                {
                    if (!Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().IdleWorkers.Contains(gameObject))
                    {
                        unit.Idle = true;
                        Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().IdleWorkers.Add(gameObject);
                    }
                }
                else
                {
                    if (Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().IdleWorkers.Contains(gameObject))
                    {
                        unit.Idle = false;
                        Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().IdleWorkers.Remove(gameObject);
                    }
                }
                if (isGathering && target == null)
                {
                    isGathering = false;
                    ai.isStopped = true;
                }
            }
            #endregion

            //Célhely kijelölése
            if (unit.Selected && unit.isWalkable && unit.Owner == Game.currentPlayer.empireName)
            {
                //Debug.Log(Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().enemies.Count);
                if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject() && !GUISetup.GhostActive)
                {
                    #region Jobb katt parancsvisszavonások
                    if (isGathering)
                    {
                        unit.StopCoroutine("GatherTarget");
                        isGathering = false;
                        repairStarted = false;
                        tempTarget = null;
                    }

                    if (isAttacking)
                    {
                        unit.StopCoroutine("AttackTarget");
                        isAttacking = false;
                        target = null;
                    }

                    if (isRepairing)
                    {
                        unit.StopCoroutine("Repair");
                        isRepairing = false;
                        target = null;
                    }
                    #endregion

                    if (unit.CurrentlyBuiltObject != null)
                    {
                        Structure building = unit.CurrentlyBuiltObject.GetComponent<Structure>();
                        Game.currentPlayer.iridium += building.iridiumCost;
                        Game.currentPlayer.palladium += building.palladiumCost;
                        Game.currentPlayer.nullElement += building.eezoCost;

                        GameObject.Find("Game").GetComponent<GUISetup>().UpdatePlayerInfoBar();

                        unit.StopCoroutine("Build");
                        unit.CurrentlyBuiltObject = null;
                    }

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
                        if (mouse.RightClickPoint != Vector3.positiveInfinity && !mouse.MoveMode && !mouse.AttackMode)
                        {
                            unit.HoldPosition = false;
                            if (ai != null && target != null) ai.destination = target.transform.position;
                            if (ai != null) ai.destination = mouse.RightClickPoint;
                            unit.ActionsQueue.Clear();
                            Debug.Log(ai.destination);
                        }
                        else mouse.EndModes();
                    }
                }
            }

            #region Javítás
            if (unit.isGatherer && target != null && target.gameObject.layer == LayerMask.NameToLayer("Unit") && target.gameObject.GetComponent<Structure>() != null &&
                !Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().enemies.Contains(Game.players.Where(x => x.empireName.Equals(target.GetComponent<Unit>().Owner)).SingleOrDefault()))
            {
                isRepairing = true;
                if (Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) < 10)
                {
                    ai.isStopped = true;
                    var q = new Quaternion();
                    if ((target.position - transform.position) != Vector3.zero)
                        q = Quaternion.LookRotation(target.position - transform.position);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 150 * Time.deltaTime);
                    //Csak akkor javít, ha nincs maxon az élete
                    if (target.gameObject.GetComponent<Structure>().currentHealth < target.gameObject.GetComponent<Structure>().maxHealth)
                    {
                        if (!repairStarted)
                        {
                            unit.StartCoroutine("Repair", target);
                            repairStarted = true;
                        }
                        else
                        {
                            unit.StopCoroutine("Repair");
                            isRepairing = false;
                            repairStarted = false;
                            target = null;
                        }
                    }
                }
            }
            #endregion

            #region Cselekvési sor
            //Cselekvési sorban levõ parancsok végrehajtása
            if (unit.ActionsQueue != null)
            {
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
            }
            #endregion

            #region Támadás
            if (target != null && target.gameObject.layer == LayerMask.NameToLayer("Unit") &&
                Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().enemies.Contains(Game.players.Where(x => x.empireName.Equals(target.GetComponent<Unit>().Owner)).SingleOrDefault()))
            {
                if (unit.HoldPosition && Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) > unit.Range * 5)
                {
                    target = null;
                    isAttacking = false;
                    return;
                }

                else if (Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= unit.Range * 5)
                {
                    //Ha a célpont lõtávon belül van, megáll és megtámadja a célt
                    ai.isStopped = true;
                    var q = new Quaternion();
                    if ((target.position - transform.position) != Vector3.zero)
                        q = Quaternion.LookRotation(target.position - transform.position);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 150 * Time.deltaTime);
                    //Csak akkor támadja ha van élete
                    if (transform.rotation == q)
                    {
                        if (target.gameObject.GetComponent<Unit>().currentHealth > 0)
                        {
                            if (!isAttacking)
                            {
                                unit.StartCoroutine("AttackTarget", target);
                                isAttacking = true;
                            }
                        }
                        //Különben elpusztul a cél
                        else
                        {
                            if (target.GetComponent<Structure>() != null)
                            {
                                target.GetComponent<Collider>().enabled = false;
                                AstarPath.active.UpdateGraphs(target.GetComponent<Collider>().bounds);
                            }
                            Unit unitObj = target.GetComponent<Unit>();
                            if (Game.players.Where(x => x.empireName.Equals(unitObj.Owner)).SingleOrDefault().units.Contains(target.gameObject))
                            {
                                Game.players.Where(x => x.empireName.Equals(unitObj.Owner)).SingleOrDefault().units.Remove(target.gameObject);
                                Game.players.Where(x => x.empireName.Equals(unitObj.Owner)).SingleOrDefault().CurrentPopulation -= target.GetComponent<Unit>().PopulationCost;
                                if (Game.players.Where(x => x.empireName.Equals(unitObj.Owner)).SingleOrDefault().CurrentWorkers.Contains(target.gameObject))
                                    Game.players.Where(x => x.empireName.Equals(unitObj.Owner)).SingleOrDefault().CurrentWorkers.Remove(target.gameObject);
                                GameObject.Find("Game").GetComponent<GUISetup>().UpdatePlayerInfoBar();
                            }
                            Destroy(target.gameObject);
                            target = null;
                            isAttacking = false;
                        }
                    }
                }
                else
                {
                    unit.StopCoroutine("AttackTarget");
                    isAttacking = false;
                }
            }

            //Leszakadás a célpontról
            if (target != null && target.gameObject.layer == LayerMask.NameToLayer("Unit") &&
                Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().enemies.Contains(Game.players.Where(x => x.empireName.Equals(target.GetComponent<Unit>().Owner)).SingleOrDefault()) &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) >= unit.Range * 10)
            {
                ai.isStopped = true;
                target = null;
            }


            #endregion

            #region Gyûjtögetés
            if (target != null && target.gameObject.layer == LayerMask.NameToLayer("Resources") &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= 15)
            {
                ai.isStopped = true;
                var q = new Quaternion();
                if ((target.position - transform.position) != Vector3.zero)
                    q = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 150 * Time.deltaTime);
                if (transform.rotation == q)
                {
                    //Ha a jelenleg cipelt nyersanyag nem éri el a kapacitást, akkor gyûjt a célpontból
                    if (unit.CurrentResourceAmount != unit.MaxResourceAmount)
                    {
                        if (!isGathering)
                        {
                            unit.StartCoroutine("GatherTarget", target);
                            isGathering = true;
                        }
                    }
                    //Ha tele van, megkeresi a legközelebbi leadópontot
                    else
                    {
                        tempTarget = target;
                        if (target.GetComponent<Structure>() == null || !target.GetComponent<Structure>().isDropOffPoint)
                        {
                            StartCoroutine("SearchDropOffPoint");
                            unit.StopCoroutine("GatherTarget");
                            isGathering = false;
                        }
                    }
                }

            }
            #endregion

            #region Nyersanyag leadás
            if (target != null && !isRepairing && target.GetComponent<Structure>() != null && target.GetComponent<Structure>().isDropOffPoint &&
                Vector3.Distance(transform.position, target.position) < 20 && target.GetComponent<Structure>().Owner.Equals(Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().empireName))

            {
                //Cipelt nyersanyag raktárhoz való hozzáadása
                ai.isStopped = true;
                if (unit.CurrentCarriedResource == resourceType.Iridium)
                    Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().Iridium += (int)unit.CurrentResourceAmount;
                else if (unit.CurrentCarriedResource == resourceType.Palladium)
                    Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().palladium += (int)unit.CurrentResourceAmount;
                else Game.players.Where(x => x.empireName.Equals(unit.Owner)).SingleOrDefault().nullElement += (int)unit.CurrentResourceAmount;

                unit.CurrentCarriedResource = resourceType.None;
                unit.CurrentResourceAmount = 0;
                target = tempTarget;
            }
            #endregion
        }
    }
}

