using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Settings : ScriptableObject{
    public float radioPercepcion = 1f;

    [Header("Distancias")]
    public float dstSeparacionObstaculo = 1.5f;
    public float dstSeparacion = 0.2f;
    public float distanciaPerro = 1f;

    //Pesos de las reglas
    [Header ("Pesos reglas")]
    public float pesoCohesion = 1f;
    public float pesoSeparacion = 2f;
    public float pesoAlineacion = 1f;
    public float pesoHuida = 5f;
    public float pesoEvitado = 5f;

    public LayerMask mascaraEvitado;
}
