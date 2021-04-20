using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] private Player _player;
    [SerializeField] private float _alertDistance;
    public static Player Player => _instance._player;

    private void Awake()
    {
        // If a GameManager already exists
        if (_instance != null)
        {
            // Destroy this extra game manager
            Destroy(gameObject);
            Debug.LogWarning($"2 instances of the Game Manager were present");
        }
        // Otherwise,
        else
        {
            // Store this as the game manager
            _instance = this;
        }
    }

    private void Start()
    {
        // Find all the player spawn points
        var playerSpawnPoints = FindObjectsOfType<SpawnPoint>().Where(spawnPoint => spawnPoint.Type == SpawnType.Player).ToArray();
        
        // Move the player to one of the spawn points
        var randomSpawnPoint = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
        _player.transform.position = randomSpawnPoint.transform.position;
        _player.transform.rotation = randomSpawnPoint.transform.rotation;
    }

    public static void OnStashArtifact()
    {
        // If the player has collected all the artifacts
        if (FindObjectsOfType<Artifact>().All(artifact => artifact.Stashed))
        {
            // The game is won!
            OnGameOver();
        }
    }

    private static void OnGameOver()
    {
        // TODO: Fanfare!
        Debug.Log($"You won. :|");
    }

    public static void AlertNearbyRobotsToPlayer()
    {
        foreach (var robot in FindObjectsOfType<Robot>().Where(r =>
            Vector3.Distance(r.transform.position, Player.transform.position) <= _instance._alertDistance))
        {
            Debug.Log($"Alerting {robot.name}");
            robot.OnPlayerDetected();
        }
    }
}
