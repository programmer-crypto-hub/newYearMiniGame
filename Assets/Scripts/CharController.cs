using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float gravityModifier = 10f;
    [SerializeField] static bool gravityInitialized = false;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip keyPickUpSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] public bool isGameOver;
    [SerializeField] private bool isOnGround = true;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TutorialManual tutorialManual;
    [SerializeField] AllCoinCounter allCoinCounter;
    private AudioSource playerAudio;
    [SerializeField] public GameObject player;
    public Animator playerAnim;
    public int coins = 0;
    public int keys = 0;
    public int lives = 3;
    [SerializeField] Vector2 startPos = new Vector2(-7f, -1.5f);
    private GameObject coinPrefab;
    [SerializeField] public ClockSystem clockSystem;
    [SerializeField] public AdditionalMechanics addMechs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        if (!gravityInitialized)
        {
            Physics.gravity *= gravityModifier;
            gravityInitialized = true;
        }
        isOnGround = false;
        
        // Показать туториал в начале игры
        if (tutorialManual != null)
        {
            tutorialManual.Start();
        }
    }

    public void OnJump()
    {
        if (isOnGround && !isGameOver)
        {
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            playerAnim.SetTrigger("jump_trig");
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isOnGround = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.timeScale == 0f) return; // Не обрабатывать ввод при паузе
    
        if (isGameOver && lives <= 0)
        {
            Debug.Log("No lives left! Game Over!");
            uiManager.Restart();
        }

        if (isGameOver) return;

        // Движение вправо при удерживании клавиши D или стрелки вправо
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = targetRotation;
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            playerAnim.SetTrigger("walk_trig");
        }
        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow) &&
            !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow))
        {
            playerAnim.ResetTrigger("walk_trig");
            playerAnim.SetTrigger("idle_trig");
        }

        // Движение влево при удерживании клавиши A или стрелки влево
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
            transform.rotation = targetRotation;
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            playerAnim.SetTrigger("walk_trig");
        }

        // Прыжок по нажатию пробела
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            OnJump();
        }

        if (transform.position.y < -4)
        {
            isGameOver = true;
            lives--;
            uiManager.UpdateHearts();
            Debug.Log("Lives: " + lives);
            playerAnim.SetBool("isDead", true);
            StartCoroutine(Respawn());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            rb.simulated = true;
            playerAnim.SetBool("isOnGround", true);
        }
        if (!collision.gameObject.CompareTag("Ground"))
        {
            playerAnim.SetBool("isOnGround", false);
        }
        if (collision.gameObject.CompareTag("Start"))
        {
            isOnGround = true;
            playerAnim.SetBool("isOnGround", true);
        }
        if (collision.gameObject.CompareTag("Collectable"))
        {
            playerAudio.PlayOneShot(pickUpSound, 1.0f);
            Destroy(collision.gameObject);
            coins++;
            allCoinCounter.coinAmount++;
            Debug.Log("Coins: " + coins);
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isGameOver = true;
            playerAudio.PlayOneShot(deathSound, 1.0f);
            lives--;
            uiManager.UpdateHearts();
            Debug.Log("Lives: " + lives);
            playerAnim.SetBool("isDead", true);
            StartCoroutine(Respawn());
            Debug.Log("Called Respawn coroutine");
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            isGameOver = true;
            uiManager.endScreen.SetActive(true);
            uiManager.ActivateUI();
            Time.timeScale = 0f;
            clockSystem.StopTimer();
            Debug.Log("You Win!");
            allCoinCounter.allCoins += coins;
            Debug.Log("Total coins collected: " + allCoinCounter.allCoins);
        }
        if (collision.gameObject.CompareTag("Key"))
        {
            playerAudio.PlayOneShot(keyPickUpSound, 1.0f);
            Destroy(collision.gameObject);
            keys++;
            Debug.Log("Key collected!, keys: " + keys);
        }
        if (collision.gameObject.CompareTag("Chest") && keys >= 1)
        {
            addMechs.OnTriggerEnter2D(collision.collider);
        }
        if (collision.gameObject.CompareTag("Ladder"))
        {
            Debug.Log("Climbing ladder");
            addMechs.isClimbing = true;
        }
        if (!collision.gameObject.CompareTag("Ladder"))
        {
            addMechs.isClimbing = false;
            addMechs.playerRb.gravityScale = 1;
        }
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.5f);
        transform.position = startPos;
        playerAnim.SetBool("isDead", false);
        playerAnim.SetTrigger("idle_trig");
        Debug.Log("Lives: " + lives);

        isGameOver = false;
        Debug.Log("You died! Respawning");

        if (lives <= 0)
        {
            Debug.Log("No lives left! Game Over!");
            uiManager.Restart();
            coins = 0;
            allCoinCounter.coinAmount = 0;
        }
    }
}