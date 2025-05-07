using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIState // 게임의 UI 상태를 정의하는 열거형
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

        // 자식 오브젝트에서 각각의 UI를 찾아 초기화
        homeUI = GetComponentInChildren<HomeUI>(true);
        homeUI?.Init(this); // homeUI가 null이 아니면 동작

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
        theStack.Restart(); // 게임상태 초기화
        ChangeState(UIState.Game); // UI를 게임 화면으로 전환
    }

    public void OnclickExit()
    {
        // 각각의 플랫폼마다 다른 동작을 할 수 있게
#if UNITY_EDITOR
        //UnityEditor.EditorApplication.isPlaying = false; // 에디터에서는 플레이 모드 종료
        SceneManager.LoadScene("MainScene");
#else
        Application.Quit(); // 어플리케이션 종료
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
