using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GhostState
{
    RESPAWNING,
    LEFT_NODE,
    RIGHT_NODE,
    CENTER_NODE,
    START_NODE,
    MOVING_IN
}

public enum GhostType
{
    RED,
    BLUE,
    PINK,
    ORANGE
}

public class EnemyController : MonoBehaviour
{
    public GhostState state;
    private GhostState respawnState;
    public GhostType type;

    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject nodeCenter;
    public GameObject nodeStart;
    private GameObject startingNode;

    private MovementController movementController;
    private GameManager gameManager;

    public bool readyToLeave = false;
    public bool hasCountToLeave = false;
    public bool testRespawn = false;
    public bool isFrightned = false;
    public bool stop = false;
    public GameObject[] scatterNodes;
    public int scatterNodesIndex;
    private const float distanceBetweenNodes = 0.35f;

    // Start is called before the first frame update
    void Awake()
    {
        scatterNodesIndex = 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();

        switch (type)
        {
            case GhostType.RED:
                state = GhostState.START_NODE;
                respawnState = GhostState.CENTER_NODE;
                startingNode = nodeStart;
                break;

            case GhostType.BLUE:
                state = GhostState.CENTER_NODE;
                respawnState = GhostState.CENTER_NODE;
                startingNode = nodeCenter;
                break;

            case GhostType.ORANGE:
                state = GhostState.RIGHT_NODE;
                respawnState = GhostState.RIGHT_NODE;
                startingNode = nodeRight;
                break;

            case GhostType.PINK:
                state = GhostState.LEFT_NODE;
                respawnState = GhostState.LEFT_NODE;
                startingNode = nodeLeft;
                break;
        }

        transform.position = startingNode.transform.position;
        movementController.currentNode = startingNode;
        movementController.SetDirection(NodeDirection.STOPPED);
    }

    // Update is called once per frame
    void Update()
    {
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (testRespawn)
        {
            readyToLeave = false;
            state = GhostState.RESPAWNING;
            testRespawn = false;
        }

        if (movementController.direction == NodeDirection.STOPPED && !hasCountToLeave && ((type == GhostType.BLUE && gameManager.collectedPelletCount == 30) || (type == GhostType.ORANGE && gameManager.collectedPelletCount == 60)))
        {
            readyToLeave = true;
            hasCountToLeave = true;
        }

        if (nodeController.isSideNode)
        {
            movementController.SetSpeed(1);
        }
        else
        {
            movementController.SetSpeed(1.5f);
        }
    }

    private NodeDirection GetRandomDirection(NodeController controller)
    {
        List<NodeDirection> possibleDirections = new List<NodeDirection>();

        if (controller.canMoveDown && movementController.lastMovingDirection != NodeDirection.UP)
            possibleDirections.Add(NodeDirection.DOWN);
        if (controller.canMoveUp && movementController.lastMovingDirection != NodeDirection.DOWN)
            possibleDirections.Add(NodeDirection.UP);
        if (controller.canMoveLeft && movementController.lastMovingDirection != NodeDirection.RIGHT)
            possibleDirections.Add(NodeDirection.LEFT);
        if (controller.canMoveRight && movementController.lastMovingDirection != NodeDirection.LEFT)
            possibleDirections.Add(NodeDirection.RIGHT);

        if (possibleDirections.Count > 0)
        {
            int randomDirectionIndex = Random.Range(0, possibleDirections.Count - 1);
            NodeDirection direction = possibleDirections[randomDirectionIndex];

            return direction;
        }
        else
            return movementController.direction;
    }

    private void MoveInScatterMode(NodeController controller)
    {
        GameObject scatterNode = scatterNodes[scatterNodesIndex];

        if (transform.position.x == scatterNode.transform.position.x && transform.position.y == scatterNode.transform.position.y)
        {
            scatterNodesIndex++;

            if (scatterNodesIndex == scatterNodes.Length - 1)
            {
                scatterNodesIndex = 0;
            }
        }

        NodeDirection direction = GetClosestDirection(scatterNode.transform.position, controller);

        movementController.SetDirection(direction);
    }

