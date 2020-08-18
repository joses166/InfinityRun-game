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

    // Start is called before the first frame update
    void Start()
    {
        raiaAtual = 1;
        target = jogador.transform.position;
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