using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus
{
    next, play, gameover, win
}


public class Manager : Loader<Manager>
{

    [SerializeField]
    int totalWaves = 10;
    [SerializeField]
    Text totalMoneyLabel;
    [SerializeField]
    Text currentWave;
    [SerializeField]
    Text totalEscapeLabel;
    [SerializeField]
    Text playBtnLabel;
    [SerializeField]
    Button playBtn;
    [SerializeField]
    GameObject spawnPoint;
    [SerializeField]
    GameObject[] enemies;
    [SerializeField]
    int totalEnemies = 5;
    [SerializeField]
    int enemiesPerSpawn; 


    int waveNumber = 0;
    int totalMoney = 10;
    int totalEscaped = 0;
    int roundEscaped = 0;
    int totalKilled = 0;
    int whichEnemyToSpawn = 0;
    gameStatus currentState = gameStatus.play;



    public List<Enemy> EnemyList = new List<Enemy>();


    const float spawnDelay = 0.5f;

  

    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }

    public int RoundEscaped
    {
        get
        {
            return roundEscaped;
        }
        set
        {
            roundEscaped = value;
        }
    }

    public int TotalKilled
    {
        get
        {
            return totalKilled;
        }
        set
        {
            totalKilled = value;
        }
    }
  

    public int TotalMoney
    {
        get
        {
            return totalMoney;
        }
        set
        {
            totalMoney = value;
            totalMoneyLabel.text = TotalMoney.ToString();
        }
    }



    // Use this for initialization
    void Start()
    {

        playBtn.gameObject.SetActive(false);
        ShowMenu();
    }

    private void Update()
    {
        HandleEscape();
    }


    IEnumerator Spawn()
    {
        if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (EnemyList.Count < totalEnemies)
                {
                    GameObject newEnemy = Instantiate(enemies[1]) as GameObject;
                    newEnemy.transform.position = spawnPoint.transform.position;

                }

            }

            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(Spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DestroyEnemies()
    {
        foreach (Enemy enemy in EnemyList)
        {
            Destroy(enemy.gameObject);
        }

        EnemyList.Clear();
    }

    public void addMoney(int amount)
    {
        TotalMoney += amount;
    }

    public void subtractMoney(int amount)
    {
        TotalMoney -= amount;


    }

    public void IsWaveOver()
    {
        totalEscapeLabel.text = "Escaped" + TotalEscaped + "/ 10"; 

        if ((RoundEscaped + TotalKilled) == totalEnemies)
        {
            SetCurrentGameState();
            ShowMenu();
        }
    }

    public void SetCurrentGameState()
    {
        if(totalEscaped >= 10)
        {
            currentState = gameStatus.gameover;
        }
        else  if(waveNumber == 0 && (RoundEscaped + TotalKilled) == 0 )
        {
            currentState = gameStatus.play;
        }
        else if (waveNumber >= totalWaves)
        {
            currentState = gameStatus.win;
        }
        else
        {
            currentState = gameStatus.next; 
        }
    }

    public void PlayButtonPressed()
    {
        switch(currentState)
        {
            case gameStatus.next:
                waveNumber += 1;
                totalEnemies += waveNumber + 1;
                break;

            default:
                totalEnemies = 5;
                TotalEscaped = 0;
                TotalMoney = 10;
                TowerManager.Instance.DestroyAllTowers();
                TowerManager.Instance.RenameTagBuildSite();
                totalMoneyLabel.text = TotalMoney.ToString();
                totalEscapeLabel.text = "Escaped" + TotalEscaped + "/ 10 ";
                break;
        }
        DestroyEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWave.text = "Wave" + (waveNumber + 1);
        StartCoroutine(Spawn());
        playBtn.gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        switch(currentState)
        {
            case gameStatus.gameover:
                playBtnLabel.text = "Play Again";

                break;

            case gameStatus.next:
                playBtnLabel.text = "Next wave";

                break;

            case gameStatus.play:
                playBtnLabel.text = "Play game";

                break;

            case gameStatus.win:
                playBtnLabel.text = "Play game again";

                break;


        }

        playBtn.gameObject.SetActive(true);
    }

    private void HandleEscape()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instance.DisableDrag();
            TowerManager.Instance.towerBtnPressed = null; 

        }
    }
}
