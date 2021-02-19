using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderIKSolver : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    [SerializeField] SpiderIKSolver otherFoot = default;
    [SerializeField] float speed = 5f;
    [SerializeField] float stepDistance = .5f;
    [SerializeField] float stepLength = .5f;
    [SerializeField] float stepHeight = .2f;
    [SerializeField] Vector3 footOffset = default;
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp; // >= 1 means leg is not moving, otherwise it is

    // Start is called before the first frame update
    void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        // create the raycast Ray
        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

        // this is where the actual raycast is made, raycast information stored in info. Executes if statement on a successful raycast
        if (Physics.Raycast(ray, out RaycastHit info, 3, terrainLayer.value))
        {
            // checks if distance is big enough to move, the other leg is not moving, and this foot is not moving
            if (Vector3.Distance(newPosition, info.point) > stepDistance /*&& !otherFoot.IsMoving()*/ && lerp >= 1)
            {
                lerp = 0;
                // checks if the leg should be moving forwards or backwards
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = info.normal;
            }
        }

        // if we are within our movement cycle
        if (lerp < 1)
        {
            // interpolates between 2 points, lerp is the percentage of completion.
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            // make an arc in the movement
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}