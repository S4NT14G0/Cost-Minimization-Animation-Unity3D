using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CostMinimizationPathFinding : MonoBehaviour {

    [SerializeField]
    Transform target;

    [SerializeField]
    float timeBetweenMovement = .5f;
    float timeSinceMovement = 0;

    GameObject[] obstacles;
    Vector3 currentPosition;

	// Use this for initialization
	void Start () {
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        Debug.Log(this.transform.position);
        currentPosition = this.transform.position;
	}

    // Update is called once per frame
    void FixedUpdate () {
        // Keep track of time for movement
        timeSinceMovement += Time.deltaTime;

        // Keep moving if the target is farther than .50 away
	    if (Vector3.Distance(currentPosition, target.position) > .50f && (timeSinceMovement > timeBetweenMovement))
        {

            float averageCost = 0.0f;
            int moves = 0;

            Dictionary<Vector3, float> possibleMovments = new Dictionary<Vector3, float>();

            for (int i = (int)currentPosition.x - 1; i <= (int)currentPosition.x + 1; i++)
            {
                for (int j = (int) currentPosition.z - 1; j <= (int) currentPosition.z + 1; j++)
                {
                    Vector3 possibleNextPosition = new Vector3(i, 0.5f, j);
                    float possibleNextPositionCost = calculateObstructionCostAroundNextCoord(possibleNextPosition, target.position);
                    possibleMovments.Add(possibleNextPosition, possibleNextPositionCost);
                }
            }

            var sortedHeatMaps = from heatMapCoord in possibleMovments orderby heatMapCoord.Value ascending select heatMapCoord;

            Debug.DrawLine(currentPosition, sortedHeatMaps.First().Key, Color.green, 10000f);

            averageCost = sortedHeatMaps.First().Value;

            currentPosition = sortedHeatMaps.First().Key;

            this.transform.position = currentPosition;

            moves++;

            Debug.Log((averageCost + moves) / 2);

            timeSinceMovement = 0;
        }
	}

    float calculateObstructionCostAroundNextCoord(Vector3 possibleNextPos, Vector3 targetPos)
    {
        float costSum = Vector3.Distance(possibleNextPos, targetPos);
        possibleNextPos = possibleNextPos * this.GetComponent<SphereCollider>().radius;

        foreach (GameObject obstacle in obstacles)
        {
            float obstacleForceFieldDistance = obstacle.GetComponent<CapsuleCollider>().radius;
            Vector3 distanceFromForceField = obstacle.transform.position * obstacleForceFieldDistance;

            float distanceFromObstacle = Vector3.Distance(possibleNextPos, distanceFromForceField);

            if (distanceFromObstacle > 0 && distanceFromObstacle <= obstacleForceFieldDistance)
            {
                costSum += Mathf.Log(obstacleForceFieldDistance / distanceFromObstacle);
            }
        }

        return costSum;
    }
}
