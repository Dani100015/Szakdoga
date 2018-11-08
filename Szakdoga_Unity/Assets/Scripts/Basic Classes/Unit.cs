using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using System.Linq;

public class Unit : MonoBehaviour
{

    //for Mouse.cs
    public Vector2 ScreenPos;
    public bool OnScreen;
    public bool Selected = false;
    private GameObject DragSelect;
    public bool HoldPosition;

    public Queue ActionsQueue;
    public Vector3 CurrentTargetLocation;
    public int Range;
    public GameObject Projectile;
    public float attackSpeed;
    public int attackDamage;

    //Játékmechanika
    public int maxHealth;
    public int currentHealth;
    public int trainingTime;
    public int palladiumCost;
    public int iridiumCost;
    public int eezoCost;

    public bool aMove;
    public bool isWalkable = true;
    public string Owner;
    public Species Race;
    public Texture2D MenuIcon;
    public Texture2D MenuIconRo;

    //Gyűjtögetéshez
    public bool isGatherer;
    public float GatherSpeed;
    public float MaxResourceAmount;
    public float CurrentResourceAmount;
    public resourceType CurrentCarriedResource;
    [HideInInspector]
    public GameObject CurrentlyBuiltObject;
    [HideInInspector]
    public bool ShowBuildables;

    void Start()
    {
        ActionsQueue = new Queue();
        if (isGatherer)
            CurrentCarriedResource = resourceType.None;
    }

    void Awake()
    {
        //Physics.IgnoreLayerCollision(8, 8, true);
        if (transform.Find("DragSelect") != null)
            DragSelect = transform.Find("DragSelect").gameObject;
        if (transform.Find("Selected") != null)         
            transform.Find("Selected").gameObject.SetActive(false);
    }

    void HideGameObject()
    {
        Mouse.CurrentlySelectedUnits.Remove(gameObject);
        gameObject.transform.Find("Selected").gameObject.SetActive(false);
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.transform.Find("Unit").gameObject.SetActive(false);
    }

    void RevealGameObject()
    {
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.transform.Find("Unit").gameObject.SetActive(true);
    }

    public IEnumerator AttackTarget(Transform target)
    {
        while (target != null && Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= Range * 5)
        {
            Projectile = Instantiate(Resources.Load("Prefabs/Projectiles/Bullet"), transform.position, transform.rotation) as GameObject;
            Projectile.GetComponent<Rigidbody>().velocity = (target.position - gameObject.transform.position).normalized * 100;
            target.gameObject.GetComponent<Unit>().currentHealth -= attackDamage;
            Destroy(Projectile.gameObject, 0.5f);
            yield return new WaitForSeconds(attackSpeed);
        }
        yield return null;
    }

    public IEnumerator GatherTarget(Transform target)
    {
        while (target != null && Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= 20)
        {
            if (CurrentCarriedResource != target.gameObject.GetComponent<ResourceObject>().Type)
                CurrentResourceAmount = 0;
            CurrentCarriedResource = target.GetComponent<ResourceObject>().Type;
            if (target.gameObject.GetComponent<ResourceObject>().Capacity - GatherSpeed <= 0)
            {
                if (CurrentResourceAmount + target.gameObject.GetComponent<ResourceObject>().Capacity >= MaxResourceAmount)
                    CurrentResourceAmount = MaxResourceAmount;
                else CurrentResourceAmount += target.gameObject.GetComponent<ResourceObject>().Capacity;
                Destroy(target.gameObject);
                target = null;
                gameObject.GetComponent<AIDestinationSetter>().StartCoroutine("SearchDropOffPoint");
                yield return null;
            }

            if (CurrentResourceAmount + GatherSpeed >= MaxResourceAmount)
                CurrentResourceAmount = MaxResourceAmount;
            else CurrentResourceAmount += GatherSpeed;
            target.gameObject.GetComponent<ResourceObject>().Capacity -= GatherSpeed;
            yield return new WaitForSeconds(.2f);
        }
        yield return null;
    }

    public IEnumerator Build()
    {
        Debug.Log("Elindul");       
        
        AIDestinationSetter setter = transform.GetComponent<AIDestinationSetter>();
        while ((Vector3.Distance(transform.position, new Vector3(setter.ai.destination.x,-2.1f,setter.ai.destination.z)) > 2))
        {
            yield return null;
        }
        Structure building = CurrentlyBuiltObject.GetComponent<Structure>();


        Debug.Log("most itt");
        GameObject placeholder = Instantiate(Resources.Load("Prefabs/Placeholder"), new Vector3(setter.ai.destination.x, 5f, setter.ai.destination.z), Quaternion.identity) as GameObject;
        placeholder.name = building.name;
        Structure phbuild = placeholder.AddComponent<Structure>();
        phbuild.Owner = Owner;
        phbuild.maxHealth = building.maxHealth;

        HideGameObject();
        ShowBuildables = false;
        int i = 0;
        while (i < building.trainingTime)
        {
            if (CurrentlyBuiltObject != null)
            {
                phbuild.currentHealth += (phbuild.maxHealth / building.trainingTime);
                i++;
                yield return new WaitForSeconds(1f);
            }
            else
            {
                RevealGameObject();
                yield break;
            }
        }

        Mouse.DeselectGameObjectsIfSelected();
        Destroy(placeholder);       

        GameObject newUnit = Instantiate(CurrentlyBuiltObject, new Vector3(setter.ai.destination.x, 5f, setter.ai.destination.z), Quaternion.identity);
        newUnit.name = CurrentlyBuiltObject.name;
        newUnit.GetComponent<Unit>().Owner = Owner;
        CurrentlyBuiltObject = null;
        RevealGameObject();
        gameObject.transform.position = new Vector3(newUnit.transform.position.x, -2.1f, newUnit.transform.position.z - (newUnit.GetComponent<Collider>().bounds.size.z));

        AstarPath.active.UpdateGraphs(newUnit.GetComponent<Collider>().bounds);
        yield return null;
    }

    void Update()
    {
        #region Selection
        //if unit not selected, get screenspace
        if (!Selected)
        {
            //track the screen position
            if (DragSelect)
                ScreenPos = Camera.main.WorldToScreenPoint(DragSelect.transform.position);
            else ScreenPos = Camera.main.WorldToScreenPoint(transform.position);

            //if within screen space
            if (Mouse.UnitWithinScreenSpace(ScreenPos))
            {
                //and not already added to UnitsOnScreen, add it
                if (!OnScreen)
                {
                    Mouse.UnitsOnScreen.Add(this.gameObject);
                    OnScreen = true;
                }
            } //unit is not in screen space
            else
            {
                //remove if previously on screen
                if (OnScreen)
                {
                    Mouse.RemoveFromOnScreenUnits(this.gameObject);
                }
            }
        }
        #endregion
    }
}
