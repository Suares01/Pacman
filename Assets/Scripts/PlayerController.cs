using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private Animator animator;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        movementController = GetComponent<MovementController>();
        movementController.lastMovingDirection = NodeDirection.LEFT;
    }

    public void Stop()
    {
        animator.SetBool("moving", false);
        movementController.stop = true;
        movementController.SetDirection(NodeDirection.STOPPED);
    }

    // Update is called once per frame
    void Update()
    {
        if (!movementController.stop)
            animator.SetBool("moving", true);

        if (Input.GetKey(KeyCode.LeftArrow))
            movementController.SetDirection(NodeDirection.LEFT);
        else if (Input.GetKey(KeyCode.RightArrow))
            movementController.SetDirection(NodeDirection.RIGHT);
        else if (Input.GetKey(KeyCode.UpArrow))
            movementController.SetDirection(NodeDirection.UP);
        else if (Input.GetKey(KeyCode.DownArrow))
            movementController.SetDirection(NodeDirection.DOWN);

        bool flipX = false;
        bool flipY = false;

        if (movementController.lastMovingDirection == NodeDirection.LEFT)
            animator.SetInteger("direction", 0);
        else if (movementController.lastMovingDirection == NodeDirection.RIGHT)
        {
            animator.SetInteger("direction", 0);
            flipX = true;
        }
        else if (movementController.lastMovingDirection == NodeDirection.UP)
            animator.SetInteger("direction", 1);
        else if (movementController.lastMovingDirection == NodeDirection.DOWN)
        {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipX = flipX;
        sprite.flipY = flipY;
    }

    public void Death()
    {
        animator.SetBool("death", true);
    }
}
