using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;

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
    public GameObject diamante;
    public GameObject mola;

    private int estagioAtual = 1;

    private AudioSource somMoeda;
    private AudioSource somExplosao;
    private AudioSource somJogo1;
    private AudioSource somJogo2;
    private bool isGameOver = false;

    public Text txtGameOver;

    public Text txtPontos;
    private int pontos = 0;

    private bool pulando = false;
    private bool caindo = false;
    private bool instanciouDiamante = false;
    private bool tocouDiamante = false;
    private bool puloAlto = false;

    // Start is called before the first frame update
    void Start()
    {
        somMoeda = GetComponents<AudioSource>()[0];
        somExplosao = GetComponents<AudioSource>()[1];
        somJogo1 = GetComponents<AudioSource>()[2];
        somJogo2 = GetComponents<AudioSource>()[3];

        raiaAtual = 1;
        target = jogador.transform.position;
        montarCenario();

        txtPontos.text = "Pontos: " + pontos;
    }

    // Update is called once per frame
    void Update() {

        if (isGameOver) {
            return;
        }

        transform.Rotate(new Vector3(90,0,0) * Time.deltaTime);

        int novaRaia = -1;
        bool pular = false;

        // Teclado
        if (Input.GetKeyDown(KeyCode.RightArrow) && raiaAtual < 2) {
            novaRaia = raiaAtual + 1;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && raiaAtual > 0) {
            novaRaia = raiaAtual - 1;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            pular = true;
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
            if (Input.mousePosition.y > initialPosition.y ) {
                pular = true;
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
                if (Input.GetTouch(0).position.y > initialPosition.y) {
                    pular = true;
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
        
        // if (jogador.transform.position.x != target.x) {
        //     jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5 * Time.deltaTime);
        // }

        if (pular && caindo == false && pulando == false) {
            pulando = true;
        }
        
        if (pulando) {
            //1.0 = chão
            //3.0 = máximo do pulo
            if (jogador.transform.position.y < 3.0 ) {
                target.y = 3.5F;
                jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5*Time.deltaTime);
            } else {
                //chegou na altura de 3.0
                pulando = false;
            }
        } else if (pulando==false && jogador.transform.position.y > 1.5 && puloAlto == false) {
            //caindo (gravidade)
            target.y = 1.0F;
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5*Time.deltaTime);
            caindo = true;
        } else if (jogador.transform.position.x != target.x) {
            //apenas move horizontalmente
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5*Time.deltaTime);
            caindo = false;
        } else {
            caindo = false;
        }

        //move o chao
        velocidadeCenario += (Time.deltaTime * 0.1f);
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
                elemento[j] = Random.Range(0, 5);
                // 0 = nada; 1 = bloco; 2 = moeda
                if ( elemento[0] == 1 && elemento[1] == 1 && elemento[2] == 1 ) {
                    // previne que não tenha 3 blocos na mesma linha
                    elemento[2] = 0;
                }
                if ( tocouDiamante == false && elemento[j] == 1 ) { instanciarBloco(i, j); }
                else if ( elemento[j] == 2 ) { instanciarMoeda(i, j); }
                else if ( instanciouDiamante == false && elemento[j] == 3 ) { 
                    instanciarDiamante(i, j); 
                    instanciouDiamante = true; 
                }
                else if ( elemento[j] == 4 ) { instanciarMola(i, j); }
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
        moeda2.transform.position = new Vector3(posx,1.0F, posz);
    }

    void instanciarDiamante(int posicaoz, int posicaox) {
        GameObject diamante2 = Instantiate(diamante);
        float posz = ((10*posicaoz)+((estagioAtual-1)*100)) + cenario.transform.position.z;
        float posx = (posicaox-1)*distanciaRaia;
        diamante2.transform.SetParent(cenario.transform);
        diamante2.transform.position = new Vector3(posx, 1.0F, posz);
    }

    void instanciarMola(int posicaoz, int posicaox) {
        GameObject mola2 = Instantiate(mola);
        float posz = ((10*posicaoz)+((estagioAtual-1)*100)) + cenario.transform.position.z;
        float posx = (posicaox-1)*distanciaRaia;
        mola2.transform.SetParent(cenario.transform);
        mola2.transform.position = new Vector3(posx, 1.0F, posz);
    }

    async void OnTriggerEnter(Collider col) {
        if (col.gameObject.CompareTag("Moeda")) {
            somMoeda.Play();
            Destroy(col.gameObject);
            pontos++;
            txtPontos.text = "Pontos: " + pontos;
        }
        if (col.gameObject.CompareTag("Obstaculo")) {
            txtGameOver.text = "GAME OVER";
            isGameOver = true;
            somExplosao.Play();
            Invoke("Menu", 5);
        }
        if (col.gameObject.CompareTag("Diamante")) {
            somJogo1.Stop();
            Destroy(col.gameObject);
            tocouDiamante = true;
            somJogo2.Play();
        }
        if (col.gameObject.CompareTag("Mola")) {
            // Pula alto
            target.y = 5.0F;
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5*Time.deltaTime);
            puloAlto = true;

            await Task.Delay(1000);

            if ( puloAlto == true ) {
                // Pula baixo
                puloAlto = false;
                target.y = 1.0F;
                jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5*Time.deltaTime);
            }
        }
    }

    void Menu() {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

}