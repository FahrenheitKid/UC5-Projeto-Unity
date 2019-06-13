using UnityEngine;

public class Inimigo : MonoBehaviour
{
    public Transform target; // alvo para o inimigo seguir

    public float speed = 5f;

    [SerializeField]
    private float timerMax; //tempo maximo do timer

    public float timer = 0; // timer em si
    private bool goLeft = true;

    [SerializeField]
    private GameObject bullet;

    // Use this for initialization
    private void Start()
    {
        timer = Time.time;
        //pegamos a referencia do jogador
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.LookAt(target);
        //followTarget();

        if (Time.time >= timer + timerMax)
        {
            goLeft = !goLeft;
            timer = Time.time;
            shootAt(target);
        }

        //moveSideways(goLeft);
    }

    private void followTarget()
    {
        Vector3 input = target.position - transform.position;
        //normalizamos para pegar apenas a direção

        Vector3 direction = input.normalized;
        //nossa velocidade será a direção multiplicada pela nossa velocidade
        Vector3 velocity = direction * speed;

        //por fim a distância para percorrer será essa velocidade multiplicada pelo tempo
        Vector3 moveAmount = velocity * Time.deltaTime;

        //aqui iremos mover nosso jogador pela distância que iremos percorrer
        transform.Translate(moveAmount);
    }

    private void moveSideways(bool goLeft)
    {
        Vector3 direction;
        if (goLeft) direction = Vector3.left;
        else direction = Vector3.right;

        // Vector3 direction = (goLeft) ? Vector3.left : Vector3.right;

        Vector3 velocity = direction * speed;
        Vector3 moveAmount = velocity * Time.deltaTime;

        transform.Translate(moveAmount);
    }

    //função para atirar num alvo
    private void shootAt(Transform target)
    {
        Vector3 instantiatePosition = transform.position + new Vector3(0, 0, 1);
        instantiatePosition.y = target.position.y;

        GameObject go = Instantiate(bullet, instantiatePosition, Quaternion.identity);
        //go.transform.LookAt(target);
    }
}