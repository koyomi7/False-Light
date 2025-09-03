using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEventManager : MonoBehaviour
{
    public static GhostEventManager Instance { get; private set; }

    public enum Occurrences
    {
        Start,
        End,
        Both
    }

    [Header("References")]
    [SerializeField] GameObject Ghost;
    [SerializeField] Transform Player;
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] AudioSource audioSource3;
    Animator animator;

    [Header("Downstairs Office Scare")]
    [SerializeField] AudioClip HeavyBreathing;
    [SerializeField] AudioClip GhostRoar;
    [SerializeField] AudioClip GhostFootsteps;
    [SerializeField] float ghostSpeed;
    bool isGhostRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        animator = Ghost.GetComponent<Animator>();
        audioSource1.Stop();
        audioSource2.Stop();
        audioSource3.Stop();
        Ghost.SetActive(false);
    }

    void Update()
    {
        RunToPlayer(); // DownstairsOfficeScare
    }

    public void DownstairsOfficeScare(Occurrences occurrence)
    {
        switch (occurrence)
        {
            case Occurrences.Start:
                Ghost.SetActive(true);
                animator.Play("Seizure");
                audioSource1.clip = HeavyBreathing;
                audioSource1.Play();
                break;
            case Occurrences.End:
                Ghost.SetActive(false);
                audioSource1.Stop();
                audioSource2.Stop();
                StartCoroutine(ReappearAndRun());
                break;
            default:
                Debug.Log($"Error: DownstairsOfficeScare() does not have a {occurrence} occurance");
                break;
        }
    }

    // DownstairsOfficeScare
    IEnumerator ReappearAndRun()
    {
        yield return new WaitForSeconds(1f); 

        // Reappear the ghost
        Ghost.transform.position = new Vector3(Ghost.transform.position.x, 0f, Ghost.transform.position.z);
        Ghost.transform.rotation = Quaternion.Euler(180, 180, 0);
        Ghost.SetActive(true);
        animator.Play("ClownRun");

        audioSource1.clip = GhostRoar;
        audioSource1.Play();
        audioSource2.clip = GhostFootsteps;
        audioSource2.loop = true;
        audioSource2.Play();

        isGhostRunning = true;
    }

    // DownstairsOfficeScare
    void RunToPlayer()
    {
        if (!isGhostRunning) return;
        Vector3 direction = (Player.position - Ghost.transform.position).normalized;
        direction.y = 0; // Prevent ghost from moving up or down
        Ghost.transform.position += direction * ghostSpeed * Time.deltaTime;
        Ghost.transform.LookAt(Player);

        if (Vector3.Distance(Ghost.transform.position, Player.position) < 0.5f)
        {
            isGhostRunning = false;
            audioSource1.Stop();
            audioSource2.Stop();
            Ghost.SetActive(false);
        }
    }
}
