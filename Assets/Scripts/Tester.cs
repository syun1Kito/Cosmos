using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text testText;

    // Start is called before the first frame update
    void Start()
    {
        testText.text = "人数" + PlayerSelectController.GetPlayerNumber() + "\n攻撃タイプ" + PlayerSelectController.GetAttackType(1) + PlayerSelectController.GetAttackType(2) + PlayerSelectController.GetAttackType(3) + PlayerSelectController.GetAttackType(4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
