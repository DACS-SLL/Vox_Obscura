using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;

    [Header("Visual")]
    public GameObject visual;

    [Header("Activación por proximidad")]
    public float triggerRadius = 3f;
    private Transform player;

    [Header("Movimiento Inicial (Escape)")]
    public float escapeSpeed = 3f;
    public float escapeDistance = 12f;

    [Header("Patrulla")]
    public float patrolSpeed = 1.5f;
    public float xMin = -10.89f;
    public float xMax = -1.2f;
    public float waitMin = 6f;
    public float waitMax = 15f;

    private Animator anim;
    private bool hasSpawned = false;
    private bool hasEscaped = false;
    private bool patrolling = false;

    void Awake()
    {
        Instance = this;
        Debug.Log("EnemyController: Awake");
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;

        anim = GetComponentInChildren<Animator>();
        visual.SetActive(false);

        Collider myCollider = GetComponent<Collider>();

        foreach (Collider col in FindObjectsByType<Collider>(FindObjectsSortMode.None))
        {
            if (col == myCollider) continue;
            if (col.CompareTag("Wood")) continue;
            if (col.CompareTag("Player")) continue;
            if (col.CompareTag("Concrete")) continue;

            Physics.IgnoreCollision(myCollider, col, true);
        }
    }

    public void SpawnEnemy()
    {
        if (hasSpawned)
        {
            return;
        }

        hasSpawned = true;
        visual.SetActive(true);
    }

    void Update()
    {
        if (!hasSpawned)
        {
            return;
        }

        if (hasEscaped)
            return;

        if (player == null)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= triggerRadius)
        {
            StartCoroutine(InitialEscape());
            hasEscaped = true;
        }
    }

    IEnumerator InitialEscape()
    {

        transform.rotation = Quaternion.Euler(0, 270f, 0);

        float moved = 0f;

        while (moved < escapeDistance)
        {
            float step = escapeSpeed * Time.deltaTime;
            transform.position += Vector3.left * step;
            moved += step;

            yield return null;
        }

        yield return new WaitForSeconds(6f);

        StartPatrol();
    }

    public void StartPatrol()
    {
        if (patrolling)
        {
            return;
        }

        StartCoroutine(PatrolRoutineFixed());
    }


    IEnumerator PatrolRoutineFixed()
    {
        patrolling = true;

        float leftLimit = -10.90f;
        float rightLimit = -0.83f;
        float midPoint = (leftLimit + rightLimit) / 2f;

        while (NoteProgress.Instance.notesFound < 6)
        {

            Vector3 pos = transform.position;
            pos.z = player.position.z;
            transform.position = pos;

            float x = transform.position.x;
            float dir = 0f;

            if (x <= leftLimit)
            {
                dir = +1f;
                transform.rotation = Quaternion.Euler(0, 90f, 0);
            }
            else if (x >= rightLimit)
            {
                dir = -1f;
                transform.rotation = Quaternion.Euler(0, -90f, 0);
            }
            else
            {
                if (x < midPoint)
                {
                    dir = +1f;
                    transform.rotation = Quaternion.Euler(0, 90f, 0);
                }
                else
                {
                    dir = -1f;
                    transform.rotation = Quaternion.Euler(0, -90f, 0);
                }
            }


            float moved = 0f;
            float moveDistance = 10f;

            while (moved < moveDistance)
            {
                float step = patrolSpeed * Time.deltaTime;
                transform.position += new Vector3(dir * step, 0, 0);
                moved += step;

                yield return null;
            }

            yield return new WaitForSeconds(2f);
        }

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SanitySystem.Instance.ReduceSanity(15);
        }
    }
}
