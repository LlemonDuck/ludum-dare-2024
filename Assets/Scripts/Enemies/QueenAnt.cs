using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueenAnt : BaseEnemy {
    public BehaviourState currentState = BehaviourState.PHASE_1_SPAWNING;

    public float antSpawnTime = 0.3f;
    private float timeSinceLastSpawn = 0.0f;

    public float vulnerabilityTimer = 0.0f;
    public float vulnerabilityDuration = 5.0f;

    private bool startedFight = false;

    public GameObject basicWorkerPrefab;

    public Transform[] workerSpawnPositions;

    public WorkerAnt[] spawnedAnts;

    public bool[] hasSpawned;

    private GameObject[] workerPool = new GameObject[20];

    public enum BehaviourState {
        PHASE_1_SPAWNING,
        PHASE_1_VULNERABLE,

        P2_SPAWNING_SPORES,
        P2_ATTACK_PLAYER,
    }

    void Awake() {
        for (int i = 0; i < workerPool.Length; i++) {
            workerPool[i] = Instantiate(basicWorkerPrefab);
            workerPool[i].GetComponent<WorkerAnt>().OnKilled();
            workerPool[i].GetComponent<SpriteRenderer>().enabled = false;
        }

        hasSpawned = new bool[workerSpawnPositions.Length];
        spawnedAnts = new WorkerAnt[workerSpawnPositions.Length];
        for(int i = 0; i < hasSpawned.Length; i++) {
            hasSpawned[i] = false;
        }
    }

    void switchState(BehaviourState newState) {
        switch (newState) {
            case BehaviourState.PHASE_1_SPAWNING:
                for (int i = 0; i < hasSpawned.Length; i++) {
                    hasSpawned[i] = false;
                }
                timeSinceLastSpawn = 0.0f;
                goto default;
            case BehaviourState.PHASE_1_VULNERABLE:
                vulnerabilityTimer = 0.0f;
                goto default;
            default:
                currentState = newState;
                break;
        }
    }

    public override void applyDamage(float damage) {
        if (currentState == BehaviourState.PHASE_1_SPAWNING) return;
        base.applyDamage(damage);
    }

    public override void applyDamage(float damage, Vector2 direction, float intensity) {
        if (currentState == BehaviourState.PHASE_1_SPAWNING) return;
        base.applyDamage(damage, direction, intensity);
    }

    // Start is called before the first frame update
    void Start() {
    }

    void applyState() {
        Vector2 playerDirection = getPlayerOffset();

        switch (currentState) {
            case BehaviourState.PHASE_1_SPAWNING:
                spawningPhase();
                break;
            case BehaviourState.PHASE_1_VULNERABLE:
                vulnerabilityTimer += Time.deltaTime;
                if (vulnerabilityTimer >= vulnerabilityDuration) switchState(BehaviourState.PHASE_1_SPAWNING);
                break;
        }
    }

    void spawningPhase() {
        if (timeSinceLastSpawn >= antSpawnTime) {
            bool didSpawn = false;

            for(int i = 0; i < hasSpawned.Length; i++) {
                if (hasSpawned[i] == false) {
                    WorkerAnt newSpawn = spawnAnt(workerSpawnPositions[i % workerSpawnPositions.Length]);

                    if (newSpawn != null) {
                        spawnedAnts[i] = newSpawn;
                        hasSpawned[i] = true;
                    }

                    didSpawn = true;
                    break;
                }
            }

            if (!didSpawn) {
                switchState(BehaviourState.PHASE_1_VULNERABLE);
                foreach (WorkerAnt ant in spawnedAnts) {
                    ant.alive = true;
                }
            }
        }
    }

    WorkerAnt spawnAnt(Transform point) {
        var ant = workerPool.First(worker => !worker.GetComponent<CircleCollider2D>().enabled);

        if (ant != null) {
            ant.GetComponent<WorkerAnt>().Revive();
            ant.GetComponent<WorkerAnt>().alive = false;
            ant.transform.position = point.position;
            timeSinceLastSpawn = 0.0f;
            return ant.GetComponent<WorkerAnt>();
        }

        return null;
    }

    // Update is called once per frame
    protected override void Update() {
        if (getPlayerOffset().magnitude < 25) startedFight = true;

        if (!alive || !startedFight) return;
        base.Update();

        BehaviourState stateBeforeUpdate = currentState;
        BehaviourState stateAfterUpdate = currentState;

        do {
            stateBeforeUpdate = currentState;

            applyState();

            stateAfterUpdate = currentState;
        } while (stateBeforeUpdate != stateAfterUpdate);

        timeSinceLastSpawn += Time.deltaTime;
    }
}
