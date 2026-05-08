using Input;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private Player player;
    public Player Player => player;

    [SerializeField] private InputManager inputManager;
    public InputManager InputManager => inputManager;

    [SerializeField] private StageManager stageManager;
    public StageManager StageManager => stageManager;

    [SerializeField] private Canvas canvas;
    public Canvas Canvas => canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
