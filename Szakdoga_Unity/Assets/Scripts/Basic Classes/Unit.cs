using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Unit : MonoBehaviour {

    //for Mouse.cs
    public Vector2 ScreenPos;
    public bool OnScreen;
    public bool Selected = false;
    private GameObject DragSelect;

    public Queue<Vector3> ActionsQueue;
    public Vector3 CurrentTargetLocation;
    public int Range;
    public GameObject Projectile;
    public int attackSpeed;
    public int attackDamage;

    //Játékmechanika
    public int maxHealth;
    public int currentHealth;
    public int trainingTime;

    public bool isWalkable = true;
    public int Owner;
    public bool isGatherer;
    public int GatherSpeed;

    void Start()
    {
        ActionsQueue = new Queue<Vector3>();
    }

    void Awake()
    {
        //Physics.IgnoreLayerCollision(8, 8, true);
        if (transform.Find("DragSelect") != null)
            DragSelect = transform.Find("DragSelect").gameObject;
        if (transform.Find("Selected") != null)
            transform.Find("Selected").gameObject.SetActive(false);
    }

    public void AttackTarget(Transform target)
    {
        var q = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 150 * Time.deltaTime);
        if (transform.rotation == q)
        {
            Projectile = Instantiate(Resources.Load("Bullet"), transform.position, transform.rotation) as GameObject;
            Projectile.GetComponent<Rigidbody>().velocity = (target.position - gameObject.transform.position).normalized * 100;
            target.gameObject.GetComponent<Unit>().currentHealth -= attackDamage;
        }
        Destroy(Projectile.gameObject, 0.5f);
    }

    public void GatherTarget(Transform target)
    {
        var q = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 150 * Time.deltaTime);
        if (transform.rotation == q)
        {
            Projectile = Instantiate(Resources.Load("Bullet"), transform.position, transform.rotation) as GameObject;
            Projectile.GetComponent<Rigidbody>().velocity = (target.position - gameObject.transform.position).normalized * 100;
            target.gameObject.GetComponent<ResourceObject>().Capacity -= gameObject.GetComponent<Unit>().GatherSpeed;
        }
        Destroy(Projectile.gameObject, 0.2f);
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
