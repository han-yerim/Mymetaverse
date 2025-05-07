using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreUI : BaseUI
{
    TextMeshProUGUI scoreText;
    TextMeshProUGUI comboText;
    TextMeshProUGUI bestScoreText;
    TextMeshProUGUI bestComboText;

    Button startButton;
    Button exitButton;

    protected override UIState GetUIState()
    {
        return UIState.Score;
    }

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);

        scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        comboText = transform.Find("ComboText").GetComponent <TextMeshProUGUI>();
        bestScoreText = transform.Find("BestScoreText").GetComponent<TextMeshProUGUI>();
        bestComboText = transform.Find("BestComboText").GetComponent<TextMeshProUGUI>();

        startButton = transform.Find("StartButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();
        
        // 버튼 클릭시 이벤트 연결
        startButton.onClick.AddListener(OnClickStartButton);
        exitButton.onClick.AddListener(OnclickExitButton);
    }

    // UI에 점수 정보 표시
    public void SetUI(int score, int combo, int bestScore, int bestCombo)
    {
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
        bestScoreText.text = bestScore.ToString();
        bestComboText.text = bestCombo.ToString();
    }

    void OnClickStartButton()
    {
        uiManager.OnClickStart();
    }
    void OnclickExitButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
