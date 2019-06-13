using UnityEngine;

public class Bullet : MonoBehaviour
{
    //alvo da bala
    public Transform target;
    //velocidade da bala
    public float speed = 5f;
    //direção da bala
    private Vector3 direction;

    //variavel do timer para autodestruir a bala
    private float timerSelfDestruct = 0;
    // valor maximo do timer
    private float timerSelfDestruct_Max = 5;

    // Use this for initialization
    private void Start()
    {
        //iniciamos o timer com o valor do Time.time (que é quanto tempo já passou desde que o jogo iniciou, como se fosse um relógio interno do jogo)
        timerSelfDestruct = Time.time;
        //pegamos uma referencia do Transform do jogador para achar sua posição
        Transform playerT = GameObject.FindGameObjectWithTag("Player").transform;

        //checamos se a variavel playerT tem um valor (como antes fizemos uma busca por tag, é possivel que essa busca não tenha encontrado nada)
        if (playerT && playerT != null)
        {
            //aqui traversamos por todos os filhos do Player para achar o filho chamado "Camera Target"
            //Fazemos isso para que o tiro vá focado nesse objeto (que fica na altura do peito), pois a posição do objeto Player mesmo acaba ficando no pé do modelo
            foreach (Transform child in playerT)
            {
                if (child.name == "Camera Target")
                {
                    //ao achar o filho com esse nome, setamos nosso alvo como esse filho
                    target = child;
                    //break para sair do foreach
                    break;
                }
            }

            //caso não achemos o filho, miramos no pé do player mesmo
            if (!target || target == null)
            {
                target = playerT;
            }
        }

        
        // aqui vamos pegar a direção do nosso alvo fazendo primeiro a diferença da nossa posição e da posição do alvo
        Vector3 input = target.position - transform.position;

        //e em seguida normalizamos para pegar apenas a direção

        direction = input.normalized;
    }


    // Update is called once per frame
    private void Update()
    {
        //transladamos o objeto na direção que já calculamos, na intesidade de speed e multiplciado pelo tempo do ultimo frame
        transform.Translate(direction * speed * Time.deltaTime);

        //aqui checamos se tempo atual de jogo já é o tempo que o timer deve ter "estourado"
        //no caso, o timer + o seu limite
        // ou caso a distância entre a nossa posição e a posição do alvo seja maior que 30, nos destruimos
        if ((Time.time >= timerSelfDestruct + timerSelfDestruct_Max)
            || (Vector3.Distance(transform.position, target.transform.position) >= 30f))
        {
                Destroy(gameObject);
            
            //resetamos o timer
            timerSelfDestruct = Time.time;
        }
    }

    //função para colisão com Triggers
    private void OnTriggerEnter(Collider other)
    {
        //caso colidirmos com um Player ou Obstáculo, nos destruimos
        if (other.transform.CompareTag("Player") ||
            other.transform.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    //função para colisão com Colliders
    private void OnCollisionEnter(Collision collision)
    {
        //caso colidirmos com um Player ou Obstáculo ou Inimigo, nos destruimos
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}