using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretModule : VehicleModule
{
    public ItemQuality quality = ItemQuality.F;

    GameObject top;
    GameObject turret;
    bool enemyInSightRange;
    bool attacked;
    
    public float sightRange;
    public LayerMask whatIsEnemy;

    public override string GetName()
    {
        return "Turret Module " + quality.ToString();
    }

    public override ItemQuality GetQuality()
    {
        return quality;
    }

    public override string GetSpriteName()
    {
        return null;
    }

    protected override void OnEquip(VehicleController vehicle)
    {
        Debug.Log("Equipped " + gameObject.name);
    }

    protected override void OnUnequip(VehicleController vehicle)
    {
        Debug.Log("Unequipped " + gameObject.name);
    }

    private void Awake()
    {
        top = this.transform.GetChild(1).gameObject;
        turret = this.transform.GetChild(2).gameObject;
        attacked = false;
    }

    // Update is called once per frame
    void Update()
    {
        enemyInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);

        if (enemyInSightRange)
        {
            Vector3 enemyPos = findNearestEnemy();
            if (enemyPos != null)
                Attack(enemyPos);
        }
    }

    /// <summary>
    /// Turns the turret to the nearest enemy and shoots at it.
    /// </summary>
    /// <param name="enemyPos">The position of the enemy to attack.</param>
    private void Attack(Vector3 enemyPos)
    {
        Debug.Log(enemyPos);
        Vector3 topLook = new Vector3(enemyPos.x, top.transform.position.y, enemyPos.z);
        Vector3 turretLook = new Vector3(enemyPos.x, checkValue(enemyPos.y, 30), enemyPos.z);
        top.transform.LookAt(topLook);
        turret.transform.LookAt(turretLook);
        //SHOOT
        attacked = true;
        Invoke(nameof(ResetAttack), 1f);
    }

    private float checkValue(float val, float max)
    {
        if (val > max)
            return max;
        return val;
    }

    /// <summary>
    /// Finds the nearest enemy to this turret module.
    /// </summary>
    /// <returns>The closest enemy's position, null if there is no closest enemy.</returns>
    private Vector3 findNearestEnemy()
    {
        GameObject[] spiders = GameObject.FindGameObjectsWithTag("Enemy_Spider");
        GameObject[] trucks = GameObject.FindGameObjectsWithTag("Enemy_Truck");

        GameObject closest = null;
        float closestLength = Mathf.Infinity;
        for (int i = 0; i < spiders.Length; i++)
        {
            float dist = Vector3.Distance(spiders[i].transform.position, this.transform.position);
            if (dist < closestLength)
            {
                closest = spiders[i];
                closestLength = dist;
            }
        }
        for (int i = 0; i < trucks.Length; i++)
        {
            float dist = Vector3.Distance(trucks[i].transform.position, this.transform.position);
            if (dist < closestLength)
            {
                closest = trucks[i];
                closestLength = dist;
            }
        }

        return closest.transform.position;
    }

    private void ResetAttack()
    {
        attacked = false;
    }

    /// <summary>
    /// Creates a gizmo showing the sight range of the Spider
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}