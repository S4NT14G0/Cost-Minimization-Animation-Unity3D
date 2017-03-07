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
        currentPosition = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate () {
        // Keep track of time for movement
        timeSinceMovement += Time.deltaTime;

        // Keep moving if the target is farther than .50 away
	    if (Vector3.Distance(currentPosition, target.position) > .50f && (timeSinceMovement > timeBetweenMovement))
        {

            Dictionary<Vector3, float> possibleMovments = new Dictionary<Vector3, float>();

            for (float i = currentPosition.x - 1; i <= currentPosition.x +1; i += .25f)
            {
                for (float j =  currentPosition.z - 1; j <=  currentPosition.z + 1; j += .25f)
                {
                    Vector3 possibleNextPosition = new Vector3(i, 0.5f, j);
                    float possibleNextPositionCost = calculateObstructionCostAroundNextCoord(possibleNextPosition, target.position);
                    possibleMovments.Add(possibleNextPosition, possibleNextPositionCost);
                }

            }

            // Use lynq query to sort our dictionary
            var sortedHeatMaps = from heatMapCoord in possibleMovments orderby heatMapCoord.Value ascending select heatMapCoord;

            // Draw our line 
            Debug.DrawLine(currentPosition, sortedHeatMaps.First().Key, Color.green, 10000f);

            // Move to new position
            this.transform.position = sortedHeatMaps.First().Key;

            // Set agent's current position after movement
            this.currentPosition = sortedHeatMaps.First().Key;

            timeSinceMovement = 0;
        }
	}

    float calculateObstructionCostAroundNextCoord(Vector3 possibleNextPos, Vector3 targetPos)
    {
        float costSum = 0;
        // |P(x,y,z) - Pgoal|
        costSum += Vector3.Magnitude(possibleNextPos - targetPos);

        foreach (GameObject obstacle in obstacles)
        {
            float obstacleForceFieldDistance = obstacle.GetComponent<CapsuleCollider>().radius * 2;

            Vector3 distanceFromForceField = obstacle.transform.position;

            float distanceFromObstacle = Vector3.Distance(possibleNextPos, obstacle.transform.position);

            if (distanceFromObstacle > 0 && distanceFromObstacle <= obstacleForceFieldDistance + this.GetComponent<SphereCollider>().radius)
            {
                costSum += Mathf.Log(obstacleForceFieldDistance + this.GetComponent<SphereCollider>().radius / distanceFromObstacle );
            }
        }

        return costSum;
    }
}
