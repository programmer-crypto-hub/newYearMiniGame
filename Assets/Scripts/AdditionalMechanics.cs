using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditionalMechanics : MonoBehaviour
{
    [SerializeField] private CharController charController;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject chestWithReward;
    //[SerializeField] private GameObject door;
    [SerializeField] private GameObject keyItem;

    //[SerializeField] private AudioClip gateOpenedSound;
    [SerializeField] private AudioClip chestOpenedSound;
    [SerializeField] private AudioClip keyGrabbedSound;
    [SerializeField] private AudioSource playerAudio;

    [SerializeField] private int totalCoinsAmount = 0;

    public GameObject rewardItem;
    public Rigidbody2D rb;
    public Transform spawnPoint; // Position inside the chest
    public float upwardForce = 5f;
    public float outwardForce = 5f;

    public bool isClimbing = false;
    private float verticalInput;
    public float climbSpeed = 5f;
    [SerializeField] public Rigidbody2D playerRb;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            playerRb.gravityScale = 1; // Restore gravity when leaving ladder
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        rewardItem.SetActive(false);
        rb = rewardItem.GetComponent<Rigidbody2D>();
        playerRb = charController.player.GetComponent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        rb.bodyType = RigidbodyType2D.Dynamic;
        Vector2 jumpDirection = new Vector2(Random.Range(-outwardForce, outwardForce), upwardForce);
        playerAudio.PlayOneShot(chestOpenedSound);
        charController.playerAnim.SetTrigger("chest_Trig");
        rewardItem.SetActive(true);
        rewardItem.transform.SetParent(null);
        rb.AddForce(jumpDirection, ForceMode2D.Impulse);
        Destroy(gameObject, 2.0f);
    }


    public void Update()
    {
        if (charController.keys >= 1)
        {
            // Decrease key count
            charController.keys -= 1;
            uiManager.UpdateHUDStats();
            // Play key grabbed sound
            playerAudio.PlayOneShot(keyGrabbedSound, 1.0f);
        }
        // Get vertical input (W/S or Up/Down arrows)
        verticalInput = Input.GetAxisRaw("Vertical");

        // Start climbing if touching ladder and pressing up/down
        if (isClimbing && Mathf.Abs(verticalInput) > 0f)
        {
            playerRb.gravityScale = 0; // Disable gravity while climbing
            playerRb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);
        }
        else if (isClimbing)
        {
            // Stay still on the ladder if no input is given
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
        }
    }
}