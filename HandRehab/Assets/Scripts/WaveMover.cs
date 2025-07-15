using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;

public class WaveMover : Strength
{
    [Header("Configuração da Onda")]
    public float speed = 5f;
    public float lifetime = 5f;
    public bool isRoot = true;
    public float waveHP = 100f;
    public float damagePerHit = 50f;
    public float knockbackForce = 5f;

    [Header("Alcance")]
    [Tooltip("Distância máxima que cada onda deve percorrer antes de ser destruída")]
    public float maxDistance = 10f;

    private Vector3 originPosition;

    void Start()
    {
        // Marca o ponto de origem para controle de distância
        originPosition = transform.position;

        Debug.Log($"[WAVE] Iniciando {gameObject.name} | Root: {isRoot}");

        // Busca o valor de strength antes de criar as ondas
        StartCoroutine(GetMyoData("http://localhost:8000/strength"));

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
            // Clona o próprio GameObject para manter todas as configurações originais
            GameObject wave = Instantiate(gameObject, transform.position, Quaternion.LookRotation(dir));
            WaveMover mover = wave.GetComponent<WaveMover>();
            if (mover != null)
            {
                mover.isRoot = false;
                mover.waveHP = waveHP;
                wave.tag = "Magic";
                Debug.Log($"[WAVE] Criada instância na direção: {dir}");
            }
        }
    }

    void Update()
    {
        if (!isRoot)
        {
            // Move a onda para frente (espaço local funciona porque usamos Instantiate(gameObject))
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            Debug.Log($"[WAVE] Movendo {gameObject.name}");

            // Destrói a onda ao ultrapassar a distância máxima
            if (Vector3.Distance(originPosition, transform.position) >= maxDistance)
            {
                Debug.Log("[WAVE] Distância máxima alcançada — destruindo esta instância.");
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRoot && other.CompareTag("Enemy"))
        {
            Debug.Log("[WAVE] Inimigo atingido pela onda!");

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                float actualDamage = Mathf.Min(damagePerHit, enemy.hp);
                enemy.hp -= actualDamage;
                waveHP -= actualDamage;

                Debug.Log($"[WAVE] Dano aplicado: {actualDamage}. HP restante da onda: {waveHP}. HP do inimigo: {enemy.hp}");

                // Só aplica knockback se a onda se esgotou
                if (waveHP <= 0f)
                {
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 knockDir = (other.transform.position - transform.position).normalized;
                        rb.AddForce(knockDir * knockbackForce * strength, ForceMode.Impulse);
                        Debug.Log("[WAVE] Rebote aplicado no inimigo.");
                    }
                }

                if (enemy.hp <= 0f)
                {
                    Debug.Log("[WAVE] Inimigo destruído.");
                    Destroy(other.gameObject);
                }

                if (waveHP <= 0f)
                {
                    Debug.Log("[WAVE] Onda destruída após atingir inimigos.");
                    Destroy(gameObject);
                }
            }
        }
    }
}
