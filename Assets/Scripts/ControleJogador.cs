using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControleJogador : MonoBehaviour
{
    public Rigidbody jogador;
    public GameObject cenario;
    public float velocidadeCenario;
    public float distanciaRaia;
    private int raiaAtual;
    private Vector3 target = new Vector3();
    private Vector2 initialPosition;

    public GameObject chao;
    public GameObject bloco;
    public GameObject moeda;
    private int estagioAtual = 1;

    // Start is called before the first frame update
    void Start()
    {
        raiaAtual = 1;
        target = jogador.transform.position;
        montarCenario();
    }

    // Update is called once per frame
    void Update() {

        transform.Rotate(new Vector3(90,0,0) * Time.deltaTime);

        int novaRaia = -1;

        // Teclado
        if (Input.GetKeyDown(KeyCode.RightArrow) && raiaAtual < 2) {
            novaRaia = raiaAtual + 1;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && raiaAtual > 0) {
            novaRaia = raiaAtual - 1;
        }

        //mouse
        if (Input.GetMouseButtonDown(0)) {
            initialPosition = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(0)) {
            if (Input.mousePosition.x > initialPosition.x && raiaAtual < 2) {
                novaRaia = raiaAtual + 1;
            } else if (Input.mousePosition.x < initialPosition.x && raiaAtual > 0) {
                novaRaia = raiaAtual - 1;
            }
        }

        // Touch
        if(Input.touchCount >= 1) {
            if(Input.GetTouch(0).phase == TouchPhase.Began) {
                initialPosition = Input.GetTouch(0).position;
            } else if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled){
                if (Input.GetTouch(0).position.x > initialPosition.x && raiaAtual < 2) {
                    novaRaia = raiaAtual + 1;
                } else if (Input.GetTouch(0).position.x < initialPosition.x && raiaAtual > 0) {
                    novaRaia = raiaAtual - 1;
                }
            }
        }

        if (novaRaia>=0) {
            raiaAtual = novaRaia;
            // X -> Lateral, Y -> Altura, Z -> Profundidade
            target = new Vector3( (raiaAtual - 1) * distanciaRaia,
            jogador.transform.position.y, jogador.transform.position.z);
            //jogador.transform.position = pos;
        }
        if (jogador.transform.position.x != target.x) {
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5 * Time.deltaTime);
        }
        //move o chao
        cenario.transform.Translate(0, 0, velocidadeCenario * Time.deltaTime * -1);

        // criando o chao
        float cenarioz = cenario.transform.position.z;
        float estagio = Mathf.Floor(((cenarioz - 50.0F) / -100.0F)) + 1;
        if (estagio > estagioAtual) {
            GameObject chao2 = Instantiate(chao);
            // O 42 é a posição do cenário
            float chao2z = ((100 * estagioAtual) + 42) + cenarioz;
            float chaox = chao.transform.position.x;
            float chaoy = chao.transform.position.y;
            chao2.transform.SetParent(cenario.transform);
            chao2.transform.position = new Vector3(chaox, chaoy, chao2z);
            estagioAtual++;
            montarCenario();
        }

    }

    void montarCenario() {
        for ( int i = 2; i < 10; i++ ) {
            // percorre as 3 colunas
            int[] elemento = new int[3];
            for ( int j = 0; j < 3; j++ ) {
                elemento[j] = Random.Range(0, 3);
                // 0 = nada; 1 = bloco; 2 = moeda
                if ( elemento[0] == 1 && elemento[1] == 1 && elemento[2] == 1 ) {
                    // previne que não tenha 3 blocos na mesma linha
                    elemento[2] = 0;
                }
                if ( elemento[j] == 1 ) { instanciarBloco(i, j); }
                else if ( elemento[j] == 2 ) { instanciarMoeda(i, j); }
            }
        }
    }

    void instanciarBloco(int posicaoz, int posicaox) {
        GameObject bloco2 = Instantiate(bloco);
        float posz = ((10*posicaoz)+((estagioAtual-1)*100)) + cenario.transform.position.z;
        float posx = (posicaox-1)*distanciaRaia;
        bloco2.transform.SetParent(cenario.transform);
        bloco2.transform.position = new Vector3(posx,2.0F, posz);
    }

    void instanciarMoeda(int posicaoz, int posicaox) {
        GameObject moeda2 = Instantiate(moeda);
        float posz = ((10*posicaoz)+((estagioAtual-1)*100)) + cenario.transform.position.z;
        float posx = (posicaox-1)*distanciaRaia;
        moeda2.transform.SetParent(cenario.transform);
        moeda2.transform.position = new Vector3(posx,2.0F, posz);
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Moeda")) {
            Destroy(col.gameObject);
        }
        if (col.gameObject.CompareTag("Obstaculo")) {
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
    }

}