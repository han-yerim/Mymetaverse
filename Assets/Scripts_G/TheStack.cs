using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    private const float BoundSize = 3.5f; // 블록 기본 너비
    private const float MovingBoundsSize = 3f; // 블록이 이동할 수 있는 거리 크기
    private const float StackMovingSpeed = 5.0f; // 전체 스택이 올라가는 속도
    private const float BlockMovingSpeed = 3.5f; // 블록이 좌우로 움직이는 속도
    private const float ErrorMargin = 0.1f; // 정밀도 판정 기준 (잘릴지 여부)

    public GameObject originBlock = null; // 원본 블록 프리팹

    private Vector3 prevBlockPosition; // 이전 블록의 위치
    private Vector3 desiredPosition; // 전체 스택의 목표 위치 (부드러운 카메라 이동용)
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize); // 현재 블록의 너비

    Transform lastBlock = null; // 현재 생성된 마지막 블록
    float blockTransition = 0f; // 블록 이동을 위한 시간 누적 변수
    float secondaryPosition = 0f; // 고정 위치 (X 또는 Z축)

    int stackCount = -1; // 현재까지 쌓은 블록 수
    public int Score { get { return stackCount; } }
    int comboCount = 0; // 연속으로 정확히 블록을 놓은 횟수
    public int Combo { get { return comboCount; } }

    private int maxCombo = 0; // 가장 많이 연속으로 성공한 콤보 수
    
    public int MaxCombo { get =>  maxCombo; }

    public Color prevColor;
    public Color nextColor;

    bool isMovingX = true;

    int bestScore = 0; // 최고 점수 저장 변수
    public int BestScore { get => bestScore; }

    int bestCombo = 0; // 최고 콤보 저장 변수
    public int BestCombo { get => bestCombo; }

    private const string BestScoreKey = "BestScore";
    private const string BestComboKey = "BestCombo";

    private bool isGameOver = true;

    void Start()
    {
        if (originBlock == null)
        {
            Debug.Log("OriginBlock is NULL"); // 프리팹이 설정되지 않았을 경우
            return;
        }

        // 저장된 최고 점수 불러오기 (없으면 기본값 0)
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestCombo = PlayerPrefs.GetInt(BestComboKey, 0);

        // 시작 시 사용할 두 가지 색상을 랜덤으로 설정
        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down; // 초기 블록보다 아래에 있는 가상 위치

        Spawn_Block(); // 첫 번째 블록 생성
        Spawn_Block();
    }

    void Update()
    {
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0)) // 마우스 클릭시 블록 생성
        {
            if(PlaceBlock())
            {
                Spawn_Block();
            }
            else
            {
                // 게임 오버
                UpdateScore();
                isGameOver = true; // 게임 오버 상태 설정
                GameOverEffect();
                UIManager.Instance.SetScoreUI(); // 스코어 UI 적용
            }
            
        }

        MoveBlock();
        // 스택 전체를 위로 부드럽게 이동 (카메라 따라올리는 효과)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed * Time.deltaTime);
    }

    bool Spawn_Block()
    {
        // 이전 블록 저장 (다음 블록 위치와 정렬 비교에 사용됨)
        if (lastBlock != null)
            prevBlockPosition = lastBlock.localPosition;

        GameObject newBlock = null;
        Transform newTrans = null;

        newBlock = Instantiate(originBlock); // 새 블록 생성

        if (newBlock == null)
        {
            return false;
        }

        ColorChange(newBlock); // 새 블록에 색 적용

        newTrans = newBlock.transform;
        // 부모 설정 (스택 안에 블록을 자식으로 넣음)
        newTrans.parent = this.transform;
        // 위치 설정 이전 블록 위에 쌓임
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        // 회전 초기화
        newTrans.localRotation = Quaternion.identity;
        // 블록 크기 설정
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++; // 블록 개수 증가

        desiredPosition = Vector3.down * stackCount; // 카메라/스택이 이동할 목표 위치 업데이트
        blockTransition = 0f; // 블록 이동 초기화

        lastBlock = newTrans; // 마지막 블록 갱신

        isMovingX = !isMovingX;

        UIManager.Instance.UpdateScore();
        return true;
    }

    Color GetRandomColor()
    {
        // 100~250 범위의 밝은 RGB 값을 생성
        float r = Random.Range(100f, 250f) / 255f;
        float g = Random.Range(100f, 250f) / 255f;
        float b = Random.Range(100f, 250f) / 255f;

        return new Color(r, g, b);
    }

    void ColorChange(GameObject go)
    {
        // prevColor > nextColor로 부드럽게 보간
        // 0~1 사이의 값 (10개 블록 기준)
        Color applyColor = Color.Lerp(prevColor, nextColor, (stackCount % 11) / 10f);

        Renderer rn = go.GetComponent<Renderer>(); // 블록에 적용

        if (rn == null)
        {
            return;
        }

        rn.material.color = applyColor;
        // 카메라 배경색도 블록보다 약간 어두운 색으로 설정
        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);

        // 만약 보간이 끝까지 도달했으면 새로운 색 준비
        if (applyColor.Equals(nextColor) == true)
        {
            prevColor = nextColor;
            nextColor = GetRandomColor();
        }
    }

    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;

        // PingPong - 좌우 반복 이동 효과 생성
        float movePosition = Mathf.PingPong(blockTransition, BoundSize) - BoundSize / 2;

        if(isMovingX)
        {
            lastBlock.localPosition = new Vector3(
                movePosition * MovingBoundsSize, stackCount, secondaryPosition); // X축이동 > Z 고정
        }
        else
        {
            lastBlock.localPosition = new Vector3(
                secondaryPosition, stackCount, movePosition * MovingBoundsSize); // Z축 이동 > X 고정
        }
    }

    bool PlaceBlock()
    {
        Vector3 lastPosition = lastBlock.transform.localPosition; // 현재 블록의 위치

        if (isMovingX) // x축 방향으로 이동중일 경우
        {
            float deltaX = prevBlockPosition.x - lastPosition.x; // 이전 블록과의 위치 차이
            bool isNegativeNum = (deltaX < 0) ? true : false; // 왼쪽으로 벗어난 여부 체크

            deltaX = Mathf.Abs(deltaX); // 오차 절대값으로 비교
            if (deltaX > ErrorMargin)
            {
                stackBounds.x -= deltaX; // 블록 너비 줄이기
                if (stackBounds.x <= 0) // 블록이 너무 작아져서 게임오버
                {
                    return false;
                }

                float middle = (prevBlockPosition.x + lastPosition.x) / 2; // 중간 위치 계산
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y); // 블록 크기 변경

                // 블록 위치를 중간 지점으로 정렬
                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.x = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                // 잘려나갈 조각(rubble) 생성
                float rubbleHalfScale = deltaX / 2f;
                CreateRubble(
                    new Vector3(isNegativeNum
                            ? lastPosition.x + stackBounds.x / 2 + rubbleHalfScale
                            : lastPosition.x - stackBounds.x / 2 - rubbleHalfScale
                        , lastPosition.y
                        , lastPosition.z),
                    new Vector3(deltaX, 1, stackBounds.y)
                );

                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up; // 거의 일치하면 블록을 바로 위로 정렬하여 쌓기 성공
            }
        }
        else // z축 방향으로 이동중인 경우
        {
            float deltaZ = prevBlockPosition.z - lastPosition.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false; // 왼쪽으로 벗어난 여부 체크

            deltaZ = Mathf.Abs(deltaZ);
            if (deltaZ > ErrorMargin)
            {
                stackBounds.y -= deltaZ;
                if (stackBounds.y <= 0)
                {
                    return false;
                }

                float middle = (prevBlockPosition.z + lastPosition.z) / 2;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.z = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                // 잘려나갈 조각(rubble) 생성
                float rubbleHalfScale = deltaZ / 2f;
                CreateRubble(
                    new Vector3(
                        lastPosition.x
                        , lastPosition.y
                        , isNegativeNum
                            ? lastPosition.z + stackBounds.y / 2 + rubbleHalfScale
                            : lastPosition.z - stackBounds.y / 2 - rubbleHalfScale),
                    new Vector3(stackBounds.x, 1, deltaZ)
                );

                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }
        // 보조 위치값 업데이트 (X 또는 Z 방향의 최종 위치 기록)
        secondaryPosition = (isMovingX) ? lastBlock.localPosition.x : lastBlock.localPosition.z;

        return true; // 블록 쌓기 성공
    }


    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject);
        go.transform.parent = this.transform;

        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localRotation = Quaternion.identity;

        go.AddComponent<Rigidbody>();
        go.name = "Rubble";
    }

    void ComboCheck()
    {
        comboCount++; // 콤보 1증가

        // 최대 콤보 갱신
        if(comboCount > maxCombo)
            maxCombo = comboCount;

        // 5콤보마다 보상효과 적용
        if( (comboCount % 5) == 0)
        {
            Debug.Log("5 Combo Success!");
            stackBounds += new Vector3(0.5f, 0.5f); // 블록 크기 확대
            // 블록 크기 최대 제한
            stackBounds.x =
                (stackBounds.x > BoundSize) ? BoundSize : stackBounds.x;
            stackBounds.y =
                (stackBounds.y > BoundSize) ? BoundSize : stackBounds.y;
        }
    }

    void UpdateScore()
    {
        if(bestScore < stackCount) // 현재 점수가 최고 점수보다 높을 경우
        {
            bestScore = stackCount; // 최고 점수 갱신
            bestCombo = maxCombo; // 최고 콤보 갱신

            // PlayerPrefs에 저장 > 앱을 껐다 켜도 점수 유지
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
        }
    }

    void GameOverEffect()
    {
        int childCount = this.transform.childCount;

        // 가장 위 블록부터 최대 20개를 순차적으로 처리
        for (int i = 1; i < 20; i++)
        {
            if (childCount < i) break; //처리할 자식이 없으면 종료

            GameObject go = transform.GetChild(childCount - i).gameObject; // 뒤에서부터 자식 블록 가져오기

            if (go.name.Equals("Rubble")) continue; // "Rubble" 라는 이름의 오브젝트는 무시

            Rigidbody rigid = go.AddComponent<Rigidbody>(); // Rigidbody를 추가해 중력 및 물리 작용 가능하게 함

            // 위쪽과 좌우 방향으로 무작위 힘을 가함
            rigid.AddForce(
            (Vector3.up * Random.Range(0, 10f) + Vector3.right * (Random.Range(0, 10f) - 5f)) * 100f);
        }
    }

    public void Restart()
    {
        int childCount = transform.childCount; // 게임에 사용된 모든 값 초기화

        for (int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        isGameOver = false;

        lastBlock = null;
        desiredPosition = Vector3.zero;
        stackBounds = new Vector3(BoundSize, BoundSize);

        stackCount = -1;
        isMovingX = true;
        blockTransition = 0f;
        secondaryPosition = 0f;

        comboCount = 0;
        maxCombo = 0;

        prevBlockPosition = Vector3.down;

        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        Spawn_Block();
        Spawn_Block();
    }
}
