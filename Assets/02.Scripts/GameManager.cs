using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private GameObject _spawnPointParent;
    private Transform[] _spawnPoints;
    
    private Text _scoreText;
    private Text _reSpawnText;
    private Text _gameResultText;

    private int _myScore;
    private int _enemyScore;

    private const int EndScore = 5;
    
    private const int MaxReSpawnDelay = 3;

    public int MyScore
    {
        get => _myScore;
        set
        {
            _myScore = value;
            UpdateScore();
        }
    }
    
    public int EnemyScore
    {
        get => _enemyScore;
        set
        {
            _enemyScore = value;
            UpdateScore();
        }
    }

    protected override void Init()
    {
        _spawnPointParent = GameObject.FindWithTag("SpawnPoint");
        _spawnPoints = _spawnPointParent.GetComponentsInChildren<Transform>();
        
        _scoreText = GameObject.FindWithTag("ScoreText").GetComponent<Text>();
        _reSpawnText = GameObject.FindWithTag("ReSpawnText").GetComponent<Text>();
        _gameResultText = GameObject.FindWithTag("GameResultText").GetComponent<Text>();

        _myScore = 0;
        _enemyScore = 0;
        
        UpdateScore();
        _gameResultText.text = "";
        _reSpawnText.text = "";
    }

    public void ReSpawn(GameObject respawnObject, bool isMine)
        => StartCoroutine(ReSpawnUI(respawnObject, isMine));

    private IEnumerator ReSpawnUI(GameObject respawnObject, bool isMine)
    {
        for (int i = MaxReSpawnDelay; i >= 0; i--)
        {
            if (isMine)
            {
                _reSpawnText.text = $"부활까지 남은 시간 : {i}초";
            }

            yield return new WaitForSeconds(1f);
        }

        _reSpawnText.text = "";
        
        SetRandomPosition(respawnObject);
    }

    public void SetRandomPosition(GameObject dirObject)
    {
        int randomIndex = Random.Range(0, _spawnPoints.Length);
        dirObject.transform.position = _spawnPoints[randomIndex].position;
        dirObject.transform.rotation = Quaternion.identity;
        dirObject.SetActive(true);
    }

    private void UpdateScore()
    {
        _scoreText.text = $"나 {_myScore} : {_enemyScore} 상대";

        if (_myScore == EndScore)
        {
            EndGame(true);
        }
        else if (_enemyScore == EndScore)
        {
            EndGame(false);
        }
    }

    private void EndGame(bool isWinnerMe)
    {
        _reSpawnText.text = "";
        
        StopAllCoroutines();
        
        if (isWinnerMe)
        {
            _gameResultText.text = "WIN";
            _gameResultText.color = Color.green;
        }
        else
        {
            _gameResultText.text = "LOSE";
            _gameResultText.color = Color.red;
        }
    }
}
