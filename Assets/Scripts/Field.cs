using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{

    public enum FieldState
    {
        none = 0,
        _1P,
        _2P,
        _3P,
        _4P,
        Mars,
        Mercury,
        Venus,
        Jupiter

    }


    [SerializeField] GameObject field;
    [SerializeField] GameObject[] small_field = new GameObject[9];



    public const int FIELD_X_NUM = 10;
    public const int FIELD_Z_NUM = 10;
    float FIELD_WIDTH;
    float FIELD_HEIGHT;

    int[] deathZone;
    static int deathNum;
    float deathWaitTime = 0.2f;
    int deathFlag1;
    int deathFlag2;

    GameObject[,] field_list = new GameObject[FIELD_X_NUM, FIELD_Z_NUM];

    //SpriteRenderer[,] field_sprite = new SpriteRenderer[FIELD_X_NUM, FIELD_Z_NUM];
    int[,] field_map = { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                         { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },};

    int[,] pre_field_map = new int[FIELD_X_NUM, FIELD_Z_NUM];

    /* Field(float _X,float _Z) {

         this.X = _X;
         this.Z = _Z;

     }*/

    // Field[,] _field = new Field[10, 10];

    /*void PutBall()
    {
       GameObject newball = Instantiate(ball, new Vector3(X, 1, Z),Quaternion.identity) as GameObject;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        FIELD_WIDTH = field.transform.localScale.x / FIELD_X_NUM;
        FIELD_HEIGHT = field.transform.localScale.z / FIELD_Z_NUM;

        deathZone = new int[] { 0, 0 };
        deathNum = 0;
        deathFlag1 = 0;
        deathFlag2 = 0;

        for (int i = 0; i < FIELD_X_NUM; i++)
        {
            for (int j = 0; j < FIELD_Z_NUM; j++)
            {
                GameObject newField = Instantiate(small_field[(int)FieldState.none], new Vector3(i - FIELD_X_NUM / 2 + FIELD_WIDTH / 2, 0.02f, j - FIELD_Z_NUM / 2 + FIELD_HEIGHT / 2), Quaternion.identity) as GameObject;
                field_list[i, j] = newField;
                // field_sprite[i, j] = field_list[i,j].GetComponent<SpriteRenderer>();
            }
        }

        /*for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                _field[i,j] = new Field(i-5, j-5);
                //_field[i, j] = field;
                //_field[i, j].PutBall();
            }
        }*/

        // Field field = new Field(0,0);
        //field.PutBall();
    }

    // Update is called once per frame
    void Update()
    {

        FieldUpdate();


        // field_map[Random.Range(0, 10), Random.Range(0, 10)] = Random.Range(0, 9);

        // FieldChange(0,0,FieldState._1P);

    }

    public static int getDeathNum() {
        return deathNum;
    }

    public void SetDeathZone(int player)
    {
        deathNum++;
        if (deathNum <= 2)
        {
            deathZone[deathNum-1] = player;
        }
    }

    public void FieldChange(int x, int z, FieldState fieldState)
    {

        if (x <= 10 && x >= 1 && z <= 10 && z >= 1)
        {
            field_map[x - 1, z - 1] = (int)fieldState;
        }
    }

    void FieldUpdate()
    {
        DeathZoneUpdate();

        for (int i = 0; i < FIELD_X_NUM; i++)
        {
            for (int j = 0; j < FIELD_Z_NUM; j++)
            {
                if (field_map[i, j] != pre_field_map[i, j])
                {
                    Destroy(field_list[i, j]);
                    GameObject newField = Instantiate(small_field[field_map[i, j]], new Vector3(i - FIELD_X_NUM / 2 + FIELD_WIDTH / 2, 0.02f, j - FIELD_Z_NUM / 2 + FIELD_HEIGHT / 2), Quaternion.identity) as GameObject;
                    field_list[i, j] = newField;
                    pre_field_map[i, j] = field_map[i, j];
                }
            }
        }


    }

    void DeathZoneUpdate()
    {

        if (deathZone[0] != 0)
        {
            if (deathFlag1 == 0)
            {
                deathFlag1 = 1;
                StartCoroutine(Death1(FieldState.none + deathZone[0], FieldState.none + 4 + deathZone[0]));
            }
            else if (deathFlag1 == 2)
            {
                for (int i = 1; i <= FIELD_X_NUM; i++)
                {
                    FieldChange(i, 1, FieldState.none + 4 + deathZone[0]);
                    FieldChange(i, 10, FieldState.none + 4 + deathZone[0]);
                }

                for (int i = 1; i <= FIELD_X_NUM; i++)
                {
                    FieldChange(1, i, FieldState.none + 4 + deathZone[0]);
                    FieldChange(10, i, FieldState.none + 4 + deathZone[0]);
                }
            }
            
            

            
        }

        if (deathZone[1] != 0)
        {
            if (deathFlag2 == 0)
            {
             
                deathFlag2 = 1;
                StartCoroutine(Death2(FieldState.none + deathZone[1], FieldState.none + 4 + deathZone[1]));

            }
            else if (deathFlag2 == 2)
            {
                for (int i = 2; i <= FIELD_X_NUM - 1; i++)
                {
                    FieldChange(i, 2, FieldState.none + 4 + deathZone[1]);
                    FieldChange(i, 9, FieldState.none + 4 + deathZone[1]);
                }

                for (int i = 2; i <= FIELD_X_NUM - 1; i++)
                {
                    FieldChange(2, i, FieldState.none + 4 + deathZone[1]);
                    FieldChange(9, i, FieldState.none + 4 + deathZone[1]);
                }
            }
          
            
        }
    }

    public IEnumerator Death1(Field.FieldState fieldState1, Field.FieldState fieldState2)
    {
        for (int i = 1; i <= FIELD_X_NUM; i++)
        {
            FieldChange(1, i, fieldState1);
            FieldChange(i, FIELD_X_NUM, fieldState1);
            FieldChange(FIELD_Z_NUM, FIELD_X_NUM - i + 1, fieldState1);
            FieldChange(FIELD_Z_NUM - i + 1, 1, fieldState1);
            yield return new WaitForSeconds(deathWaitTime);

        }

        for (int i = 1; i <= FIELD_X_NUM; i++)
        {
            FieldChange(1, i, fieldState2);
            FieldChange(i, FIELD_X_NUM, fieldState2);
            FieldChange(FIELD_Z_NUM, FIELD_X_NUM - i + 1, fieldState2);
            FieldChange(FIELD_Z_NUM - i + 1, 1, fieldState2);
            yield return new WaitForSeconds(deathWaitTime);

        }

        deathFlag1 = 2;
    }

    public IEnumerator Death2(Field.FieldState fieldState1, Field.FieldState fieldState2)
    {
        for (int i = 2; i <= FIELD_X_NUM - 1; i++)
        {
            FieldChange(2, i, fieldState1);
            FieldChange(i, FIELD_X_NUM - 1, fieldState1);
            FieldChange(FIELD_Z_NUM - 1, FIELD_X_NUM - i + 1, fieldState1);
            FieldChange(FIELD_Z_NUM - i + 1, 2, fieldState1);
            yield return new WaitForSeconds(deathWaitTime);

        }

        for (int i = 2; i <= FIELD_X_NUM - 1; i++)
        {
            FieldChange(2, i, fieldState2);
            FieldChange(i, FIELD_X_NUM - 1, fieldState2);
            FieldChange(FIELD_Z_NUM - 1, FIELD_X_NUM - i + 1, fieldState2);
            FieldChange(FIELD_Z_NUM - i + 1, 2, fieldState2);
            yield return new WaitForSeconds(deathWaitTime);

        }

        deathFlag2 = 2;
    }


}
