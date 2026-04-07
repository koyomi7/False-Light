using System.Collections;
using UnityEngine;

public class GhostEventManager : MonoBehaviour
{
    public static GhostEventManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] GameObject GhostModel;
    [SerializeField] Transform Player;
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] AudioSource audioSource3;
    [SerializeField] AudioSource auxiliaryAudioSource;
    GameObject Ghost;
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

    [Header("Downstairs Kitchen Scare")]
    [SerializeField] RuntimeAnimatorController downstairsKitchenScareController;
    [SerializeField] AudioClip downstairsKitchenScareScatterSound;
    [SerializeField] Animator downstairsKitchenScarePropsAnimator;
    [HideInInspector] public bool isGhostDiveFinished = false;

    [Header("Downstairs Secret Scare")]
    [SerializeField] RuntimeAnimatorController downstairsSecretScareController;
    [SerializeField] GameObject downstairsSecretChair;
    [SerializeField] GameObject downstairsSecretTVObject;
    [SerializeField] GenericAccessMechanismScript downstairsSecretTV;
    [SerializeField] AudioClip downstairsSecretTVGlitchSound;
    [SerializeField] Light downstairsSecretSpotLight;
    [SerializeField] Light downstairsSecretPointLight;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        animator = GhostModel.GetComponent<Animator>();
        Ghost = GhostModel.transform.parent.gameObject;
        ResetAll();
    }

    void Update()
    {
        RunToPlayer(); // DownstairsOfficeScare
    }

    void ResetAnimatorState()
    {
        // Inside the Inspector for an animation clip:
            // Make sure Root Transform Position (XZ) is Based Upon Original
            // if you want runtime behavior to match what you see in the Animation preview
        animator.Rebind();
        GhostModel.transform.localPosition = Vector3.zero;
        GhostModel.transform.localRotation = Quaternion.identity;
    }

    void SetTransform(float posX = 0f, float posY = 0f, float posZ = 0f, float rotX = 0f, float rotY = 0f, float rotZ = 0f, float scale = 0.13f)
    {
        Ghost.transform.position = new Vector3(posX, posY, posZ);
        Ghost.transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, rotZ));
        Ghost.transform.localScale = new Vector3(scale, scale, scale);
        Ghost.SetActive(true);
    }

    void PlayAnimation(string name, bool useRootMotion)
    {
        animator.applyRootMotion = useRootMotion;
        animator.Play(name, 0, 0f);
    }

    void PlayAudio(AudioClip clip = null, int source = 1, float posX = 0f, float posY = 0f, float posZ = 0f)
    {
        // source 1, 2, 3: ghost audio sources
        // source 4: auxiliary audio source
        switch (source)
        {
            case 1:
                audioSource1.clip = clip;
                audioSource1.Play();
                break;
            case 2:
                audioSource2.clip = clip;
                audioSource2.Play();
                break;
            case 3:
                audioSource3.clip = clip;
                audioSource3.Play();
                break;
            case 4:
                auxiliaryAudioSource.transform.position = new Vector3(posX, posY, posZ);
                auxiliaryAudioSource.clip = clip;
                auxiliaryAudioSource.Play();
                break;
            default:
                Debug.Log($"Error: PlayAudio() does not have a source {source}");
                break;
        }
    }

    void StopAudio(int source = 1)
    {
        // source 1, 2, 3: ghost audio sources
        // source 4: auxiliary audio source
        switch (source)
        {
            case 1:
                audioSource1.Stop();
                audioSource1.clip = null;
                break;
            case 2:
                audioSource2.Stop();
                audioSource2.clip = null;
                break;
            case 3:
                audioSource3.Stop();
                audioSource3.clip = null;
                break;
            case 4:
                auxiliaryAudioSource.transform.position = Vector3.zero;
                auxiliaryAudioSource.Stop();
                auxiliaryAudioSource.clip = null;
                break;
            default:
                Debug.Log($"Error: StopAudio() does not have a source {source}");
                break;
        }
    }

    void ResetAll()
    {
        StopAudio(1);
        StopAudio(2);
        StopAudio(3);
        StopAudio(4);
        animator.runtimeAnimatorController = null;
        animator.applyRootMotion = true;
        animator.speed = 1;
        ResetAnimatorState();
        SetTransform();
        Ghost.SetActive(false);
    }
    
    IEnumerator MoveToPoint(Vector3 targetPos, float duration)
    {
        Vector3 startPos = Ghost.transform.position;
        
        // Calculates direction and rotation
        Vector3 direction = (targetPos - startPos).normalized;
        direction.y = 0;

        // Face the target immediately
        if (direction != Vector3.zero) Ghost.transform.forward = direction;

        float elapsedTime = 0f;
        
        // Smoothly move to position for duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            Ghost.transform.position = Vector3.Lerp(startPos, targetPos, t);
            
            yield return null;
        }

        // Ensure we land exactly on the target to prevent drift
        Ghost.transform.position = targetPos;
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
            case 1: // Player enters downstairs bathroom
                GameManager.Instance.StartEvent(2);
                animator.runtimeAnimatorController = downstairsBathroomScareController;
                if (downstairsBathroomDoor.state == GenericAccessMechanismScript.states.OPEN) goto DoorOpen;
                
                // Door is closed -> player hears footsteps and then a slam on the door
                PlayAudio(footstepsBehindDoor, 4, 8.1f, 0.16f, 10f);
                yield return new WaitForSeconds(footstepsBehindDoor.length);
                PlayAudio(doorSlamSound, 4, 8.1f, 0.16f, 7.5f);
                yield return new WaitForSeconds(doorSlamSound.length);
                ResetAll();
                GameManager.Instance.EndEvent(2);
                break;
                
                // Door is open -> player hears footsteps and sees the ghost running outside
                DoorOpen:
                ResetAnimatorState();
                SetTransform(8.1f, 0.16f, 10f, 0f, 180f, 0f, 0.13f);
                PlayAnimation("GoofyRun", false);
                PlayAudio(footstepsBehindDoor, 1);

                // Ghost moves towards the door, turns -> ghost moves into the bathroom then disappears
                yield return MoveToPoint(new Vector3(8.1f, 0.16f, 7.5f), 1f);
                yield return MoveToPoint(new Vector3(10f, 0.16f, 7.5f), 1f);
                yield return new WaitForSeconds(0.05f);
                Ghost.SetActive(false);
                StopAudio(1); // stops footsteps sound

                // Door slams shut
                AnimationClip temp = downstairsBathroomDoor.openClip;
                downstairsBathroomDoor.overrideController["OPEN"] = doorSlamClip;
                downstairsBathroomDoor.Close();
                yield return new WaitForSeconds(doorSlamClip.length);
                PlayAudio(doorSlamSound, 4, 8.1f, 0.16f, 7.5f);
                yield return new WaitForSeconds(doorSlamSound.length);
                downstairsBathroomDoor.overrideController["OPEN"] = temp;
                ResetAll();
                GameManager.Instance.EndEvent(2);
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
            case 1: // Player enters downstairs bedroom
                GameManager.Instance.StartEvent(3);
                animator.runtimeAnimatorController = downstairsBedroomScareController;

                // Ghost slowly gets up from laying facedown on the bed
                ResetAnimatorState();
                SetTransform(2.88400006f, 0.578959823f, 9.03600025f, 0f, 270f, 0f, 0.1f);
                PlayAnimation("SlowGetUp", true);
                PlayAudio(fleshSoundEffect, 1);
                yield return new WaitUntil(() => isSlowGettingUpFinished);
                StopAudio(1);

                // Ghost runs from the bed and jumps to the wardrobe, hanging from it -> disappears and blood leads the player to the drawer with the pill inside
                ResetAnimatorState();
                SetTransform(2.97500014f, 0.634959817f, 8.77799988f, 0f, 62.3800011f, 0f, 0.1f);
                PlayAnimation("RunAndHit", false);
                PlayAudio(runAndHitSound, 2);
                yield return new WaitUntil(() => isRunAndHitFinished);
                blood.SetActive(true);
                Ghost.SetActive(false);
                yield return new WaitUntil(() => downstairsBedroomPill == null);

                // Player consumes the pill -> ghost is sitting on the bed staring at the player
                ResetAnimatorState();
                SetTransform(3.79900002f, 0.150000006f, 8.92000008f, 0f, 63.52f, 0f, 0.1f);
                PlayAnimation("BedSit", true);
                GameManager.Instance.NextEventReady();
                break;
            case 2: // Player looks at the ghost -> ghost crawls away from the player
                ResetAnimatorState();
                SetTransform(3.26799989f, 0.550000012f, 8.78999996f, 0f, 55f, 0f, 0.1f);
                PlayAnimation("CrawlBack", true);
                yield return new WaitUntil(() => isCrawlBackFinished);
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
                    downstairsHallwayScareDoorAnimator.Play("Open");
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
                yield return new WaitForSeconds(2.1f);
                audioSource1.Stop();
                audioSource2.Stop();
                Ghost.SetActive(false);
                auxiliaryAudioSource.transform.position = new Vector3(8.7173996f, 1.21599996f, 4.99700022f);
                auxiliaryAudioSource.clip = downstairsHallwayScareDoorSlamSound;
                auxiliaryAudioSource.Play();
                downstairsHallwayScareDoorAnimator.Play("Slam");
                yield return new WaitForSeconds(downstairsHallwayScareDoorSlamSound.length);
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

    public IEnumerator DownstairsKitchenScare(int occurrence)
    {
        switch (occurrence)
        {
            case 1: // Player walks into the kitchen
                GameManager.Instance.StartEvent(6);
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsKitchenScareController;
                Ghost.transform.position = new Vector3(2.34100008f, 0.120959818f, 14.1019993f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                Ghost.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Ghost.SetActive(false);
                animator.applyRootMotion = false;
                animator.speed = 0;
                GameManager.Instance.NextEventReady();
                break;
            case 2: // Ghost dives through kitchen window
                Ghost.SetActive(true);
                animator.speed = 1;
                animator.Play("Dive");
                yield return new WaitForSeconds(0.2f);
                audioSource2.clip = downstairsKitchenScareScatterSound;
                audioSource2.Play();
                yield return new WaitUntil(() => isGhostDiveFinished);
                downstairsKitchenScarePropsAnimator.GetComponent<Animator>().Play("Scatter");
                yield return new WaitForSeconds(downstairsKitchenScareScatterSound.length);
                ResetAll();
                GameManager.Instance.EndEvent(6);
                break;
        }
    }

    public IEnumerator DownstairsSecretScare(int occurrence)
    {
        switch (occurrence)
        {
            case 1: // Player near TV
                GameManager.Instance.StartEvent(7);
                Ghost.GetComponent<Animator>().runtimeAnimatorController = downstairsSecretScareController;
                Ghost.transform.position = new Vector3(9.52799988f, 0.0869999975f, 3.39700007f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(0f, -149.744f, 0f));
                Ghost.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
                Ghost.SetActive(true);
                animator.applyRootMotion = false;
                animator.Play("Sitting");
                float tempCoolDownDuration = downstairsSecretTV.CooldownDuration;
                downstairsSecretTV.CooldownDuration = 4f;
                // Player uses the TV (turns it on) -> TV turns off after 3 seconds
                yield return new WaitUntil(() => downstairsSecretTV.state == GenericAccessMechanismScript.states.OPEN);
                yield return new WaitForSeconds(3f);
                downstairsSecretTV.Close(playAudio: true);
                // Player uses the TV again -> Scare
                Ghost.transform.position = new Vector3(11.21f,0.625f,6.4000001f);
                Ghost.transform.rotation = Quaternion.Euler(new Vector3(-35f, 36.796f, 0f));
                animator.Play("Trapped");
                downstairsSecretChair.transform.position = new Vector3(9.56000042f, 0.342000008f, 3.48799992f);
                downstairsSecretChair.transform.rotation = Quaternion.Euler(new Vector3(90f, 155.230011f, 0f));
                downstairsSecretSpotLight.enabled = true;
                AudioClip tempOpenSound = downstairsSecretTV.openSound;
                downstairsSecretTV.openSound = downstairsSecretTVGlitchSound;
                downstairsSecretTV.CooldownDuration = 6f;
                float tempIntensity = downstairsSecretPointLight.intensity;
                downstairsSecretPointLight.intensity = 1f;
                yield return new WaitUntil(() => downstairsSecretTV.state == GenericAccessMechanismScript.states.OPEN);
                yield return new WaitForSeconds(5f);
                downstairsSecretSpotLight.enabled = false;
                downstairsSecretTV.Close(playAudio: true);
                downstairsSecretTV.openSound = tempOpenSound;
                downstairsSecretTV.CooldownDuration = tempCoolDownDuration;
                downstairsSecretPointLight.intensity = tempIntensity;
                ResetAll();
                GameManager.Instance.EndEvent(7);
                break;
            default:
                Debug.Log($"Error: DownstairsSecretScare() does not have a {occurrence} occurrence");
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
