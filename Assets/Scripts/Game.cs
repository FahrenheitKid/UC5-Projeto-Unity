using System.Collections;
using System.Collections.Generic;
using System.Linq; // algumas funcções adicionais
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    //variáveis para guardar referência dos prefabs
    public GameObject coinPrefab;
    public GameObject inimigoPrefab;

    // lista com posições para spawnar os inimigos
    public List<Transform> spawnPoints = new List<Transform>();
    //lista com as moedas do jogo
    public List<Coin> listCoins = new List<Coin>();
    //lista com os inimigos
    public List<Inimigo> listInimigos = new List<Inimigo>();

    //quantidade de inimigos para spawnar
    [SerializeField]
    int quantityOfEnemies = 5;

    // Use this for initialization
    void Start () {

        //aqui instanciamos 100 moedas num padrão 10x10
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                //aqui salvamos numa variavel temporária para guardar o GameObject ue estamos instanciando
                // Instanciamos o GameObject salvo na variavel coinPrefab, na posição passada, sem alteração na sua rotação (por isso o Quaternion.identity)
                GameObject go = Instantiate(coinPrefab, new Vector3(i * 1.0f, 2, j * -1.0f), Quaternion.identity);

                //e aqui adicionamos o objeto criado na lista de Coins
                // Temos que colocar GetComponent<Coin> pois é uma lista do tipo Coins (componente do tipo Script) e não uma lista do tipo GameObject
                listCoins.Add(go.GetComponent<Coin>());
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
		
        //caso a quantidade de moedas na lista seja 80
        if(listCoins.Count == 80)
        {
            print("POUCAS MOEDAS");

            //for para criar inimigos
            for(int i =0; i < quantityOfEnemies; i++)
            {

                
                if (spawnPoints.Any()) // caso tenha algum elemento na lista de spawnPoints
                {
                    //sorteamos uma posição da lista
                    int pos = Random.Range(0, spawnPoints.Count - 1);
                    //e chamamos a função de spawn que criamos mais embaixo no código, passando como parametro a posição a ser criado o inimigo
                    spawnEnemy(spawnPoints[pos].position);

                    //depois de utilizar essa posição, temos que remover sua entrada da lista
                    spawnPoints.RemoveAt(pos);
                }
                else break;
                
            }
        }

        //caso não haja mais moedas na lista, significa que o player achou todas elas
        if(listCoins.Count <= 0)
        {
            //carregamos uma nova cena
            SceneManager.LoadScene(1);
        }
       
	}

    //função para removermos uma moeda da lista de Coins, pasasndo como parametro o GameObject da moeda
    //como é public, podemos chamar essa função de outro script (o que fazemos, no script Player)
    public void RemoveCoinFromList(GameObject coin)
    {
        //caso o gameobject passado tenha o componente Coin
        if(coin.GetComponent<Coin>())
        {
            //removemos essa moeda da lista de Coins
            listCoins.Remove(coin.GetComponent<Coin>());
        }
    }

    //função para spawnar inimigos em determinada posição
    void spawnEnemy(Vector3 position)
    {
        //instanciamos o inimigo na posição passada
        GameObject go = Instantiate(inimigoPrefab, position, Quaternion.identity);

        //e após, adicionamos esse inimigo na lista de inimigos
        listInimigos.Add(go.GetComponent<Inimigo>());

    }
}
