﻿using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public Terrain terrain;

    float time;
    int stageNumber;
    bool requireOk;
    bool stageIsTimeBased;
    LeapProvider provider;
    // Start is called before the first frame update
    void Start()
    {
        requireOk = true;
        stageIsTimeBased = false;
        stageNumber = 1;
        provider = FindObjectOfType<LeapProvider>();
        InvokeRepeating("CheckEndStage", 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (requireOk) {
            Hand rightHand = null;
            Hand leftHand = null;
            Frame frame = provider.CurrentFrame;

            if (frame.Hands.Capacity > 0) {
                foreach (Hand h in frame.Hands) {
                    //Debug.Log(h);
                    if (h.IsLeft)
                        leftHand = h;
                    if (h.IsRight)
                        rightHand = h;
                }
            }

            if (rightHand != null) {
                int extendedFingers = 0;
                for (int i = 0; i < rightHand.Fingers.Count; i++) {
                    Finger f = rightHand.Fingers[i];
                    if (f.IsExtended)
                        extendedFingers++;
                }
                if (extendedFingers == 1 && rightHand.Fingers[0].IsExtended) {
                    requireOk = false;
                    StartStage(stageNumber);
                }
            }
        }
    }

    void StartStage(int nextStage) {
        if(nextStage == 1) {
            time = 0;
        }

        StreamReader reader = File.OpenText($"Assets/Stages/stage{nextStage}.txt");
        string line;
        while ((line = reader.ReadLine()) != null) {
            string[] items = line.Split(' ');
            if (items[0] == "TIME") {
                float stageTime = float.Parse(items[1]);
                if (stageTime == -1) {
                    stageIsTimeBased = false;
                }
                else {
                    Invoke("endTimeBasedStage", stageTime);
                }
            }
            else if (items[0] == "MAGIC") {
                for (int i = 1; i < items.Length; i++) {
                    ExerciseDetector.availableMagics.Add((ExerciseType) System.Enum.Parse(typeof(ExerciseType), items[i]));
                }
            }
            else {
                Element element = (Element)System.Enum.Parse(typeof(Element), items[0]);
                int numberOfEnemies = int.Parse(items[1]);
                float spawnTime = float.Parse(items[2]);
                for (int i = 0; i < numberOfEnemies; i++) {
                    StartCoroutine(SpawnEnemy(element, spawnTime));
                }
            }
        }

        stageNumber = nextStage + 1;
    }

    IEnumerator SpawnEnemy(Element element, float spawnTime) {
        yield return new WaitForSeconds(spawnTime);

        GameObject copy = GameObject.Instantiate(enemy);
        var enemyInstance = copy.GetComponent<Enemy>();
        enemyInstance.type = new CharType(element);
        copy.GetComponent<Renderer>().material.color = enemyInstance.type.color;
        copy.transform.position = player.transform.position + Random.onUnitSphere * 20;
        Vector3 enemyPosition = copy.transform.position;
        enemyPosition.y = terrain.SampleHeight(enemyPosition) + terrain.transform.position.y + 1;
        copy.transform.position = enemyPosition;
    }
    
    void CheckEndStage() {
        if (!stageIsTimeBased) {
            requireOk = GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
        }
    }

    void endTimeBasedStage() {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies) {
            DestroyImmediate(enemy);
        }

        requireOk = true;
    }
}
