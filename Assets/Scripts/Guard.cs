using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardSpotPlayer;

    public Transform pathHolder;
    public float speed = 7f;
    public float waitTime = 0.5f;
    public float trunSpeed = 90;
    public float timeToSpot = 0.5f;

    public Light spotLight;
    public float viewDistance = 10f;
    public LayerMask viewMask;

    float playerVisibleTimer;
    float viewAngle;
    Color originalSpotLightColor;
    Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotLight.spotAngle;
        originalSpotLightColor = spotLight.color;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i=0; i<waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i].y = transform.position.y;
        }

        StartCoroutine(FollowPath(waypoints));
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime; 
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpot);
        spotLight.color = Color.Lerp(originalSpotLightColor, Color.red, playerVisibleTimer / timeToSpot);
        if(playerVisibleTimer >= timeToSpot)
        {
            if(OnGuardSpotPlayer != null)
            {
                OnGuardSpotPlayer();
            }
        }
    }

    bool CanSeePlayer()
    {
        if(Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if(angleToPlayer < viewAngle / 2f)
            {
                if(!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int nextWayPoint = 1;
        Vector3 nextWaypointPosition = waypoints[nextWayPoint];
        transform.LookAt(nextWaypointPosition);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextWaypointPosition, speed * Time.deltaTime);
            if(transform.position == nextWaypointPosition)
            {
                nextWayPoint = (nextWayPoint + 1) % waypoints.Length;
                nextWaypointPosition = waypoints[nextWayPoint];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(nextWaypointPosition)); 
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTraget)
    {
        Vector3 directionToLookTraget = (lookTraget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(directionToLookTraget.z, directionToLookTraget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, trunSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach(Transform wayPoint in pathHolder)
        {
            Gizmos.DrawSphere(wayPoint.position, 0.2f);
            Gizmos.DrawLine(previousPosition, wayPoint.position);
            previousPosition = wayPoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
