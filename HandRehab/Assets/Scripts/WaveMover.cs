using UnityEngine;

public class WaveMover : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;
    public bool isRoot = true;

    void Start()
    {
        Debug.Log($"[WAVE] Iniciando {gameObject.name} | Root: {isRoot}");

        if (GameObject.FindGameObjectsWithTag("Magic").Length > 100)
        {
            Debug.LogWarning("[WAVE] Limite de 100 ondas atingido — destruindo esta instância.");
            Destroy(gameObject);
            return;
        }

        if (isRoot)
        {
            SpawnWavesInAllDirections();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, lifetime);
        }
    }

    void SpawnWavesInAllDirections()
    {
        Debug.Log("[WAVE] Criando ondas em 4 direções");

        Vector3[] directions = new Vector3[] {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        foreach (Vector3 dir in directions)
        {
            GameObject wave = Instantiate(gameObject, transform.position, Quaternion.LookRotation(dir));
            wave.GetComponent<WaveMover>().isRoot = false;
            wave.tag = "Magic";
            Debug.Log($"[WAVE] Criada instância na direção: {dir}");
        }
    }

    void Update()
    {
        if (!isRoot)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            Debug.Log($"[WAVE] Movendo {gameObject.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRoot && other.CompareTag("Enemy"))
        {
            Debug.Log("[WAVE] Inimigo atingido pela onda!");
        }
    }
}
