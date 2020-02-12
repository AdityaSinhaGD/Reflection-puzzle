using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   
    public enum GameState { start, playing, end };
    public GameState gameState = GameState.start;
    public AudioClip audioClip, levelCompleteSFX;
    AudioSource audioSource;

    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if(instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void OnDisable()
    {
        instance = null;
    }
    #endregion

    private void Awake()
    {
        if (instance != null && instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeStartMessage();
       
    }

    private void InitializeStartMessage()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void LoadNextLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex <= 4)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }

    public void PlayLevelCompleteSFX()
    {
        audioSource.PlayOneShot(levelCompleteSFX);
    }

    
}
