using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [Header("Game Settings")]
    public int levelTime = 30; 
    public int targetScore = 5; 
    public float spawnInterval = 1f; 
    public float bonusSpawnChance = 0.2f; 
    public int initialTargets = 10; 
    public int initialDistractors = 10; 
    public float movementSpeed = 2f; 
    public float minDistanceBetweenObjects = 1f;
    public float spawnSpeedIncrease = 0.05f; 

    [Header("UI Elements")]
    public Text scoreText;
    public Text timerText;
    public Text targetScoreText;
    public GameObject gameWinPanel;
    public GameObject gameLosePanel;

    [Header("UI Elements")]
    public Text scoreWinText;
    public Text scoreLoseText; 

    [Header("Game Objects")]
    public GameObject targetPrefab; 
    public GameObject distractorPrefab; 
    public GameObject bonusPrefab; 

    [Header("Camera Settings")]
    public Camera mainCamera; 

    [Header("Audio Clips")]
    public AudioClip bonusSound;
    public AudioClip targetSound;
    public AudioClip distractorSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    private AudioSource audioSource;
    private int score = 0;
    private float timer;
    private bool isGameActive = false;
    private List<GameObject> activeObjects = new List<GameObject>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timer = levelTime;
        targetScoreText.text = "Target: " + targetScore;
        UpdateUI();
        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);

        StartGame();
    }

    void Update()
    {
        if (isGameActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                EndGame(false);
            }
            UpdateUI();

            
            foreach (GameObject obj in activeObjects)
            {
                if (obj != null)
                {
                    obj.transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);

                    
                    if (!IsWithinCameraView(obj.transform.position))
                    {
                        activeObjects.Remove(obj);
                        Destroy(obj);
                        break;
                    }
                }
            }
        }
    }

    void StartGame()
    {
        isGameActive = true;
        InvokeRepeating("SpawnElement", 0f, spawnInterval);
    }

    void SpawnElement()
    {
        if (!isGameActive) return;

        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject prefabToSpawn = null;

        
        float spawnDecision = Random.Range(0f, 1f);

        if (spawnDecision < bonusSpawnChance)
        {
            prefabToSpawn = bonusPrefab; 
        }
        else if (Random.Range(0, 2) == 0)
        {
            prefabToSpawn = targetPrefab; 
        }
        else
        {
            prefabToSpawn = distractorPrefab;
        }

        
        int attempts = 0;
        while (IsOverlapping(spawnPosition) && attempts < 10)
        {
            spawnPosition = GetRandomSpawnPosition();
            attempts++;
        }

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        activeObjects.Add(spawnedObject);

        TapObject1 tapObject = spawnedObject.GetComponent<TapObject1>();
        if (tapObject != null)
        {
            tapObject.isTarget = prefabToSpawn == targetPrefab;
            tapObject.isBonus = prefabToSpawn == bonusPrefab;
        }

        spawnInterval = Mathf.Max(0.1f, spawnInterval - spawnSpeedIncrease);
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, mainCamera.nearClipPlane));
        float randomX = Random.Range(bottomLeft.x, topRight.x);
        float randomY = Random.Range(bottomLeft.y, topRight.y);
        return new Vector2(randomX, randomY);
    }

    bool IsOverlapping(Vector2 position)
    {
        foreach (GameObject obj in activeObjects)
        {
            if (obj != null && Vector2.Distance(obj.transform.position, position) < minDistanceBetweenObjects)
            {
                return true;
            }
        }
        return false;
    }

    bool IsWithinCameraView(Vector2 position)
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(position);
        return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    public void HandleTap(bool isTarget, bool isBonus)
    {
        if (!isGameActive) return;

        
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(targetSound);
            audioSource.Stop();
        }

        GameObject clickedObject = null;
        foreach (var obj in activeObjects)
        {
            if (obj != null && obj.activeInHierarchy && obj.GetComponent<TapObject1>() != null)
            {
                TapObject1 tapObject = obj.GetComponent<TapObject1>();

                if ((tapObject.isTarget && isTarget) || (tapObject.isBonus && isBonus))
                {
                    clickedObject = obj;
                    break;
                }
            }
        }

        if (clickedObject != null)
        {
            clickedObject.SetActive(false);
            activeObjects.Remove(clickedObject);
        }

        if (isBonus)
        {
            score += 5;
            audioSource.PlayOneShot(bonusSound);
        }
        else if (isTarget)
        {
            score++;
            audioSource.PlayOneShot(targetSound);
        }
        else
        {
            if (score > 0) score--;
            audioSource.PlayOneShot(distractorSound);
        }

        UpdateUI();

        Invoke("SpawnElement", 0.5f);

        if (score >= targetScore)
        {
            EndGame(true);
        }
    }

    void EndGame(bool hasWon)
    {
        isGameActive = false;
        CancelInvoke("SpawnElement");

        if (hasWon)
        {
            gameWinPanel.SetActive(true);
            scoreWinText.text = "Final Score: " + score;
            audioSource.PlayOneShot(winSound);
        }
        else
        {
            gameLosePanel.SetActive(true);
            scoreLoseText.text = "Final Score: " + score;
            audioSource.PlayOneShot(loseSound);
        }
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        timerText.text = Mathf.Ceil(timer).ToString();
    }
}
