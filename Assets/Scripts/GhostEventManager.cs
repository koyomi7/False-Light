using System.Collections;
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
    [SerializeField] RuntimeAnimatorController downstairsOfficeScareController;
    [SerializeField] AudioClip heavyBreathing;
    [SerializeField] AudioClip ghostRoar;
    [SerializeField] AudioClip ghostFootsteps;
    [SerializeField] float ghostSpeed;
    bool isGhostRunning = false;

    [Header("Downstairs Bathroom Scare")]
    [SerializeField] RuntimeAnimatorController downstairsBathroomScareController;
    [SerializeField] GenericAccessMechanismScript downstairsBathroomDoor;
    [SerializeField] AudioClip footstepsBehindDoor;
    [SerializeField] AudioClip doorSlamSound;
    [SerializeField] AnimationClip doorSlamClip;

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

    void ClearAudios()
    {
        audioSource1.clip = null;
        audioSource2.clip = null;
        audioSource3.clip = null;
    }

    public IEnumerator DownstairsOfficeScare(Occurrences occurrence)
    {
        switch (occurrence)
        {
            case Occurrences.Start:
                GameManager.Instance.StartEvent(1);
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsOfficeScareController;
                Ghost.transform.position = new Vector3(7.12599993f, 1.43995976f, 13.243f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(90f, 55.2999878f, 0f));
                Ghost.SetActive(true);
                animator.Play("Seizure");
                audioSource1.clip = heavyBreathing;
                audioSource1.Play();
                break;
            case Occurrences.End:
                Ghost.SetActive(false);
                audioSource1.Stop();
                audioSource2.Stop();
                yield return new WaitForSeconds(1f);

                // Reappear the ghost
                Ghost.transform.position = new Vector3(Ghost.transform.position.x, 0f, Ghost.transform.position.z);
                Ghost.transform.rotation = Quaternion.Euler(180, 180, 0);
                Ghost.SetActive(true);
                animator.Play("ClownRun");

                audioSource1.clip = ghostRoar;
                audioSource1.Play();
                audioSource2.clip = ghostFootsteps;
                audioSource2.loop = true;
                audioSource2.Play();

                isGhostRunning = true; // RunToPlayer() until the event ends
                break;
            default:
                Debug.Log($"Error: DownstairsOfficeScare() does not have a {occurrence} occurance");
                break;
        }
    }

    public IEnumerator DownstairsBathroomScare(Occurrences occurrence)
    {
        switch (occurrence)
        {
            case Occurrences.Both:
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsBathroomScareController;
                Ghost.transform.position = new Vector3(8.09600067f, 0.0820000172f, 9.92300034f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                Ghost.SetActive(true);
                if (downstairsBathroomDoor.state == GenericAccessMechanismScript.states.CLOSED)
                {
                    GameManager.Instance.StartEvent(2);

                    // Play the scary sound
                    audioSource1.clip = footstepsBehindDoor;
                    audioSource1.Play();

                    // Wait for the scary sound to finish 
                    yield return new WaitForSeconds(footstepsBehindDoor.length);

                    // Play the door slam sound
                    audioSource2.clip = doorSlamSound;
                    audioSource2.Play();

                    // Wait for door slam sound to finish
                    yield return new WaitForSeconds(doorSlamSound.length);

                    ClearAudios();
                    GameManager.Instance.EndEvent(2);
                }
                else
                {
                    GameManager.Instance.StartEvent(2);
                    animator.Play("GoofyRun");

                    // Play footsteps from ghost
                    audioSource1.clip = footstepsBehindDoor;
                    audioSource1.Play();

                    float runDuration = 1.5f;
                    float elapsedTime = 0f;
                    Vector3 startPos = Ghost.transform.position;
                    Vector3 targetPos = new Vector3(8.01299953f, 0f, 7.42399979f);
                    Vector3 targetPos2 = new Vector3(8.82499981f, 0f, 7.42399979f);

                    // Move ghost to door
                    while (elapsedTime < runDuration)
                    {
                        elapsedTime += Time.deltaTime;
                        float t = elapsedTime / runDuration;

                        // Calculate direction and rotation
                        Vector3 directionToDoor = (targetPos - Ghost.transform.position).normalized;
                        directionToDoor.y = 0;

                        if (directionToDoor != Vector3.zero)
                        {
                            Ghost.transform.forward = directionToDoor;
                        }

                        // Move ghost
                        Ghost.transform.position = Vector3.Lerp(startPos, targetPos, t);

                        yield return null;
                    }
                    Ghost.transform.forward = (targetPos2 - Ghost.transform.position).normalized;

                    // Wait extra second before destroying ghost
                    yield return new WaitForSeconds(0.8f);

                    // Close door and play door slam sound
                    AnimationClip temp = downstairsBathroomDoor.openClip;
                    downstairsBathroomDoor.overrideController["OPEN"] = doorSlamClip;
                    downstairsBathroomDoor.Close();
                    yield return new WaitForSeconds(doorSlamClip.length); // Wait for door animation to start
                    audioSource2.clip = doorSlamSound;
                    audioSource2.Play();
                    Ghost.transform.position = targetPos;
                    animator.Play("Idle");
                    audioSource1.Stop();
                    yield return new WaitForSeconds(doorSlamSound.length);
                    Ghost.SetActive(false);
                    
                    audioSource2.Stop();
                    downstairsBathroomDoor.overrideController["OPEN"] = temp;

                    ClearAudios();
                    GameManager.Instance.EndEvent(2);
                }
                break;
            default:
                Debug.Log($"Error: DownstairsBathroomScare() does not have a {occurrence} occurance");
                break;
        }
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

            ClearAudios();
            GameManager.Instance.EndEvent(1);
        }
    }
}