    public void ReachedCenterOfNode(NodeController controller)
    {
        if (state == GhostState.MOVING_IN)
        {
            if (gameManager.currentGhostMode == GhostMode.SCATTER)
            {
                MoveInScatterMode(controller);
            }
            else if (isFrightned)
            {
                NodeDirection direction = GetRandomDirection(controller);
                movementController.SetDirection(direction);
            }
            else
            {
                if (type == GhostType.RED)
                {
                    DetermineRedGhostDirection(controller);
                }
                else if (type == GhostType.PINK)
                {
                    DeterminePinkGhostDirection(controller);
                }
                else if (type == GhostType.BLUE)
                {
                    DetermineBlueGhostDirection(controller);
                }
                else if (type == GhostType.ORANGE)
                {
                    DetermineOrangeGhostDirection(controller);
                }
            }

        }
        else if (state == GhostState.RESPAWNING)
        {
            NodeDirection direction = NodeDirection.STOPPED;

            if (transform.position.x == nodeStart.transform.position.x && transform.position.y == nodeStart.transform.position.y)
                direction = NodeDirection.DOWN;
            else if (transform.position.x == nodeCenter.transform.position.x && transform.position.y == nodeCenter.transform.position.y)
            {
                if (respawnState == GhostState.CENTER_NODE)
                    state = respawnState;
                else if (respawnState == GhostState.LEFT_NODE)
                    direction = NodeDirection.LEFT;
                else if (respawnState == GhostState.RIGHT_NODE)
                    direction = NodeDirection.RIGHT;
            }
            else if ((transform.position.x == nodeLeft.transform.position.x && transform.position.y == nodeLeft.transform.position.y) || (transform.position.x == nodeRight.transform.position.x && transform.position.y == nodeRight.transform.position.y))
                state = respawnState;
            else
                direction = GetClosestDirection(nodeStart.transform.position, controller);

            movementController.SetDirection(direction);
        }
        else
        {
            if (readyToLeave)
            {
                if (state == GhostState.LEFT_NODE)
                {
                    state = GhostState.CENTER_NODE;
                    movementController.SetDirection(NodeDirection.RIGHT);
                }
                else if (state == GhostState.RIGHT_NODE)
                {
                    state = GhostState.CENTER_NODE;
                    movementController.SetDirection(NodeDirection.LEFT);
                }
                else if (state == GhostState.CENTER_NODE)
                {
                    state = GhostState.START_NODE;
                    movementController.SetDirection(NodeDirection.UP);
                }
                else if (state == GhostState.START_NODE)
                {
                    state = GhostState.MOVING_IN;
                    movementController.SetDirection(NodeDirection.LEFT);
                }
            }
        }
    }

    private void DetermineRedGhostDirection(NodeController controller)
    {
        NodeDirection direction = GetClosestDirection(gameManager.pacman.transform.position, controller);
        movementController.SetDirection(direction);
    }

    private void DeterminePinkGhostDirection(NodeController controller)
    {
        NodeDirection pacmanDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;

        Vector2 target = gameManager.pacman.transform.position;

        if (pacmanDirection == NodeDirection.LEFT)
            target.x -= distanceBetweenNodes * 2;
        else if (pacmanDirection == NodeDirection.RIGHT)
            target.x += distanceBetweenNodes * 2;
        else if (pacmanDirection == NodeDirection.UP)
            target.y += distanceBetweenNodes * 2;
        else if (pacmanDirection == NodeDirection.DOWN)
            target.y -= distanceBetweenNodes * 2;

        NodeDirection direction = GetClosestDirection(target, controller);
        movementController.SetDirection(direction);
    }

    private void DetermineBlueGhostDirection(NodeController controller)
    {
        NodeDirection pacmanDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;

        Vector2 target = gameManager.pacman.transform.position;

        if (pacmanDirection == NodeDirection.LEFT)
            target.x -= distanceBetweenNodes * 2;
        else if (pacmanDirection == NodeDirection.RIGHT)
            target.x += distanceBetweenNodes * 2;
        else if (pacmanDirection == NodeDirection.UP)
            target.y += distanceBetweenNodes * 2;
        else if (pacmanDirection == NodeDirection.DOWN)
            target.y -= distanceBetweenNodes * 2;

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);
        NodeDirection direction = GetClosestDirection(blueTarget, controller);
        movementController.SetDirection(direction);
    }

    private void DetermineOrangeGhostDirection(NodeController controller)
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);

        if (distance < 0)
            distance *= -1;

        if (distance <= distanceBetweenNodes * 8)
        {
            DetermineRedGhostDirection(controller);
        }
        else
        {
            MoveInScatterMode(controller);
        }
    }

    private NodeDirection GetClosestDirection(Vector2 target, NodeController controller)
    {
        float shortestDistance = 0;
        NodeDirection lastMovingDirection = movementController.lastMovingDirection;
        NodeDirection newDirection = NodeDirection.STOPPED;

        if (controller.canMoveUp && lastMovingDirection != NodeDirection.DOWN)
        {
            GameObject nodeUp = controller.nodeUp;
            float distance = Vector2.Distance(nodeUp.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = NodeDirection.UP;
            }
        }

        if (controller.canMoveDown && lastMovingDirection != NodeDirection.UP)
        {
            GameObject nodeDown = controller.nodeDown;
            float distance = Vector2.Distance(nodeDown.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = NodeDirection.DOWN;
            }
        }

        if (controller.canMoveLeft && lastMovingDirection != NodeDirection.RIGHT)
        {
            GameObject nodeLeft = controller.nodeLeft;
            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = NodeDirection.LEFT;
            }
        }

        if (controller.canMoveRight && lastMovingDirection != NodeDirection.LEFT)
        {
            GameObject nodeRight = controller.nodeRight;
            float distance = Vector2.Distance(nodeRight.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                newDirection = NodeDirection.RIGHT;
            }
        }

        return newDirection;
    }

    public void Stop()
    {
        movementController.stop = true;
        movementController.SetDirection(NodeDirection.STOPPED);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (isFrightned)
            {

            }
            else
            {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }
}
