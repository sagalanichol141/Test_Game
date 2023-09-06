using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MainCounter : MonoBehaviour
{
    public List<GameObject> meleeEnemy = new List<GameObject>();
    public List<GameObject> rangedEnemy = new List<GameObject>();
    public List<GameObject> destructibleProp = new List<GameObject>();
    public GameObject checkWinUI;
    public GameObject winUI;
    public GameObject defeatUI;
    public TMP_Text meleeEnemyText;
    public TMP_Text rangedEnemyText;
    public TMP_Text destructiblePropText;
    public TMP_Text killCountText;
    public float targetKillCount;
    [HideInInspector] public int killCount;

    private bool checkWinDone;
    private bool win;

    private void Start() {
        Time.timeScale = 1;
        Cursor.visible = false;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);

        CheckCondition();

        foreach (GameObject item in meleeEnemy)
        {
            if (item == null)
            {
                killCount += 1;
                meleeEnemy.Remove(item);
            }
        }

        foreach (GameObject item in rangedEnemy)
        {
            if (item == null)
            {
                killCount += 1;
                rangedEnemy.Remove(item);
            }
        }

        foreach (GameObject item in destructibleProp)
        {
            if (item == null)
                destructibleProp.Remove(item);
        }

        meleeEnemyText.text = meleeEnemy.Count.ToString();
        rangedEnemyText.text = rangedEnemy.Count.ToString();
        destructiblePropText.text = destructibleProp.Count.ToString();
        killCountText.text = killCount.ToString() + " Enemy Killed";
    }

    private void CheckCondition()
    {
        if (killCount >= targetKillCount && !checkWinDone)
        {
            Time.timeScale = 0;
            checkWinUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                checkWinDone = true;
                checkWinUI.SetActive(false);
                winUI.SetActive(true);
                win = true;
                Time.timeScale = 1;
                GameObject.FindWithTag("Player").SetActive(false);
                GameObject.FindWithTag("Player").SetActive(true);
                GameObject.FindWithTag("Player").GetComponent<Animator>().Play("Victory");
                GameObject.FindWithTag("Player").gameObject.tag = "Untagged";
                gameObject.SetActive(false);
            }

            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                Application.Quit();
            }

            else if (Input.GetKeyDown(KeyCode.Return))
            {
                checkWinDone = true;
                Time.timeScale = 1;
                checkWinUI.SetActive(false);
            }
        }

        else if (GameObject.FindWithTag("Player") == null && !win)
        {
            defeatUI.SetActive(true);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy.GetComponent<EnemyController>() != null)
                {
                    enemy.SetActive(false);
                    enemy.SetActive(true);
                    enemy.GetComponent<Animator>().Play("Victory");
                    enemy.GetComponent<NavMeshAgent>().enabled = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                int index = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(index);
            }

            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        else if (meleeEnemy.Count <= 0 && rangedEnemy.Count <= 0)
        {
            win = true;
            winUI.SetActive(true);
            GameObject.FindWithTag("Player").SetActive(false);
            GameObject.FindWithTag("Player").SetActive(true);
            GameObject.FindWithTag("Player").GetComponent<Animator>().Play("Victory");
            GameObject.FindWithTag("Player").gameObject.tag = "Untagged";
            gameObject.SetActive(false);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
