using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class GameUI : BaseUI
{
    TextMeshProUGUI scoreText;
    TextMeshProUGUI comboText;
    TextMeshProUGUI maxComboText;

    protected override UIState GetUIState()
    {
        return UIState.Game;
    }

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);

        // 자식 오브젝트에서 각 컴포넌트 연결
        scoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        comboText = transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
        maxComboText = transform.Find("MaxComboText").GetComponent<TextMeshProUGUI>();
    }

    // 점수, 콤보, 최대 콤보 값을 UI에 표시
    public void SetUI(int score, int combo, int maxCombo)
    {
        scoreText.text = score.ToString();
        comboText.text = combo.ToString();
        maxComboText.text = maxCombo.ToString();
    }
}
