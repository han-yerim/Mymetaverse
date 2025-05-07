using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIState // ������ UI ���¸� �����ϴ� ������
{
    Home,
    Game,
    Score,
}

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    UIState currentState = UIState.Home;
    HomeUI homeUI = null;
    GameUI gameUI = null;
    ScoreUI scoreUI = null;

    TheStack theStack = null;

    private void Awake()
    {
        instance = this;

        theStack = FindObjectOfType<TheStack>();

        // �ڽ� ������Ʈ���� ������ UI�� ã�� �ʱ�ȭ
        homeUI = GetComponentInChildren<HomeUI>(true);
        homeUI?.Init(this); // homeUI�� null�� �ƴϸ� ����

        gameUI = GetComponentInChildren<GameUI>(true);
        gameUI?.Init(this);

        scoreUI = GetComponentInChildren<ScoreUI>(true);
        scoreUI?.Init(this);

        ChangeState(UIState.Home);
    }

    public void ChangeState(UIState state)
    {
        currentState = state;
        homeUI?.SetActive(currentState);
        gameUI?.SetActive(currentState);
        scoreUI?.SetActive(currentState);
    }

    public void OnClickStart()
    {
        theStack.Restart(); // ���ӻ��� �ʱ�ȭ
        ChangeState(UIState.Game); // UI�� ���� ȭ������ ��ȯ
    }

    public void OnclickExit()
    {
        // ������ �÷������� �ٸ� ������ �� �� �ְ�
#if UNITY_EDITOR
        //UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ����� �÷��� ��� ����
        SceneManager.LoadScene("MainScene");
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }

    public void UpdateScore()
    {
        gameUI.SetUI(theStack.Score, theStack.Combo, theStack.MaxCombo);
    }

    public void SetScoreUI()
    {
        scoreUI.SetUI(theStack.Score, theStack.MaxCombo, theStack.BestScore, theStack.BestCombo);
        ChangeState(UIState.Score);
    }
}
