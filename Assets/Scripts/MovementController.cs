using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private GameManager gameManager;
    private EnemyController enemyController;
    public GameObject currentNode;
    public float speed = 4f;
    public NodeDirection direction = NodeDirection.STOPPED;
    public NodeDirection lastMovingDirection = NodeDirection.STOPPED;

    private bool canWarp = true;
    public bool isGhost = false;
    public bool stop = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        lastMovingDirection = NodeDirection.STOPPED;

        if (isGhost)
            enemyController = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        NodeController currentNodeController = currentNode.GetComponent<NodeController>();

        if (!stop)
            transform.position = Vector2.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);

        if (IsInNodeCenter() || IsReversingDirection())
        {
            if (isGhost)
                enemyController.ReachedCenterOfNode(currentNodeController);

            if (currentNodeController.isWarpLeftNode && canWarp)
            {
                currentNode = gameManager.rightWarpNode;
                direction = NodeDirection.LEFT;
                lastMovingDirection = NodeDirection.LEFT;
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            else if (currentNodeController.isWarpRightNode && canWarp)
            {
                currentNode = gameManager.leftWarpNode;
                direction = NodeDirection.RIGHT;
                lastMovingDirection = NodeDirection.RIGHT;
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            else
            {
                if (currentNodeController.isGhostStartNode && direction == NodeDirection.DOWN && (!isGhost || enemyController.state != GhostState.RESPAWNING))
                    direction = lastMovingDirection;

                GameObject newNode = currentNodeController.GetNodeFromDirection(direction);

                if (newNode != null)
                {
                    currentNode = newNode;
                    lastMovingDirection = direction;
                }
                else
                {
                    direction = lastMovingDirection;
                    newNode = currentNodeController.GetNodeFromDirection(direction);

                    if (newNode != null)
                        currentNode = newNode;
                }
            }
        }
        else
        {
            canWarp = true;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDirection(NodeDirection newDirection)
    {
        direction = newDirection;
    }

    private bool IsReversingDirection()
    {
        return (direction == NodeDirection.LEFT && lastMovingDirection == NodeDirection.RIGHT) ||
                (direction == NodeDirection.RIGHT && lastMovingDirection == NodeDirection.LEFT) ||
                (direction == NodeDirection.UP && lastMovingDirection == NodeDirection.DOWN) ||
                (direction == NodeDirection.DOWN && lastMovingDirection == NodeDirection.UP);
    }

    private bool IsInNodeCenter()
    {
        return transform.position.x == currentNode.transform.position.x && transform.position.y == currentNode.transform.position.y;
    }
}
