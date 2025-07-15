// WaveMover.cs
using System.Collections;
using UnityEngine;

public class WaveMover : MonoBehaviour
{
    [Header("Referências")]
    private MyoDataManager myo;

    [Header("Configuração da Onda")]
    public float speed = 5f;
    public float lifetime = 5f;
    public bool isRoot = true;
    public float waveHP = 100f;
    public float damagePerHit = 50f;
    public float knockbackForce = 5f;
    [Tooltip("Prefab usado para gerar sub-ondas")]
    public GameObject wavePrefab;
    [Tooltip("URL de onde buscar o valor de strength")]
    public string dataAddress = "http://localhost:8000/passive";

     private IEnumerator Start()
    {
        // 1) Busca o gerenciador central de strength
        myo = FindObjectOfType<MyoDataManager>();
        if (myo == null)
        {
            Debug.LogError("WaveMover: MyoDataManager não encontrado na cena!");
            yield break;
        }

        // 2) Espera a atualização do valor de strength
        yield return StartCoroutine(myo.GetMyoData(dataAddress));

        // 3) Define o waveHP exatamente igual ao strength recebido
        waveHP = myo.strength;

        // 4) Lógica de root vs. sub-ondas
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

    private void SpawnWavesInAllDirections()
    {
        const int rayCount = 8;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * Mathf.PI * 2f / rayCount;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            var wave = Instantiate(wavePrefab, transform.position, Quaternion.LookRotation(dir));
            var mover = wave.GetComponent<WaveMover>();
            mover?.isRoot = false;
        }
    }


    void Update()
    {
        // Move apenas as ondas filhas
        if (!isRoot)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isRoot) return;
        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        // 1) Consome da força da onda o HP total do inimigo
        waveHP -= enemy.hp;

        if (waveHP > 0f)
        {
            // 2a) Se ainda sobrou força, mata o inimigo e segue adiante
            enemy.hp = 0f;
            Destroy(enemy.gameObject);
            // (não aplica knockback aqui)
        }
        else
        {
            // 2b) Se a onda se esgotou, aplica o knockback ao inimigo
            var rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (enemy.transform.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce * myo.strength, ForceMode.Impulse);
            }
            // e então destrói o inimigo e a própria onda
            Destroy(enemy.gameObject);
            Destroy(gameObject);
        }
    }
}
