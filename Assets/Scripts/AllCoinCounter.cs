using UnityEngine;

public class AllCoinCounter : MonoBehaviour
{
    [SerializeField] public int allCoins = 0;
    [SerializeField] public int coinAmount = 0;
    [SerializeField] private CharController charController;
    public static AllCoinCounter instance;

    void Update()
    {
        if (charController == null)
        {
            charController = GameObject.Find("Player").GetComponent<CharController>();
        }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins()
    {
        if (charController != null && charController.isGameOver)
        {
            coinAmount = charController.coins;
            allCoins += coinAmount;
            PlayerPrefs.SetFloat("AllCoins", allCoins);
            Debug.Log("Total coins collected: " + allCoins);
        }
    }
}