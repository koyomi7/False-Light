using UnityEngine;

public class StalkerGhostController : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] GameObject sequenceTrigger;
    [SerializeField] private Transform player;
    [SerializeField] private float followDistance = 1f;
    [SerializeField] private float followSpeed    = 3f;   // real‑time follow

    [Header("Reveal Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private float flyInSpeed  = 6f;

    [Header("Footstep Audio")]
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private CharacterController playerController;

    [Header("Rotation Lag")]
    [SerializeField] private float rotationFollowSpeed = 2f;

    [SerializeField] public AudioClip Gasp;
    [SerializeField] public AudioClip Footstep;

    private bool hasRevealed;
    bool stopFly = false;

    // ─────────────────────────────────────────────────────────────
    void OnEnable()
    {
        SnapBehindPlayer();          // start in the right place
    }

    void Update()
    {
        FollowPlayer();
        HandleFootstepAudio();
        CheckIfPlayerLooking();
    }

    // ───────── FOLLOW ─────────
    void SnapBehindPlayer()
    {
        Vector3 flatBack = new Vector3(player.forward.x, 0, player.forward.z).normalized;
        Vector3 startPos = player.position - flatBack * followDistance;
        startPos.y = 0.3f;
        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(player.position - transform.position);
    }

    void FollowPlayer()
    {
        Vector3 flatBack = new Vector3(player.forward.x, 0, player.forward.z).normalized;
        Vector3 target   = player.position - flatBack * followDistance;
        target.y         = 0.3f;

        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);

        Quaternion lookRot = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationFollowSpeed * Time.deltaTime);
    }

    // ───────── FOOTSTEPS ─────────
    void HandleFootstepAudio()
    {
        bool moving = playerController.velocity.magnitude > 0.1f;
        footstepAudio.clip = Footstep;
        if (moving && !footstepAudio.isPlaying) footstepAudio.Play();
        if (!moving &&  footstepAudio.isPlaying) footstepAudio.Stop();
    }

    // ───────── LOOK DETECTION ─────────
    void CheckIfPlayerLooking()
    {
        if (stopFly) return;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, ghostLayer) &&
            hit.collider.transform == transform)
        {
            stopFly = true;
            FlyIntoPlayer();
        }
    }

    void FlyIntoPlayer()
    {
        Vector3 target = playerCamera.transform.position + playerCamera.transform.forward * 0.3f;
        transform.position = Vector3.MoveTowards(transform.position, target, flyInSpeed * Time.deltaTime);
        transform.LookAt(playerCamera.transform);

        ghostAnimator.Play("Fall");
        footstepAudio.PlayOneShot(Gasp);
        Debug.Log("Stalker Ghost GASP");
    }

    // Call from Animation Event at the end of the reveal clip
    public void OnRevealComplete()
    {
        hasRevealed = true;
        Destroy(gameObject);
        Destroy(sequenceTrigger);
        Debug.Log("Stalker Ghost reveal complete. Cleaning up.");
    }
}
