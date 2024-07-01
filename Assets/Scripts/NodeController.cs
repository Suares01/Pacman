using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum NodeDirection
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
    STOPPED
}

public class NodeController : MonoBehaviour
{
    public bool canMoveLeft = false;
    public bool canMoveRight = false;
    public bool canMoveUp = false;
    public bool canMoveDown = false;
    public bool isWarpLeftNode = false;
    public bool isWarpRightNode = false;
    public bool isPelletNode = false;
    public bool hasPellet = false;
    public bool isGhostStartNode = false;
    public bool isSideNode = false;

    public GameObject nodeRight;
    public GameObject nodeLeft;
    public GameObject nodeUp;
    public GameObject nodeDown;
    private SpriteRenderer pelletSprite;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (transform.childCount > 0)
        {
            isPelletNode = true;
            hasPellet = true;
            pelletSprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (gameObject.name == "RightWarpNode")
            isWarpRightNode = true;
        else if (gameObject.name == "LeftWarpNode")
            isWarpLeftNode = true;

        RaycastHit2D[] hitsDown = Physics2D.RaycastAll(transform.position, Vector2.down);
        RaycastHit2D[] hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up);
        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(transform.position, Vector2.left);
        RaycastHit2D[] hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right);

        for (int i = 0; i < hitsDown.Length; i++)
        {
            float distance = Mathf.Abs(hitsDown[i].point.y - transform.position.y);

            if (distance < 0.4f && hitsDown[i].collider.tag == "Node")
            {
                canMoveDown = true;
                nodeDown = hitsDown[i].collider.gameObject;
            }
        }

        for (int i = 0; i < hitsUp.Length; i++)
        {
            float distance = Mathf.Abs(hitsUp[i].point.y - transform.position.y);

            if (distance < 0.4f && hitsUp[i].collider.tag == "Node")
            {
                canMoveUp = true;
                nodeUp = hitsUp[i].collider.gameObject;
            }
        }

        for (int i = 0; i < hitsLeft.Length; i++)
        {
            float distance = Mathf.Abs(hitsLeft[i].point.x - transform.position.x);

            if (distance < 0.4f && hitsLeft[i].collider.tag == "Node")
            {
                canMoveLeft = true;
                nodeLeft = hitsLeft[i].collider.gameObject;
            }
        }

        for (int i = 0; i < hitsRight.Length; i++)
        {
            float distance = Mathf.Abs(hitsRight[i].point.x - transform.position.x);

            if (distance < 0.4f && hitsRight[i].collider.tag == "Node")
            {
                canMoveRight = true;
                nodeRight = hitsRight[i].collider.gameObject;
            }
        }

        if (isGhostStartNode)
        {
            canMoveDown = true;
            nodeDown = gameManager.ghostNodeCenter;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject GetNodeFromDirection(NodeDirection direction)
    {
        if (direction == NodeDirection.LEFT && canMoveLeft)
            return nodeLeft;

        if (direction == NodeDirection.RIGHT && canMoveRight)
            return nodeRight;

        if (direction == NodeDirection.UP && canMoveUp)
            return nodeUp;

        if (direction == NodeDirection.DOWN && canMoveDown)
            return nodeDown;

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player" && isPelletNode && hasPellet)
        {
            hasPellet = false;
            pelletSprite.enabled = false;
            gameManager.CollectedPellet(this);
        }
    }
}
