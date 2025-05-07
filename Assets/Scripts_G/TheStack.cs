using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    private const float BoundSize = 3.5f; // ��� �⺻ �ʺ�
    private const float MovingBoundsSize = 3f; // ����� �̵��� �� �ִ� �Ÿ� ũ��
    private const float StackMovingSpeed = 5.0f; // ��ü ������ �ö󰡴� �ӵ�
    private const float BlockMovingSpeed = 3.5f; // ����� �¿�� �����̴� �ӵ�
    private const float ErrorMargin = 0.1f; // ���е� ���� ���� (�߸��� ����)

    public GameObject originBlock = null; // ���� ��� ������

    private Vector3 prevBlockPosition; // ���� ����� ��ġ
    private Vector3 desiredPosition; // ��ü ������ ��ǥ ��ġ (�ε巯�� ī�޶� �̵���)
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize); // ���� ����� �ʺ�

    Transform lastBlock = null; // ���� ������ ������ ���
    float blockTransition = 0f; // ��� �̵��� ���� �ð� ���� ����
    float secondaryPosition = 0f; // ���� ��ġ (X �Ǵ� Z��)

    int stackCount = -1; // ������� ���� ��� ��
    public int Score { get { return stackCount; } }
    int comboCount = 0; // �������� ��Ȯ�� ����� ���� Ƚ��
    public int Combo { get { return comboCount; } }

    private int maxCombo = 0; // ���� ���� �������� ������ �޺� ��
    
    public int MaxCombo { get =>  maxCombo; }

    public Color prevColor;
    public Color nextColor;

    bool isMovingX = true;

    int bestScore = 0; // �ְ� ���� ���� ����
    public int BestScore { get => bestScore; }

    int bestCombo = 0; // �ְ� �޺� ���� ����
    public int BestCombo { get => bestCombo; }

    private const string BestScoreKey = "BestScore";
    private const string BestComboKey = "BestCombo";

    private bool isGameOver = true;

    void Start()
    {
        if (originBlock == null)
        {
            Debug.Log("OriginBlock is NULL"); // �������� �������� �ʾ��� ���
            return;
        }

        // ����� �ְ� ���� �ҷ����� (������ �⺻�� 0)
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestCombo = PlayerPrefs.GetInt(BestComboKey, 0);

        // ���� �� ����� �� ���� ������ �������� ����
        prevColor = GetRandomColor();
        nextColor = GetRandomColor();

        prevBlockPosition = Vector3.down; // �ʱ� ��Ϻ��� �Ʒ��� �ִ� ���� ��ġ

        Spawn_Block(); // ù ��° ��� ����
        Spawn_Block();
    }

    void Update()
    {
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0)) // ���콺 Ŭ���� ��� ����
        {
            if(PlaceBlock())
            {
                Spawn_Block();
            }
            else
            {
                // ���� ����
                UpdateScore();
                isGameOver = true; // ���� ���� ���� ����
                GameOverEffect();
                UIManager.Instance.SetScoreUI(); // ���ھ� UI ����
            }
            
        }

        MoveBlock();
        // ���� ��ü�� ���� �ε巴�� �̵� (ī�޶� ����ø��� ȿ��)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed * Time.deltaTime);
    }

    bool Spawn_Block()
    {
        // ���� ��� ���� (���� ��� ��ġ�� ���� �񱳿� ����)
        if (lastBlock != null)
            prevBlockPosition = lastBlock.localPosition;

        GameObject newBlock = null;
        Transform newTrans = null;

        newBlock = Instantiate(originBlock); // �� ��� ����

        if (newBlock == null)
        {
            return false;
        }

        ColorChange(newBlock); // �� ��Ͽ� �� ����

        newTrans = newBlock.transform;
        // �θ� ���� (���� �ȿ� ����� �ڽ����� ����)
        newTrans.parent = this.transform;
        // ��ġ ���� ���� ��� ���� ����
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        // ȸ�� �ʱ�ȭ
        newTrans.localRotation = Quaternion.identity;
        // ��� ũ�� ����
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++; // ��� ���� ����

        desiredPosition = Vector3.down * stackCount; // ī�޶�/������ �̵��� ��ǥ ��ġ ������Ʈ
        blockTransition = 0f; // ��� �̵� �ʱ�ȭ

        lastBlock = newTrans; // ������ ��� ����

        isMovingX = !isMovingX;

        UIManager.Instance.UpdateScore();
        return true;
    }

    Color GetRandomColor()
    {
        // 100~250 ������ ���� RGB ���� ����
        float r = Random.Range(100f, 250f) / 255f;
        float g = Random.Range(100f, 250f) / 255f;
        float b = Random.Range(100f, 250f) / 255f;

        return new Color(r, g, b);
    }

    void ColorChange(GameObject go)
    {
        // prevColor > nextColor�� �ε巴�� ����
        // 0~1 ������ �� (10�� ��� ����)
        Color applyColor = Color.Lerp(prevColor, nextColor, (stackCount % 11) / 10f);

        Renderer rn = go.GetComponent<Renderer>(); // ��Ͽ� ����

        if (rn == null)
        {
            return;
        }

        rn.material.color = applyColor;
        // ī�޶� ������ ��Ϻ��� �ణ ��ο� ������ ����
        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);

        // ���� ������ ������ ���������� ���ο� �� �غ�
        if (applyColor.Equals(nextColor) == true)
        {
            prevColor = nextColor;
            nextColor = GetRandomColor();
        }
    }

    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;

        // PingPong - �¿� �ݺ� �̵� ȿ�� ����
        float movePosition = Mathf.PingPong(blockTransition, BoundSize) - BoundSize / 2;

        if(isMovingX)
        {
            lastBlock.localPosition = new Vector3(
                movePosition * MovingBoundsSize, stackCount, secondaryPosition); // X���̵� > Z ����
        }
        else
        {
            lastBlock.localPosition = new Vector3(
                secondaryPosition, stackCount, movePosition * MovingBoundsSize); // Z�� �̵� > X ����
        }
    }

    bool PlaceBlock()
    {
        Vector3 lastPosition = lastBlock.transform.localPosition; // ���� ����� ��ġ

        if (isMovingX) // x�� �������� �̵����� ���
        {
            float deltaX = prevBlockPosition.x - lastPosition.x; // ���� ��ϰ��� ��ġ ����
            bool isNegativeNum = (deltaX < 0) ? true : false; // �������� ��� ���� üũ

            deltaX = Mathf.Abs(deltaX); // ���� ���밪���� ��
            if (deltaX > ErrorMargin)
            {
                stackBounds.x -= deltaX; // ��� �ʺ� ���̱�
                if (stackBounds.x <= 0) // ����� �ʹ� �۾����� ���ӿ���
                {
                    return false;
                }

                float middle = (prevBlockPosition.x + lastPosition.x) / 2; // �߰� ��ġ ���
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y); // ��� ũ�� ����

                // ��� ��ġ�� �߰� �������� ����
                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.x = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                // �߷����� ����(rubble) ����
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
                lastBlock.localPosition = prevBlockPosition + Vector3.up; // ���� ��ġ�ϸ� ����� �ٷ� ���� �����Ͽ� �ױ� ����
            }
        }
        else // z�� �������� �̵����� ���
        {
            float deltaZ = prevBlockPosition.z - lastPosition.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false; // �������� ��� ���� üũ

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

                // �߷����� ����(rubble) ����
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
        // ���� ��ġ�� ������Ʈ (X �Ǵ� Z ������ ���� ��ġ ���)
        secondaryPosition = (isMovingX) ? lastBlock.localPosition.x : lastBlock.localPosition.z;

        return true; // ��� �ױ� ����
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
        comboCount++; // �޺� 1����

        // �ִ� �޺� ����
        if(comboCount > maxCombo)
            maxCombo = comboCount;

        // 5�޺����� ����ȿ�� ����
        if( (comboCount % 5) == 0)
        {
            Debug.Log("5 Combo Success!");
            stackBounds += new Vector3(0.5f, 0.5f); // ��� ũ�� Ȯ��
            // ��� ũ�� �ִ� ����
            stackBounds.x =
                (stackBounds.x > BoundSize) ? BoundSize : stackBounds.x;
            stackBounds.y =
                (stackBounds.y > BoundSize) ? BoundSize : stackBounds.y;
        }
    }

    void UpdateScore()
    {
        if(bestScore < stackCount) // ���� ������ �ְ� �������� ���� ���
        {
            bestScore = stackCount; // �ְ� ���� ����
            bestCombo = maxCombo; // �ְ� �޺� ����

            // PlayerPrefs�� ���� > ���� ���� �ѵ� ���� ����
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
        }
    }

    void GameOverEffect()
    {
        int childCount = this.transform.childCount;

        // ���� �� ��Ϻ��� �ִ� 20���� ���������� ó��
        for (int i = 1; i < 20; i++)
        {
            if (childCount < i) break; //ó���� �ڽ��� ������ ����

            GameObject go = transform.GetChild(childCount - i).gameObject; // �ڿ������� �ڽ� ��� ��������

            if (go.name.Equals("Rubble")) continue; // "Rubble" ��� �̸��� ������Ʈ�� ����

            Rigidbody rigid = go.AddComponent<Rigidbody>(); // Rigidbody�� �߰��� �߷� �� ���� �ۿ� �����ϰ� ��

            // ���ʰ� �¿� �������� ������ ���� ����
            rigid.AddForce(
            (Vector3.up * Random.Range(0, 10f) + Vector3.right * (Random.Range(0, 10f) - 5f)) * 100f);
        }
    }

    public void Restart()
    {
        int childCount = transform.childCount; // ���ӿ� ���� ��� �� �ʱ�ȭ

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
