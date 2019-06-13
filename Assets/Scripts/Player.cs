using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    //variavel para guardar a velociade atual
    public float speed = 2f;
    //altura do pulo
    public float jumpHeight = 5f;
    //contador das moedas
    public int coinCount = 0;
    //hp do personagem
    public int hp = 10;

    //velocidade da caminhada
    public float walkSpeed = 3f;
    //velocidade da corrida
    public float runSpeed = 8f;
    //força da gravidade (é negativo pois queremos que ele "desça" no eixo Y) 
    public float gravity = -12f;

    // velociade do personagem no eixo Y ( para fazermos o personagem pular e cair)
    [SerializeField]
    private float velocityY;

    // bool para controlar/saber se o personagem está correndo
    [SerializeField]
    private bool running = false;

    //referencia do Script Game
    [SerializeField]
    private Game game_ref;

    //Referência dos (Scripts) dos Textos na tela.
    public TextMeshProUGUI textoMoedas;
    public TextMeshProUGUI textoHP;

    //variável necessária para alimentarmos a função de suavização / interpolação da rotação
    private float smoothRotationVelocity;

    // tempo da suaviação da rotação
    [SerializeField]
    private float smoothRotationTime = 0.2f;

    //variável necessária para alimentarmos a função de suavização / interpolação da velociadde
    private float smoothSpeedVelocity;

    // tempo da suaviação da velocidade
    [SerializeField]
    private float smoothSpeedTime = 0.2f;

    //referência do Transform da Camera
    [SerializeField]
    private Transform cameraT;

    //referência do  Character controller do personagem
    [SerializeField]
    private CharacterController charController;

    //referência do Animator do personagem
    [SerializeField]
    private Animator animator;

    // Função que é executada quando o jogo inicia (mais especificamente, toda vez que uma cena é carregada).
    // Utilizamos essa função para inicialização/setup
    private void Start()
    {
        //pegando referências de outros objetos/componentes para modificá-los via código
        cameraT = Camera.main.transform;
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // setando o valor inicial dos textos nas telas
        textoMoedas.text = "Coins: " + coinCount;
        textoHP.text = "HP: " + hp;
    }

    // Update é uma função chamada uma vez por frame
    private void Update()
    {
        //chamamos a função de movimentação
        walkingRotating();

        //walkSideways();
    }

    //logica para andar rotacionando
    private void walkingRotating()
    {
        // pegar input do jogador
        // Input.getaxis retorna um valor de -1 a 1
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //normalizamos para pegar apenas a direção
        Vector2 inputDir = input.normalized;

        //aqui calculamos a nossa rotação alvo utilizando o arco tangente do input, convertendo de radianos para angulos
        //e somando a rotação da camera em Y para o jogador olhar para a camera quando começar a andar
        float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
        
        //Aqui aplicamos de fato a rotação
        if (inputDir != Vector2.zero) // vector.up pois vamos rotacionar ao redor do eixo Y multiplicado pela nossa rotação alvo (que está sendo suavizada na função smoothDampAngle)
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref smoothRotationVelocity, smoothRotationTime);

        // Aqui setamos o valor da bool running baseado no pressionamento da tecla shift. Running será true sempre que Shift estiver pressionado, e false quando não estiver pressionado
        running = (Input.GetKey(KeyCode.LeftShift));

        // salvamos nessa variavel a velocidade alvo
        // verificamos o valor de running, caso true recebemos o valor da corrida, senão da caminhada.
        // Por fim ainda multiplicamos pela magnitude do vetor do input da direção (caso não estejamos apertando tecla nenhuma, o resultado da multiplicação será 0.
        // caso estejamos apertando, o valor será 1 e não alterará o resultado)
        float targetSpeed = (running) ? runSpeed : walkSpeed * inputDir.magnitude;

        //atualizamos nossa velocidade atual com  a velocidade alvo suavizada
        speed = Mathf.SmoothDamp(speed, targetSpeed, ref smoothSpeedVelocity, smoothSpeedTime);

        //aumentando a aceleração da gravidade
        velocityY += gravity * Time.deltaTime;

        // aqui calculamos o vetor de movimentação total (direções e intesindades)
        // a primeira parte é para mover o personagem para frente (transform.foward) na intensidade speed (novamente multiplicado pelo inputDir.magnitude para impedir a movimentação quando não há teclas pressionadas
        // depois adicionamos no eixo Y (vector3.up == (0,1,0)) na intensidade velocityY (que é a força da gravidade + algum possível pulo)
        Vector3 velocity = transform.forward * speed * inputDir.magnitude + Vector3.up * velocityY;

        //aqui efetuamos a movimentação chamando o método Move do Character controller
        // Na função passamos o vetor de movimentação (multiplicamos por Time.deltaTime pois como estamos na função Update, queremos mover o personagem só a quantidade necessário baseado no último frame)
        charController.Move(velocity * Time.deltaTime);

        //aqui atualizamos a velociade inicial com a velocidade interna do character controller que é mais precisa
        speed = new Vector2(charController.velocity.x, charController.velocity.z).magnitude;

        //aqui resetamos a velocidade no Y para zero, não precisamos mais cair quando já estamos no chão
        if (charController.isGrounded)
        {
            velocityY = 0;
        }

        {
            //aqui calculamos o valor que será passado paraa blend tree das animações de Idle, Walk e Run
            // verificamos o estado de running e dividimos o valor de speed pela velocidade de caminhada ou run adequada para que o valor seja de 0 à 1
            float animationSpeedPercent = ((running) ? speed / runSpeed : speed / walkSpeed * 0.5f) * inputDir.magnitude;

            //aqui passamos o valor que calculamos para o parâmetro da blend tree
            animator.SetFloat("speedPercent", animationSpeedPercent, smoothSpeedTime, Time.deltaTime);
            //aqui setamos o valor do parametro isGrounded do animator
            animator.SetBool("isGrounded", charController.isGrounded);
            //aqui setamos o valor do parametro velocityY do animator
            animator.SetFloat("velocityY", velocityY);
            //aqui setamos o valor do parametro spaceDown do animator
            animator.SetBool("spaceDown", Input.GetKeyDown(KeyCode.Space));
        }

        //aqui chamamos a função de pulo
        Jump();
    }

    private void Jump()
    {
        //caso espaço seja apertado
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //caso estejamos no chão
            if (charController.isGrounded)
            {
                //caso o estado atual do animator se chame "Base Animation"
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Animation"))
                {
                    //vamos para o estado "JumpUp" do animator
                    animator.Play("JumpUp");
                }
                //calculo de kinematics para saber qual a velocidade necessária para pular uma determinada altura (sabendo o valor da gravidade)
                float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
                //adicionamos a força do pulo à variavel da velocidade no Y
                velocityY = jumpVelocity;
            }
        }
    }

    //logica para andar sem rotacionar
    private void walkSideways()
    {
        // pegar input do jogador
        // Input.getaxis retorna um valor de -1 a 1
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //normalizamos para pegar apenas a direção

        Vector3 direction = input.normalized;
        //nossa velocidade será a direção multiplicada pela nossa velocidade
        Vector3 velocity = direction * speed;

        //por fim a distância para percorrer será essa velocidade multiplicada pelo tempo
        Vector3 moveAmount = velocity * Time.deltaTime;

        //aqui iremos mover nosso jogador pela distância que iremos percorrer
        transform.Translate(moveAmount);
    }

    // Função chamada toda vez que esse objeto entrar em um trigger
    private void OnTriggerEnter(Collider other)
    {
        //caso o objeto que colidimos tenha a tag "Coin"
        if (other.transform.CompareTag("Coin"))
        {
            //chamamos função do game para remover essa Coin a lista de Coins
            game_ref.RemoveCoinFromList(other.gameObject);

            // destruímos o objeto que colidimos
            GameObject.Destroy(other.gameObject);
            //aumentamos em 1 nossa quantiade de moedas e hp
            coinCount++;
            hp++;

            //atualizamos o texto na tela com os novos valores de Coins e HP
            textoMoedas.text = "Coins: " + coinCount;
            textoHP.text = "HP: " + hp;
        }
    }

    // Função chamada toda vez que esse objeto entrar em um collider
    private void OnCollisionEnter(Collision collision)
    {
        //caso o objeto que colidimos tenha a tag "Enemy"
        if (collision.transform.CompareTag("Enemy"))
        {
            //diminuimos em 1 o HP
            hp--;
            //atualizamos o HP mostrado na tela
            textoHP.text = "HP: " + hp;

            //destruímos o objeto que colidimos
            GameObject.Destroy(collision.gameObject);
        }
    }
}