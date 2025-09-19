using System.Collections;
using UnityEngine;

public class GhostEventManager : MonoBehaviour
{
    public static GhostEventManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] GameObject Ghost;
    [SerializeField] Transform Player;
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] AudioSource audioSource3;
    [SerializeField] AudioSource auxiliaryAudioSource;
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

    [Header("Downstairs Bedroom Scare")]
    [SerializeField] RuntimeAnimatorController downstairsBedroomScareController;
    [SerializeField] GameObject downstairsBedroomPill;
    [SerializeField] AudioClip fleshSoundEffect;
    [SerializeField] AudioClip runAndHitSound;
    [SerializeField] GameObject blood;
    [HideInInspector] public bool isSlowGettingUpFinished = false;
    [HideInInspector] public bool isRunAndHitFinished = false;
    [HideInInspector] public bool isCrawlBackFinished = false;

    [Header("Downstairs Living Room Scare")]
    [SerializeField] RuntimeAnimatorController downstairsLivingRoomScareController;
    [SerializeField] AudioClip glassBreak;
    [SerializeField] GameObject spotLight;
    [HideInInspector] public bool isFallingFinished = false;
    [HideInInspector] public bool isVanishFinished = false;

    [Header("Downstairs Hallway Scare")]
    [SerializeField] RuntimeAnimatorController downstairsHallwayScareController;
    [SerializeField] GenericAccessMechanismScript downstairsHallwayDoor;
    [SerializeField] AudioClip downstairsHallwayScareBreathingSound;
    [SerializeField] AudioClip downstairsHallwayScareFootstepSound;
    [SerializeField] AudioClip downstairsHallwayScareDoorSlamSound;
    [SerializeField] Animator downstairsHallwayScareDoorAnimator;
    [HideInInspector] public bool isWalkingBackFinished = false;

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
        ResetAll();
    }

    void Update()
    {
        RunToPlayer(); // DownstairsOfficeScare
    }

    void ResetAll()
    {
        audioSource1.Stop();
        audioSource2.Stop();
        audioSource3.Stop();
        auxiliaryAudioSource.Stop();
        audioSource1.clip = null;
        audioSource2.clip = null;
        audioSource3.clip = null;
        auxiliaryAudioSource.clip = null;
        Ghost.GetComponent<Animator>().runtimeAnimatorController = null;
        animator.applyRootMotion = true;
        animator.speed = 1;
        Ghost.SetActive(false);
        Ghost.transform.position = new Vector3(0f, 0f, 0f);
        Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        Ghost.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public IEnumerator DownstairsOfficeScare(int occurrence)
    {
        switch (occurrence)
        {
            case 1: // Start
                GameManager.Instance.StartEvent(1);
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsOfficeScareController;
                Ghost.transform.position = new Vector3(7.12599993f, 1.43995976f, 13.243f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(90f, 55.2999878f, 0f));
                Ghost.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
                Ghost.SetActive(true);
                animator.Play("Seizure");
                audioSource1.clip = heavyBreathing;
                audioSource1.Play();
                GameManager.Instance.NextEventReady();
                break;
            case 2: // End
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
                Debug.Log($"Error: DownstairsOfficeScare() does not have a {occurrence} occurrence");
                break;
        }
    }

    public IEnumerator DownstairsBathroomScare(int occurrence)
    {
        switch (occurrence)
        {
            case 1:
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsBathroomScareController;
                Ghost.transform.position = new Vector3(8.09600067f, 0.0820000172f, 9.92300034f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                Ghost.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
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
                    ResetAll();
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
                    downstairsBathroomDoor.overrideController["OPEN"] = temp;
                    ResetAll();
                    GameManager.Instance.EndEvent(2);
                }
                break;
            default:
                Debug.Log($"Error: DownstairsBathroomScare() does not have a {occurrence} occurrence");
                break;
        }
    }

    public IEnumerator DownstairsBedroomScare(int occurrence)
    {
        switch (occurrence)
        {
            case 1:
                GameManager.Instance.StartEvent(3);
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsBedroomScareController;
                Ghost.transform.position = new Vector3(2.88400006f, 0.578959823f, 9.03600025f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
                Ghost.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Ghost.SetActive(true);
                animator.Play("SlowGetUp");
                audioSource1.clip = fleshSoundEffect;
                audioSource1.Play();
                yield return new WaitUntil(() => isSlowGettingUpFinished);
                audioSource1.Stop();
                Ghost.transform.position = new Vector3(2.97500014f, 0.634959817f, 8.77799988f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 62.3800011f, 0f));
                Ghost.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
                animator.Play("RunAndHit");
                audioSource2.clip = runAndHitSound;
                audioSource2.Play();
                yield return new WaitUntil(() => isRunAndHitFinished);
                blood.SetActive(true);
                ResetAll();
                yield return new WaitUntil(() => downstairsBedroomPill == null);
                Ghost.transform.position = new Vector3(3.79900002f, 0.150000006f, 8.92000008f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 63.52f, 0f));
                Ghost.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Ghost.SetActive(true);
                animator.Play("BedSit");
                GameManager.Instance.NextEventReady();
                break;
            case 2:
                Ghost.transform.position = new Vector3(3.26799989f, 0.550000012f, 8.78999996f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 55f, 0f));
                Ghost.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                animator.Play("CrawlBack");
                yield return new WaitUntil(() => isCrawlBackFinished);
                isCrawlBackFinished = false;
                ResetAll();
                GameManager.Instance.EndEvent(3);
                break;
            default:
                Debug.Log($"Error: DownstairsBedroomScare() does not have a {occurrence} occurrence");
                break;
        }
    }
    
    public IEnumerator DownstairsLivingRoomScare(int occurrence)
    {
        switch (occurrence)
        {
            case 1:
                GameManager.Instance.StartEvent(4);
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsLivingRoomScareController;
                auxiliaryAudioSource.transform.position = new Vector3(7.48999977f, 3.58100009f, 19.1130009f);
                auxiliaryAudioSource.clip = glassBreak;
                auxiliaryAudioSource.Play();
                GameManager.Instance.NextEventReady();
                break;
            case 2:
                Ghost.transform.position = new Vector3(7.53399992f, 0.397959799f, 19.6789989f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 283.464966f, 0f));
                Ghost.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
                Ghost.SetActive(true);
                animator.Play("FallingOnImpact");
                yield return new WaitUntil(() => isFallingFinished);
                Ghost.transform.position = new Vector3(11.6700001f, -0.0520402193f, 14.6309986f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
                Ghost.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
                animator.applyRootMotion = false;
                spotLight.SetActive(true);
                animator.Play("Trapped");
                GameManager.Instance.NextEventReady();
                break;
            case 3:
                animator.applyRootMotion = true;
                animator.Play("Vanish");
                yield return new WaitUntil(() => isVanishFinished);
                spotLight.SetActive(false);
                ResetAll();
                GameManager.Instance.EndEvent(4);
                break;
            default:
                Debug.Log($"Error: DownstairsLivingRoomScare() does not have a {occurrence} occurrence");
                break;
        }
    }

    public IEnumerator DownstairsHallwayScare(int occurrence)
    {
        switch (occurrence)
        {
            case 1:
                GameManager.Instance.StartEvent(5);
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsHallwayScareController;
                if (downstairsHallwayDoor.state == GenericAccessMechanismScript.states.OPEN)
                {
                    Ghost.transform.position = new Vector3(5.47200012f, 0.0850000009f, 4.9380002f);
                    Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                    Ghost.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Ghost.SetActive(true);
                    animator.speed = 0;
                    animator.Play("FastCrawl");
                    audioSource1.clip = downstairsHallwayScareBreathingSound;
                    audioSource1.Play();
                    GameManager.Instance.NextEventReady();
                }
                else if (downstairsHallwayDoor.state == GenericAccessMechanismScript.states.CLOSED)
                {
                    Ghost.transform.position = new Vector3(10.3109035f, 1.47300005f, 13.0962687f);
                    Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 242.046951f, 0f));
                    Ghost.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Ghost.SetActive(true);
                    animator.speed = 0;
                    animator.applyRootMotion = false;
                    animator.Play("WalkingBack");
                    audioSource1.clip = downstairsHallwayScareBreathingSound;
                    audioSource1.Play();
                    GameManager.Instance.NextEventReady(2); // skip to case 3
                }
                break;
            case 2: // Downstairs office door open -> crawls to ghost room
                animator.speed = 1;
                audioSource2.clip = downstairsHallwayScareFootstepSound;
                audioSource2.Play();
                downstairsHallwayScareDoorAnimator.Play("Open");
                yield return new WaitForSeconds(2.1f);
                audioSource1.Stop();
                audioSource2.Stop();
                Ghost.SetActive(false);
                auxiliaryAudioSource.transform.position = new Vector3(8.7173996f, 1.21599996f, 4.99700022f);
                auxiliaryAudioSource.clip = downstairsHallwayScareDoorSlamSound;
                auxiliaryAudioSource.Play();
                downstairsHallwayScareDoorAnimator.Play("Slam");
                yield return new WaitForSeconds(0.3f);
                ResetAll();
                GameManager.Instance.EndEvent(5);
                break;
            case 3: // Downstairs office door closed -> appears at stairs and walks backwards
                animator.speed = 1;
                audioSource2.clip = downstairsHallwayScareFootstepSound;
                audioSource2.Play();
                yield return new WaitUntil(() => isWalkingBackFinished);
                ResetAll();
                GameManager.Instance.EndEvent(5);
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
            ResetAll();
            GameManager.Instance.EndEvent(1);
        }
    }
}
