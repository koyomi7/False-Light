using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEvent : MonoBehaviour
{
    public static GhostEvent Instance { get; private set; }

    [Header("References")]
    [SerializeField] protected GameObject Ghost;
    [SerializeField] protected Transform Player;
    protected Animator animator;
    protected AudioSource audioSource;

    // Stuff that I need to figure out how to make an object for to reference as a child
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip HeavyBreathing;
    [SerializeField] private AudioClip GhostRoar;
    [SerializeField] private AudioClip GhostFootsteps;
    [SerializeField] private float ghostSpeed;
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
        audioSource = Ghost.GetComponent<AudioSource>();
        Ghost.SetActive(false);
    }

    void Update()
    {
        RunToPlayer();
    }

    public void DownstairsOfficeScareStart()
    {
        Ghost.SetActive(true);
        animator.Play("Seizure");
        audioSource.clip = HeavyBreathing;
        audioSource.Play();
    }

    public void DownstairsOfficeScareEnd()
    {
        Ghost.SetActive(false);
        audioSource.Stop();
        footstepAudioSource.Stop(); 
        StartCoroutine(ReappearAndRun());
    }

    IEnumerator ReappearAndRun()
    {
        yield return new WaitForSeconds(1f); 

        // Reappear the ghost
        Ghost.transform.position = new Vector3(Ghost.transform.position.x, 0f, Ghost.transform.position.z);
        Ghost.transform.rotation = Quaternion.Euler(180, 180, 0);
        Ghost.SetActive(true);
        animator.Play("ClownRun");

        audioSource.clip = GhostRoar;
        audioSource.Play();
        footstepAudioSource.clip = GhostFootsteps;
        footstepAudioSource.loop = true;
        footstepAudioSource.Play();

        isGhostRunning = true;
    }

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
            audioSource.Stop();
            footstepAudioSource.Stop();
            Ghost.SetActive(false);
        }
    }
}
