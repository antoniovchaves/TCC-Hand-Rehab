// ChainLightning.cs
using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    [Header("Referências")]
    private MyoDataManager myo;
    private List<GameObject> targets;

    [Header("Configuração do Efeito")]
    public GameObject lightningBoltPrefab;
    public float range = 10f;
    public float baseDamage = 10f;
    [Tooltip("URL de onde buscar o valor de strength")]
    public string dataAddress = "http://localhost:8000/strength";
    [Tooltip("Número máximo de inimigos atingidos em cadeia")]
    public int maxChains = 3;

    void Start()
    {
        // Busca o gerenciador central de strength
        myo = FindObjectOfType<MyoDataManager>();
        if (myo == null)
        {
            Debug.LogError("ChainLightning: MyoDataManager não encontrado na cena!");
            return;
        }
        // Puxa o valor inicial
        StartCoroutine(myo.GetMyoData(dataAddress));

        // Carrega todos os inimigos
        targets = GameObject.FindGameObjectsWithTag("Enemy").ToList();
    }

    /// <summary>
    /// Chama este método passando a mão detectada para disparar o Chain Lightning.
    /// </summary>
    public void LightItUp(Hand hand)
    {
        if (myo == null) return;

        float strength = myo.strength;
        Vector3 startPos = hand.PalmPosition.ToVector3();
        Vector3 palmNormal = hand.PalmNormal.ToVector3();

        // Cria uma cópia da lista para não remover da original
        List<GameObject> available = new List<GameObject>(targets);

        HitChain(startPos, palmNormal, available, strength, maxChains);
    }

    private void HitChain(Vector3 fromPos, Vector3 direction, List<GameObject> available, float strength, int remaining)
    {
        if (remaining <= 0 || available.Count == 0) return;

        // Encontra o inimigo mais próximo dentro de range e dentro de 30° de ângulo
        GameObject closest = FindClosestEnemy(fromPos, direction, available);
        if (closest == null) return;

        // Instancia o raio
        GameObject bolt = Instantiate(lightningBoltPrefab, fromPos, Quaternion.identity);
        var lr = bolt.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, fromPos);
            lr.SetPosition(1, closest.transform.position);
        }

        // Aplica dano
        var enemy = closest.GetComponent<Enemy>();
        if (enemy != null)
        {
            float dmg = baseDamage * strength;
            enemy.hp -= dmg;
        }

        // Prepara próxima iteração
        available.Remove(closest);
        HitChain(closest.transform.position, direction, available, strength, remaining - 1);
    }

    private GameObject FindClosestEnemy(Vector3 fromPos, Vector3 direction, List<GameObject> candidates)
    {
        GameObject best = null;
        float bestDist = float.MaxValue;

        foreach (var go in candidates)
        {
            float dist = Vector3.Distance(fromPos, go.transform.position);
            if (dist > range) continue;

            // filtra pelo ângulo com a palma da mão
            Vector3 toEnemy = (go.transform.position - fromPos).normalized;
            float angle = Vector3.Angle(direction, toEnemy);
            if (angle > 30f) continue;

            if (dist < bestDist)
            {
                bestDist = dist;
                best = go;
            }
        }

        return best;
    }
}
