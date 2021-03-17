using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Oveja : MonoBehaviour{
    public Settings settings;
    public GameObject perro;
    public List<Oveja> todasOvejas;
    public List<Oveja> manada;
    //Tener en cuenta que la y siempre sera 0.15
    public Vector3 posicion;
    public Vector3 direccion;
    
    private float velocidad = 0.5f;
    //Reglas para dibujarlas en gizmos cohesion, alineacion y separacion
    public Vector3 r1,r2,r3,r4,r5,r6;
    private Vector3 cero = new Vector3(0,0,0);

    public void Inicializar(Vector3 pos, Vector3 dir){
        posicion = pos;
        this.transform.position = pos;
        //print("Rotacion: " + this.transform.rotation);

        direccion = dir;
        this.transform.forward = dir;
        //print("Rotacion2: " + this.transform.rotation);
    }

    void Update() {
        PercibirManada();

        //Vectores creados por las reglas. NUNCA SE MOVERAN EN Y
        Vector3 cohesion = Cohesion(manada);
        Vector3 alineacion = Alineacion(manada);
        Vector3 separacion = Separacion(manada);
        Vector3 huida = HuirPerro(perro);
        //Correccion de la y que siempre debe ser 0 en las reglas
        cohesion.y=0;alineacion.y=0;separacion.y=0; huida.y = 0;
        r1=cohesion;r2=alineacion;r3=separacion;r5 = huida;

        Vector3 col = cero;//Colision()? EvitadoObstaculo():cero;
        if(Colision()){
            col = ObstacleRays() * settings.pesoEvitado;
            //print("Voy a chocar contra el escenario");
        }
        r4=col;            
        direccion = transform.forward;
        //Tiene que ser la suma de las diferentes reglas de la oveja
        Vector3 aceleracion = (cohesion+alineacion+separacion+col+huida);
        float speed = aceleracion.magnitude;

        print("Distancia perro: " + Distancia(perro.transform.position, this.transform.position));

        //Si no hay manada ni nada, seguimos nuestra direccion normal
        direccion = speed==0? direccion: aceleracion/speed;

        //Nueva posicion
        posicion = direccion * velocidad;
        posicion.y=0;

        //Actualizacion de los elementos fisicos del objeto
        transform.forward = direccion;
        transform.position += posicion * Time.deltaTime;        
    }

    //Comprobamos que ovejas forman parte de nuestra manada, esto se comprueba una vez por frame. PUEDE SER OPTIMIZADO, ES UN POCO KK
    void PercibirManada(){
        List<Oveja> listaAux = new List<Oveja>();
        foreach (var o in todasOvejas){
            //NO NOS INCLUIMOS EN LA MANADA
            if(Distancia(o.transform.position, this.transform.position) <= settings.radioPercepcion && o!=this){
                listaAux.Add(o);
            }
        }
        manada = listaAux;
    }

    // Vector3 EvitadoObstaculo(){
    //     Vector3 nuevaDir = cero;
    //     RaycastHit hit;
    //     for (int i = 0; i < 100; i++){
    //         float r = settings.radioPercepcion * UnityEngine.Random.Range(0f,1f);
    //         float theta = UnityEngine.Random.Range(0,1) * 2 * Mathf.PI;
    //         nuevaDir = new Vector3( (float) (r*Math.Cos(theta)) , 0, (float) (r*Math.Sin(theta)) );
    //         if (Physics.SphereCast (transform.position, 0.2f, nuevaDir, out hit, settings.radioPercepcion, settings.mascaraEvitado))
    //             return nuevaDir*settings.pesoEvitado;
    //     }
    //     return nuevaDir;
    // }

    ///<summary>Dado un numero de puntos N, devuelve un array de Vector3 con n puntos uniformemente distribuidos  en una esfera a partir del golden ratio</summary>
    //NOTA: ESTO NO ES EXACTAMENTE APLICABLE A UNA CIRCUNFERENCIA, ARREGLAR
    private Vector3[] EsferaFibonacci(int numeroPuntos){
        Vector3[] puntos = new Vector3[numeroPuntos];
        var goldenN = (1 + Math.Pow(5,0.5)) / 2;
        for (int i = 0; i < numeroPuntos; i++){
            var theta = 2 * Math.PI * (i/goldenN);
            var phi = Math.Acos(1-2*(i+0.5f)/numeroPuntos);
            puntos[i] = new Vector3((float) Math.Cos(theta)* (float) Math.Sin(phi), 0f, (float) Math.Cos(phi));
        }
        return puntos;
    }

    public Ray rayo;//Rayo para dibujar en gizmos
    public Ray rayo2;
    ///<summary>Decidimos a donde nos movemos en funcion de los rayos casteados</summary>
    Vector3 ObstacleRays () {
        //Vector con las direcciones de los rayos en funcion del numero aureo
        Vector3[] rayDirections = EsferaFibonacci(30);
        //Si un rayo NO golpea, nos movemos en esa direccion
        for (int i = 0; i < rayDirections.Length; i++) {
            //Vector3 dir = rayDirections[i]-transform.position;
            //Pasa la direccion de espacio local a espacio global
            Vector3 dir = this.transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray (transform.position, dir);

            //Ray ray2 = new Ray (transform.position, dir2);
            //Devolvemos la direccion del primer rayo que no golpea un obstaculo
            if (!Physics.SphereCast (ray, 0.2f, settings.dstSeparacionObstaculo, settings.mascaraEvitado)) {
                rayo = ray;
                return dir;
            }
        }
        //Si todos los rayos han golpeado, seguimos adelante (Si todos golpean, no tenemos escapatoria asi que da igual que hacer)
        return direccion;
    }

    //Vector entre nosotros y la direccion general de la manada
    Vector3 Alineacion(List<Oveja> manada){
        Vector3 rA = cero;
        if(manada.Count == 0)
            return rA;
        foreach (var o in manada){
            rA += o.direccion;
        }
        rA = rA/manada.Count;
        rA.y=0;
        return (rA)*settings.pesoAlineacion;
    }

    Vector3 HuirPerro(GameObject perro) {
        Vector3 huida = cero;
        return Distancia(perro.transform.position, this.transform.position) >= settings.distanciaPerro? cero: (this.transform.position - perro.transform.position)*settings.pesoHuida;
    }

    //El vector entre nosotros y el centro de la manada
    Vector3 Cohesion(List<Oveja> manada){
        Vector3 rC = cero;
        if(manada.Count == 0)
            return rC;
        foreach (var o in manada){
            rC += o.transform.position;
        }
        rC = rC/manada.Count;
        rC.y=0;
        return (rC-this.transform.position)*settings.pesoCohesion;
    }

    //El vector entre nosotros y las ovejas demasiado cercanas, devuelto en negativo para alejarnos
    Vector3 Separacion(List<Oveja> manada){
        Vector3 rS = cero;
        if(manada.Count == 0)
            return rS;
        foreach (var o in manada){
            if(Distancia(o.transform.position, this.transform.position) < settings.dstSeparacion)
                rS -= o.transform.position - this.transform.position;
        }
        rS.y=0;
        return rS*settings.pesoSeparacion;
    }

    //Comprobamos con un rayo si vamos a colisionar con un obstaculo
    public bool Colision(){
        RaycastHit hit;
        //origen, radio esfera que casteamos, direccion, informacion hit, maxima distancia casteo, mascara de obstaculos
        if (Physics.SphereCast (transform.position, 0.2f, direccion, out hit, settings.radioPercepcion, settings.mascaraEvitado))
            return true;
        else
            return false;
    }

    //Devuelve la distancia entre dos posiciones
    float Distancia(Vector3 destino, Vector3 origen){
        Vector3 vector = destino - origen;
        return Mathf.Sqrt(vector.x*vector.x + vector.y*vector.y +vector.z*vector.z);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;

        var colorAux = Color.blue;colorAux.a = 0.3f;
        var colorAux2 = Color.yellow;colorAux2.a = 0.1f;

        //Radio percepcion
        Gizmos.color = colorAux;
        Gizmos.DrawSphere(transform.position, settings.radioPercepcion);

        //Radios distancia separacion entre boids
        Gizmos.color = colorAux2;
        Gizmos.DrawSphere(transform.position, settings.dstSeparacion);

        //Direccion del boid
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, (direccion + transform.position)*1f);

        //R1: Cohesion, el centro de la manada
        Gizmos.color = Color.red;
        if(r1!=cero)
            Gizmos.DrawLine(transform.position, r1+transform.position);

        //R2: Separacion, vector para alejarnos resultante si hay un boid muy cerca de nosotros
        Gizmos.color = Color.white;
        if(r3!=cero)
            Gizmos.DrawLine(transform.position, r3+transform.position);

        //R3: Alineacion, direccion general de la manada
        Gizmos.color = Color.green;
        if(r2!=cero)
            Gizmos.DrawLine(transform.position, (r2+transform.position)*1f);

        //Evitado
        Gizmos.color = Color.blue;
        if(r4!=cero)
            Gizmos.DrawLine(transform.position, (r4+transform.position)*1f);

        Gizmos.color = Color.magenta;
        if (r5 != cero)
            Gizmos.DrawLine(transform.position, (r5 + transform.position) * 1f);
    }
}
