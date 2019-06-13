using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {


    //valor da rotação no X
    float yaw;
    //valor da rotação no Y
    float pitch;
    
    //sensitividade do mouse
    [SerializeField]
    float mouseSensitivity = 3f;
    //distância da camera a partir do player, no eixo X
    [SerializeField]
    float distanceFromTargetX;
    //distância da camera a partir do player, no eixo y
    [SerializeField]
    float distanceFromTargetY;

    //valor minimo da rotação no pitch
    [SerializeField]
    float pitchMin = -40;
    //valor maximo na rotação no pitch
    [SerializeField]
    float pitchMax = 80;

    //bool para controlar se o cursor deve ser travado
    [SerializeField]
    bool lockCursor;

    //vector3 com nossa rotação atual
    Vector3 currentRotation;
    //valor do tempo de suavização
    [SerializeField]
    float rotationSmoothTime = 0.12f;

    //variavel para passar para a função de suavização
    [SerializeField]
    Vector3 rotationSmoothVelocity;

    [SerializeField]
    Transform target; // rotacionar em volta de um alvo, no caso o player

    //máscara para definir quais objetos a camera deve "colidir" e não bloquear a visão
    [SerializeField]
    LayerMask cameraLayerMask;

    // Use this for initialization
    void Start () {

        //caso lockCursor seja true
        if(lockCursor)
        {
            //trava o cursor no centro da tela
            Cursor.lockState = CursorLockMode.Locked;
            // deixa o cursor invisivel
            Cursor.visible = false;
        }

      
	}
	
	// Update is called once per frame
    // 
	void LateUpdate () {

        
        // input do mouse
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        //limita a rotacao no y
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        //alvo da rotação, que vai ser nosso picth e yaw
        Vector3 targetRotation = new Vector3(pitch, yaw);


        //suaviza~mos a rotação
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        //e aplicamos a rotação
        transform.eulerAngles = currentRotation;

        // colocamos a camera numa posicao logo atras do jogador
        transform.position = target.position - transform.forward * distanceFromTargetX + transform.up * distanceFromTargetY;

        //checamos se ela vai estar atrás de uma parede
        CheckWall();


	}

    void CheckWall()
    {
        //variavel do tipo RaycastHit que é onde um raycast colide
        RaycastHit hit;
        // inicio do raio
        Vector3 raystart = target.position;
        //direção da posição
        Vector3 dir = (transform.position - target.position).normalized;

        //distancia do raio ( que é a distancia do player até a camera
        float dist = Vector3.Distance(transform.position, target.position);

        //caso o raio saindo da posição rayStart, indo na direção dir, com tamanho de dist colida com algo
        if(Physics.Raycast(raystart,dir, out hit, dist, cameraLayerMask))
        {

            //pegamos a distancia do inicio até a colisao
            float hitDistance = hit.distance;

            // e salvamos a posição nova da camera que sera a posição do player + essa distancia da colisao
            Vector3 castCenterHit = target.position + (dir.normalized * hitDistance);

            //atualizamos essa posição
            transform.position = castCenterHit;
        }

    }
}
