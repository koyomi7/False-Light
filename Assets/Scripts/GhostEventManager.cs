using System.Collections;
using UnityEngine;

public class GhostEventManager : MonoBehaviour
{
    public static GhostEventManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] GameObject GhostModel;
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] AudioSource audioSource3;
    [SerializeField] AudioSource auxiliaryAudioSource;
    GameObject Ghost;
    Animator animator;

    [Header("Downstairs Office Scare")]
    [SerializeField] RuntimeAnimatorController downstairsOfficeScareController;
    [SerializeField] AudioClip downstairsOfficeGhostFootsteps;
    [SerializeField] AudioClip downstairsOfficeGhostRoar;
    [SerializeField] AudioClip downstairsOfficeHeavyBreathing;

    [Header("Downstairs Bathroom Scare")]
    [SerializeField] RuntimeAnimatorController downstairsBathroomScareController;
    [SerializeField] AudioClip downstairsBathroomDoorSlam;
    [SerializeField] AudioClip downstairsBathroomGhostFootsteps;
    [SerializeField] GenericAccessMechanismScript downstairsBathroomDoor;
    [SerializeField] AnimationClip downstairsBathroomDoorSlamClip;

    [Header("Downstairs Bedroom Scare")]
    [SerializeField] RuntimeAnimatorController downstairsBedroomScareController;
    [SerializeField] AudioClip downstairsBedroomFlesh;
    [SerializeField] AudioClip downstairsBedroomRunAndHit;
    [SerializeField] GameObject downstairsBedroomPill;
    [SerializeField] GameObject downstairsBedroomBlood;
    [HideInInspector] public bool isSlowGettingUpFinished = false;
    [HideInInspector] public bool isRunAndHitFinished = false;
    [HideInInspector] public bool isCrawlBackFinished = false;

    [Header("Downstairs Living Room Scare")]
    [SerializeField] RuntimeAnimatorController downstairsLivingRoomScareController;
    [SerializeField] AudioClip downstairsLivingRoomWindowBreak;
    [SerializeField] GameObject downstairsLivingRoomSpotLight;
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

    void ResetAnimatorState()
    {
        // Inside the Inspector for an animation clip:
            // Make sure Root Transform Rotation is Based Upon (at Start) Original and
            // Root Transform Position (XZ) is Based Upon Original
            // if you want runtime behavior to match what you see in the Animation preview
        animator.Rebind();
        GhostModel.transform.localPosition = Vector3.zero;
        GhostModel.transform.localRotation = Quaternion.identity;
    }

    void SetTransform(Vector3 position = default, Vector3 rotation = default, float scale = 0.13f)
    {
        Ghost.transform.position = position;
        Ghost.transform.rotation = Quaternion.Euler(rotation);
        Ghost.transform.localScale = new Vector3(scale, scale, scale);
        Ghost.SetActive(true);
    }

    void PlayAnimation(string name, bool useRootMotion, float speed = 1f)
    {
        animator.applyRootMotion = useRootMotion;
        animator.speed = speed;
        animator.Play(name, 0, 0f);
    }

    void PlayAudio(int source = 1, AudioClip clip = null, bool loop = false, Vector3 position = default)
    {
        // source 1, 2, 3: ghost audio sources
        // source 4: auxiliary audio source
        switch (source)
        {
            case 1:
                audioSource1.clip = clip;
                audioSource1.loop = loop;
                audioSource1.Play();
                break;
            case 2:
                audioSource2.clip = clip;
                audioSource2.loop = loop;
                audioSource2.Play();
                break;
            case 3:
                audioSource3.clip = clip;
                audioSource3.loop = loop;
                audioSource3.Play();
                break;
            case 4:
                auxiliaryAudioSource.transform.position = position;
                auxiliaryAudioSource.clip = clip;
                auxiliaryAudioSource.loop = loop;
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
        if (direction != Vector3.zero)
        {
            float targetY = Quaternion.LookRotation(direction).eulerAngles.y;
            Vector3 currentRotation = Ghost.transform.rotation.eulerAngles;

            Ghost.transform.rotation = Quaternion.Euler(currentRotation.x, targetY, currentRotation.z);
        }
        
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
            case 1: // Player enters downstairs office
                GameManager.Instance.StartEvent(1);
                animator.runtimeAnimatorController = downstairsOfficeScareController;

                // Player can hear the ghost crying outside -> player sees the ghost having a seizure
                ResetAnimatorState();
                SetTransform(new Vector3(7.12599993f, 1.43995976f, 13.243f), new Vector3(90f, 55.2999878f, 0f), 0.13f);
                PlayAnimation("Seizure", true);
                PlayAudio(1, downstairsOfficeHeavyBreathing, true);
                GameManager.Instance.NextEventReady();
                break;
            case 2: // Play comes near the ghost -> ghost disappears for 1 second
                Ghost.SetActive(false);
                StopAudio(1);
                yield return new WaitForSeconds(1f);

                // Ghost reappears -> ghost runs at the player
                ResetAnimatorState();
                SetTransform(new Vector3(7.3f, 0.96f, 16.5f), new Vector3(24f, 180, 0f), 0.13f);
                PlayAnimation("ClownRun", false, 1.5f);
                PlayAudio(1, downstairsOfficeGhostRoar);
                PlayAudio(2, downstairsOfficeGhostFootsteps, true);
                yield return MoveToPoint(new Vector3(7.3f, 0.96f, 10.5f), 1f);
                ResetAll();
                GameManager.Instance.EndEvent(1);
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
                PlayAudio(4, downstairsBathroomGhostFootsteps, false, new Vector3(8.1f, 0.16f, 10f));
                yield return new WaitForSeconds(downstairsBathroomGhostFootsteps.length);
                PlayAudio(4, downstairsBathroomDoorSlam, false, new Vector3(8.1f, 0.16f, 7.5f));
                yield return new WaitForSeconds(downstairsBathroomDoorSlam.length);
                ResetAll();
                GameManager.Instance.EndEvent(2);
                break;
                
                // Door is open -> player hears footsteps and sees the ghost running outside
                DoorOpen:
                ResetAnimatorState();
                SetTransform(new Vector3(8.1f, 0.16f, 10f), new Vector3(0f, 180f, 0f), 0.13f);
                PlayAnimation("GoofyRun", false);
                PlayAudio(1, downstairsBathroomGhostFootsteps, true);

                // Ghost moves towards the door, turns -> ghost moves into the bathroom then disappears
                yield return MoveToPoint(new Vector3(8.1f, 0.16f, 7.5f), 1f);
                yield return MoveToPoint(new Vector3(10f, 0.16f, 7.5f), 1f);
                yield return new WaitForSeconds(0.05f);
                Ghost.SetActive(false);
                StopAudio(1); // stops footsteps sound

                // Door slams shut
                AnimationClip temp = downstairsBathroomDoor.openClip;
                downstairsBathroomDoor.overrideController["OPEN"] = downstairsBathroomDoorSlamClip;
                downstairsBathroomDoor.Close();
                yield return new WaitForSeconds(downstairsBathroomDoorSlamClip.length);
                PlayAudio(4, downstairsBathroomDoorSlam, false, new Vector3(8.1f, 0.16f, 7.5f));
                yield return new WaitForSeconds(downstairsBathroomDoorSlam.length);
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
                SetTransform(new Vector3(2.88400006f, 0.578959823f, 9.03600025f), new Vector3(0f, 270f, 0f), 0.1f);
                PlayAnimation("SlowGetUp", true);
                PlayAudio(1, downstairsBedroomFlesh);
                yield return new WaitUntil(() => isSlowGettingUpFinished);
                StopAudio(1);

                // Ghost runs from the bed and jumps to the wardrobe, hanging from it -> disappears and blood leads the player to the drawer with the pill inside
                ResetAnimatorState();
                SetTransform(new Vector3(2.97500014f, 0.634959817f, 8.77799988f), new Vector3(0f, 62.3800011f, 0f), 0.1f);
                PlayAnimation("RunAndHit", false);
                PlayAudio(2, downstairsBedroomRunAndHit);
                yield return new WaitUntil(() => isRunAndHitFinished);
                downstairsBedroomBlood.SetActive(true);
                Ghost.SetActive(false);
                yield return new WaitUntil(() => downstairsBedroomPill == null);

                // Player consumes the pill -> ghost is sitting on the bed staring at the player
                ResetAnimatorState();
                SetTransform(new Vector3(3.79900002f, 0.150000006f, 8.92000008f), new Vector3(0f, 63.52f, 0f), 0.1f);
                PlayAnimation("BedSit", true);
                GameManager.Instance.NextEventReady();
                break;
            case 2: // Player looks at the ghost -> ghost crawls away from the player
                ResetAnimatorState();
                SetTransform(new Vector3(3.26799989f, 0.550000012f, 8.78999996f), new Vector3(0f, 55f, 0f), 0.1f);
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
            case 1: // Player enters the living room downstairs
                GameManager.Instance.StartEvent(4);
                animator.runtimeAnimatorController = downstairsLivingRoomScareController;
                GameManager.Instance.NextEventReady();
                break;
            case 2: // Player looks at window in front -> player hears glass breaking and sees the ghost falling outside the window
                PlayAudio(4, downstairsLivingRoomWindowBreak, false, new Vector3(7.5f, 3.6f, 19f));
                ResetAnimatorState();
                SetTransform(new Vector3(7.5f, 0.4f, 19.6f), new Vector3(0f, 270f, 0f), 0.13f);
                PlayAnimation("FallingOnImpact", true);
                yield return new WaitUntil(() => isFallingFinished);

                // Ghost stares at player through the window on the right
                ResetAnimatorState();
                SetTransform(new Vector3(12.25f, 0.04f, 14.631f), new Vector3(0f, 270f, 0f), 0.13f);
                PlayAnimation("Trapped", true);
                downstairsLivingRoomSpotLight.SetActive(true);
                GameManager.Instance.NextEventReady();
                break;
            case 3: // Player looks at the ghost through the window on the right -> ghost vanishes
                downstairsLivingRoomSpotLight.SetActive(false);
                ResetAnimatorState();
                PlayAnimation("Vanish", true);
                yield return new WaitUntil(() => isVanishFinished);
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
}
