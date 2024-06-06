using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    /// <summary>
    /// �ו����i�[����ꏊ�̃v���n�u
    /// </summary>
    public GameObject storePrefab;
    public GameObject wallPrefab;
    /// <summary>
    /// �N���A�������Ƃ������e�L�X�g��GameObject
    /// </summary>
    public GameObject clearText;
    int[,] map; // �}�b�v�̌��f�[�^�i�����j
    GameObject[,] field;    // map �����ɂ����I�u�W�F�N�g�̊i�[��
    Vector2[,] initialPositions;

    bool isCleared()
    {
        List<Vector2Int> goals = new List<Vector2Int>();
        // �i�[�ꏊ
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                    // �i�[�ꏊ�ł���ꍇ
                }
            }
        }
        // �i�[�ꏊ�ɔ������邩���ׂ�
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x]; //�S�[���̍��W�ɉ������邩�Ƃ��Ă���

            if (f == null || f.tag != "Box")
            {
                return false;
                // �i�[�ꏊ�ɔ����Ȃ��A�Ƃ����P�[�X����ł�����΃N���A���ĂȂ��Ɣ��肷��
            }
        }
        return true;
    }


    /// <summary>
    /// number �𓮂���
    /// </summary>
    /// <param name="number">����������</param>
    /// <param name="moveFrom">�ړ����C���f�b�N�X</param>
    /// <param name="moveTo">�ړ���C���f�b�N�X</param>
    /// <returns></returns>
    bool MoveNumber(Vector2Int moveFrom, Vector2Int moveTo)
    {
        // �����Ȃ��ꍇ�� false ��Ԃ�
        if (moveTo.y < 1 || moveTo.y >= field.GetLength(0) - 1)
            return false;
        if (moveTo.x < 1 || moveTo.x >= field.GetLength(1) - 1)
            return false;

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success)
                return false;
        }


        //if (map[moveTo] == 2)
        //{
        //    // �ړ������i���Ȃ灨�A���Ȃ灩���v�Z����j
        //    int velocity = moveTo - moveFrom;
        //    bool success = MoveNumber(2, moveTo, moveTo + velocity);

        //    if (!success)
        //    {
        //        return false;
        //    }
        //}   // �v���C���[�̈ړ���ɔ��������ꍇ�̏���

        // �v���C���[�E���̋��ʏ���
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        // �I�u�W�F�N�g�̃V�[����̍��W�𓮂���
        //field[moveTo.y, moveTo.x].transform.position =
        //    new Vector3(moveTo.x, -1 * moveTo.y, 0);
        //�v���C���[or���̃I�u�W�F�N�g����AMove�R���|�[�l���g��
        Move move = field[moveTo.y, moveTo.x].GetComponent<Move>();
        //Move�R���|�[�l���g�ɑ΂��āA�����Ɩ��߂���
        move.MoveTo(new Vector3(moveTo.x, -1 * moveTo.y, 0));
        return true;
    }

    /// <summary>
    /// �v���C���[�̍��W�𒲂ׂĎ擾����
    /// ���jGetPlayerPosition 
    /// </summary>
    /// <returns>�v���C���[�̍��W</returns>
    Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] != null
                    && field[y, x].tag == "Player")
                {
                    // �v���C���[��������
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);  // ������Ȃ�����
    }

    void PrintArray()
    {
        string debugText = "";

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                debugText += map[y, x].ToString() + ",";
            }

            debugText += "\n";
        }

        Debug.Log(debugText);
    }

    void Start()
    {

        Screen.SetResolution(1280, 720, false);

        clearText.SetActive(false);
        map = new int[,]
        {
            { 4, 4, 4, 4, 4, 4, 4},
            { 4, 0, 0, 0, 0, 0, 4},
            { 4, 3, 0, 1, 0, 3, 4},
            { 4, 0, 0, 2, 0, 0, 4},
            { 4, 2, 0, 3, 0, 2, 4},
            { 4, 0, 0, 0, 0, 0, 4},
            { 4, 4, 4, 4, 4, 4, 4},
        };  // 0: �����Ȃ�, 1: �v���C���[, 2: ��, 3:�i�[�ꏊ, 4:��

        field = new GameObject
        [
            map.GetLength(0),
            map.GetLength(1)
        ];  // map �̍s��Ɠ������ڂ̔z��������ЂƂ����

        PrintArray();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    // �����Ƀv���C���[���o��
                    GameObject instance =
                        Instantiate(playerPrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                    // �v���C���[�͂P�����Ȃ̂Ŕ�����
                    field[y, x] = instance; // �v���C���[��ۑ����Ă���
                    //break;
                } // �v���C���[���o��
                else if (map[y, x] == 2)
                {
                    GameObject instance =
                       Instantiate(boxPrefab,
                       new Vector3(x, -1 * y, 0),
                       Quaternion.identity);
                    field[y, x] = instance; // ����ۑ����Ă���
                } //�����o��
                else if (map[y, x] == 3)
                {
                    GameObject instance =
                       Instantiate(storePrefab,
                       new Vector3(x, -1 * y, 0),
                       Quaternion.identity);
                    field[y, x] = instance; //�i�[�ꏊ���o��
                }
                else if (map[y, x] == 4)
                {
                    GameObject instance =
                        Instantiate(wallPrefab,
                        new Vector3(x, -1 * y, 0),
                        Quaternion.identity);
                    field[y, x] = instance;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var playerPosition = GetPlayerIndex();
            MoveNumber(playerPosition, new Vector2Int(playerPosition.x + 1, playerPosition.y));    // ���Ɉړ�
            if (isCleared())
            {
                clearText.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var playerPosition = GetPlayerIndex();
            MoveNumber(playerPosition, new Vector2Int(playerPosition.x - 1, playerPosition.y));    // ���Ɉړ�
            if (isCleared())
            {
                clearText.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var playerPosition = GetPlayerIndex();
            MoveNumber(playerPosition, new Vector2Int(playerPosition.x, playerPosition.y - 1));    // ���Ɉړ�
            if (isCleared())
            {
                clearText.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var playerPosition = GetPlayerIndex();
            MoveNumber(playerPosition, new Vector2Int(playerPosition.x, playerPosition.y + 1));    // ���Ɉړ�
            if (isCleared())
            {
                clearText.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Start();
        }
    }
}