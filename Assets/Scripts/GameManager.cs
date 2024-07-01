using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GhostMode
{
    CHASE,
    SCATTER
}

public class GameManager : MonoBehaviour
{
    public GameObject pacman;
    public GameObject rightWarpNode;
    public GameObject leftWarpNode;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;
    public Sprite frightnedGhost;
    public AudioSource siren;
    public AudioSource munch1;
    public AudioSource munch2;
    public AudioSource death;
    private int currentMunch = 0;

    public GhostMode currentGhostMode;

    public GameObject redGhost;
    public GameObject blueGhost;
    public GameObject pinkGhost;
    public GameObject orangeGhost;

    private int score;
    public TMP_Text scoreText;

    public int collectedPelletCount;

    // Start is called before the first frame update
    void Awake()
    {
        redGhost.GetComponent<EnemyController>().readyToLeave = true;
        pinkGhost.GetComponent<EnemyController>().readyToLeave = true;
        blueGhost.GetComponent<EnemyController>().readyToLeave = false;
        orangeGhost.GetComponent<EnemyController>().readyToLeave = false;

        ghostNodeStart.GetComponent<NodeController>().isGhostStartNode = true;
        pacman = GameObject.Find("Player");
        score = 0;
        collectedPelletCount = 0;
        currentMunch = 0;
        scoreText.text = "0";
        currentGhostMode = GhostMode.CHASE;
        siren.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void IncrementScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    private void IncrementCollectedPelletCount()
    {
        collectedPelletCount++;
    }

    public void CollectedPellet(NodeController controller)
    {
        if (currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1;
        }
        else if (currentMunch == 1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        IncrementScore(10);
        IncrementCollectedPelletCount();

        string pelletTag = controller.GetComponentInChildren<SpriteRenderer>().gameObject.tag;

        if (pelletTag == "PowerPellet")
        {

        }
    }

    public IEnumerator PlayerEaten()
    {
        PlayerController playerController = pacman.GetComponent<PlayerController>();

        siren.Stop();
        playerController.Stop();
        redGhost.GetComponent<EnemyController>().Stop();
        orangeGhost.GetComponent<EnemyController>().Stop();
        pinkGhost.GetComponent<EnemyController>().Stop();
        blueGhost.GetComponent<EnemyController>().Stop();
        yield return new WaitForSeconds(0.5f);
        death.Play();
        yield return new WaitForSeconds(1);
        playerController.Death();
        yield return new WaitForSeconds(0.2f);
        pacman.SetActive(false);
    }
}
