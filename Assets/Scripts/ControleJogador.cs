using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleJogador : MonoBehaviour
{
    public Rigidbody jogador;
    public GameObject cenario;
    public float velocidadeJogador;
    public float velocidadeCenario;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float direcao = Input.GetAxis("Horizontal");
        // X -> Lateral, Y -> Altura, Z -> Profundidade
        Vector3 move = new Vector3(direcao * velocidadeJogador, 0, 0);
        jogador.AddForce(move);
        // move o cenário
        cenario.transform.Translate( 0, 0, velocidadeCenario * Time.deltaTime * (-1) );
    }
}
