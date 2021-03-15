using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour{
    public int numero_ovejas;
    public GameObject oveja_prefab;

    private float radioSpawner = 1.5f;
    //NOTA: TODASLASOVEJAS SOLO SON LAS QUE SACA ESTE SPAWNER, NO JUNTA LAS DEMAS, ARREGLAR
    void Start(){
        if (numero_ovejas > 0){
            List<Oveja> listaO = new List<Oveja>();
            for (int i=0; i<numero_ovejas; i++){
                Vector3 posRandom = transform.position + Random.insideUnitSphere * radioSpawner;
                Vector3 dirRandom = new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f,1f));
                posRandom.y = 0.15f;
                GameObject ov = Instantiate(oveja_prefab);
                listaO.Add(ov.GetComponent<Oveja>());

                //Le damos posicion y direccion a la oveja
                ov.GetComponent<Oveja>().Inicializar(posRandom, dirRandom);
            }
            //AQUI DEBERIA BUSCAR TODAS LAS OVEJAS DE LA ESCENA
            foreach (var o in listaO){
                o.todasOvejas = listaO;
            }   
        }
    }
}
