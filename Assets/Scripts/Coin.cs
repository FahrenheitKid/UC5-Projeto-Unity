using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    //velocidade da rotação
    float rotationSpeed = 180;
	
	// Update is called once per frame
	void Update () {

        //aqui apenas rotacionamos as moedas no eixo Y na velocidade rotationSpeed
        //como está no update, ela ficará rotacionando para sempre
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
    }
}
