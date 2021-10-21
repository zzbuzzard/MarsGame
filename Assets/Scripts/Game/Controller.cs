using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Controller : MonoBehaviour
{
    private GameState gameState;
    public TextMeshProUGUI coinText, waveText;

    public Image selectButton;
    private Color normalCol = Color.white, yellowCol = Color.yellow;

    public GameObject panel0, panel1;

    private void Awake()
    {
        gameState = new GameState(this);
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    float nextWaveTimer = 5.0f;
    bool waiting = true;

    private void Update()
    {
        gameState.Check();

        if (waiting)
        {
            nextWaveTimer -= Time.deltaTime;
            if (nextWaveTimer < 0)
            {
                waiting = false;
                gameState.NextWave();
            }
        }

        coinText.text = "Coins: " + gameState.currencyManager.GetCoins();
        waveText.text = "Wave " + gameState.CurrentWave;
    }

    // Pass GameState so we get an error instead of weird behaviour if our gameState reference changes
    public void SpawnWave(Wave w, GameState g)
    {
        StartCoroutine(SpawnWaveCoroutine(w, g));

        waiting = true;
        nextWaveTimer = w.GetRealWaveGap();
    }

    private IEnumerator SpawnWaveCoroutine(Wave w, GameState g)
    {
        foreach (SpawnGroup s in w.objectSpawns)
        {
            for (int i = 0; i < s.count; i++)
            {
                g.WaveSpawnEnemy(s.prefabName);
            }
            yield return new WaitForSeconds(w.spawnGapTimes);
        }
    }



    ///////////////////////////////// Buttons and UI
    public void SelectClicked()
    {
        gameState.unitSelector.SelectClicked();
        selectButton.color = yellowCol;
    }

    public void SelectDragEnd()
    {
        selectButton.color = normalCol;
    }

    public void CommandButtonPressed(int i)
    {
        gameState.unitSelector.CommandPressed(i);
    }

    public void MoveToPanelPressed()
    {
        gameState.unitSelector.MoveToPanelPressed();
    }

    public bool ScreenScrollable()
    {
        return gameState.unitSelector.ScreenScrollable();
    }

    public void SetPanel(int val)
    {
        if (val == 0)
        {
            panel0.SetActive(true);
            panel1.SetActive(false);
        }
        else
        {
            panel0.SetActive(false);
            panel1.SetActive(true);
        }
    }

    public void DeselectAllClicked()
    {
        gameState.unitSelector.DeselectAll();
    }
}
