using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitNode = System.Collections.Generic.LinkedListNode<Unit>;
using EnemyNode = System.Collections.Generic.LinkedListNode<Enemy>;
using BuildingNode = System.Collections.Generic.LinkedListNode<Building>;

// TODO:
// HIGH
// - Unit control
//   - UnitSelector script: Must be called every update()
//   - Select button and deselect button
//   - For each tap, Physics2D.OverlapCircleAll to scoop up all of em
//     -> Or alternatively, each unit always checks the touches
//     -> This would probably be *faster*
//     -> Maybe do physics2d cast every time we move a certain distance in world coords.
//        That distance being 2*radius. Or 1*radius.

// - Building creation UI

// - Building upgrades

// - Showing damage text: do we actually want this? Might be worth implementing and looking at
// - Don't create blood objects in ShowDamage when offscreen.
//   -> Should reduce in general tbh, if more than a certain number per sec then spawn half randomly
//   -> Maybe make probability a function of atks per second... so that it maxes out?
//                  or atks per deltasecond..? cap per frame..? based on delta..? camera distance?


// LOW
//  - Units recheck target to find new closest ones regularly
//  - Timer for next wave
//  - Castle circle: Change to castle oblong, make it look cool 3D perspective and go behind the castle.

//  - Enemies attacking castle / buildings needs some deeper thought, I think
//    Though saying that the current approach can be overidden, so maybe it's ok

//   - When units are idle, return to castle and move about randomly. (new mode: defence mode?)
//     -> or just sorta spread out? -> Or depends on unit? That can be overidden already

//   - Lifebars alpha fade in and out so they're only visible for those in center. Would look cool.

// - Online:
//   - Plan. Don't use any inbuilt Unity networking stuff, because it really, really sucks.

public class GameState
{
    public Controller controller { get; private set; }

    private LinkedList<Building> buildings;
    private LinkedList<Enemy> enemies;
    public LinkedList<Unit> units { get; private set; }

    // These should be the asset names at Resources/Prefabs/...
    // And the name of each enemy once
    private string[] enemyNames = {"enemy1", "enemy2", "enemy3"};
    private string[] unitNames = { "swordsman", "archer" };
    private int[] unitCosts = { 30, 40 };
    private IDictionary<string, GameObject> enemyNameMap;
    private GameObject[] unitObjList;

    public GameObject redblood, greenblood, spark, castleprefab, houseprefab, selectCircle;
    public CurrencyManager currencyManager;
    public WaveManager waveManager;
    public UnitSelector unitSelector;

    public int CurrentWave { get; private set; }
    
    private Castle gameCastle;
    private const int startCoins = 100;

    private void LoadEnemyAssets()
    {
        enemyNameMap = new Dictionary<string, GameObject>();
        foreach (string name in enemyNames)
        {
            GameObject obj = Resources.Load<GameObject>("Prefabs/" + name);
            if (obj == null)
            {
                Debug.LogError("ERROR: Enemy name not found: \"" + name + "\" in GameState constructor");
            }
            enemyNameMap[name] = obj;
        }
    }

    private void LoadUnitAssets()
    {
        unitObjList = new GameObject[unitNames.Length];

        for (int i=0; i<unitNames.Length; i++)
        {
            string name = unitNames[i];

            GameObject obj = Resources.Load<GameObject>("Prefabs/" + name);
            if (obj == null)
            {
                Debug.LogError("ERROR: Enemy name not found: \"" + name + "\" in GameState constructor");
            }

            unitObjList[i] = obj;
        }
    }

    // TODO: We probably shouldn't be loading these once per game, move to a static thing at start of game?
    private void LoadResources()
    {
        LoadEnemyAssets();
        LoadUnitAssets();

        redblood = Resources.Load<GameObject>("Prefabs/redblood");
        greenblood = Resources.Load<GameObject>("Prefabs/greenblood");
        spark = Resources.Load<GameObject>("Prefabs/spark");

        castleprefab = Resources.Load<GameObject>("Prefabs/castle");
        houseprefab = Resources.Load<GameObject>("Prefabs/house");

        selectCircle = Resources.Load<GameObject>("Prefabs/SelectCircle");
    }

    public GameState(Controller myController)
    {
        LoadResources();

        controller = myController;

        waveManager = new WaveManager();
        currencyManager = new CurrencyManager(startCoins);
        unitSelector = new UnitSelector(this);

        units = new LinkedList<Unit>();
        enemies = new LinkedList<Enemy>();
        buildings = new LinkedList<Building>();

        GameObject obj = Spawn(castleprefab, new Vector2(0, 0), 2);
        gameCastle = obj.GetComponent<Castle>();

        Spawn(houseprefab, new Vector2(10, 0), 2);
    }

    public Castle GetCastle()
    {
        return gameCastle;
    }

    // mode: 0 = unit, 1 = enemy, 2 = building
    private GameObject Spawn(GameObject prefab, Vector2 pos, int mode)
    {
        GameObject obj = Object.Instantiate(prefab, pos, Quaternion.identity);
        Attackable en = obj.GetComponent<Attackable>();
        en.gameState = this;

        switch (mode)
        {
            case 0:
                units.AddLast(obj.GetComponent<Unit>());
                break;
            case 1:
                enemies.AddLast(obj.GetComponent<Enemy>());
                break;
            case 2:
                buildings.AddLast(obj.GetComponent<Building>());
                break;                
        }

        return obj;
    }

    public void WaveSpawnEnemy(string name)
    {
        if (!enemyNameMap.ContainsKey(name))
        {
            Debug.LogError("Wave error: Enemy \"" + name + "\" not found.");
            return;
        }
        Spawn(enemyNameMap[name], Util.RandomPointOnCircle() * 15, 1);
    }

    // Deal area damage
    public void Explode(Vector2 center, float radius, int damage, bool attackAlly)
    {
        foreach (var collider in Physics2D.OverlapCircleAll(center, radius))
        {
            if (attackAlly)
            {
                if (collider.gameObject.tag == "Ally")
                {
                    collider.gameObject.GetComponent<Attackable>().TakeDamage(damage, center);
                }
            }
            else
            {
                if (collider.gameObject.tag == "Enemy")
                {
                    collider.gameObject.GetComponent<Attackable>().TakeDamage(damage, center);
                }
            }
        }
    }

    public void NextWave()
    {
        Wave wave = waveManager.GetNextWave();
        CurrentWave = waveManager.GetCurrentWave();
        controller.SpawnWave(wave, this);
    }

    private void RemoveNull()
    {
        for (UnitNode n = units.First; n != null;)
        {
            UnitNode next = n.Next;

            if (n.Value == null || n.Value.gameObject == null)
            {
                units.Remove(n);
            }

            n = next;
        }

        for (EnemyNode n = enemies.First; n != null;)
        {
            EnemyNode next = n.Next;

            if (n.Value == null || n.Value.gameObject == null)
            {
                enemies.Remove(n);
            }

            n = next;
        }

        for (BuildingNode n = buildings.First; n != null;)
        {
            BuildingNode next = n.Next;

            if (n.Value == null || n.Value.gameObject == null)
            {
                buildings.Remove(n);
            }

            n = next;
        }
    }

    // TODO: Quadtree it. Reconstruct quadtree every t seconds.
    //  -> Or, google for inbuilt Unity function (surely it has a quadtree?)
    public Unit GetClosestUnit(Vector2 pos)
    {
        float closestDist = -1;
        Unit closestUnit = null;

        foreach (Unit u in units)
        {
            if (u == null) continue;

            float f = Vector2.Distance(u.GetPos(), pos);
            if (f < closestDist || closestDist == -1)
            {
                closestDist = f;
                closestUnit = u;
            }
        }

        return closestUnit;
    }

    // Random = 0..1
    public Enemy GetClosestEnemy(Vector2 pos, float random = 0.0f)
    {
        float L = 1.0f - random, H = 1.0f + random;

        float closestDist = -1;
        Enemy closestEnemy = null;

        foreach (Enemy u in enemies)
        {
            if (u == null) continue;

            float f = Vector2.Distance(u.GetPos(), pos) * Random.Range(L, H);
            if (f < closestDist || closestDist == -1)
            {
                closestDist = f;
                closestEnemy = u;
            }
        }

        return closestEnemy;
    }

    // Random = 0..1
    public Building GetClosestBuilding(Vector2 pos, float random = 0.0f)
    {
        float L = 1.0f - random, H = 1.0f + random;

        float closestDist = -1;
        Building closestEnemy = null;

        foreach (Building u in buildings)
        {
            if (u == null) continue;

            float f = Vector2.Distance(u.GetPos(), pos) * Random.Range(L, H);
            if (f < closestDist || closestDist == -1)
            {
                closestDist = f;
                closestEnemy = u;
            }
        }

        return closestEnemy;
    }

    // Unit spawning stuff

    public void DragBegin()
    {
        foreach (Building b in buildings)
        {
            if (b!=null)
                b.ShowCircle(true);
        }
    }

    public void DragEnd()
    {
        foreach (Building b in buildings)
        {
            b.ShowCircle(false);
        }
    }

    public void TryPlacing(int unitID, Vector2 worldPos)
    {
        if (unitID < 0 || unitID >= unitNames.Length)
        {
            Debug.LogError("Attempting to spawn unit with invalid ID: " + unitID);
            return;
        }

        bool isValid = false;
        foreach (Building b in buildings)
        {
            if (b != null && b.CanPlace(worldPos))
            {
                isValid = true;
            }
        }

        if (isValid)
        {
            int cost = unitCosts[unitID];

            if (currencyManager.GetCoins() >= cost)
            {
                currencyManager.Spend(cost);
                Spawn(unitObjList[unitID], worldPos, 0);
            }
        }


    }

    private float timeSinceLastGC = 0;
    public void Check()
    {
        if (timeSinceLastGC > 0.5f)
        {
            RemoveNull();
            timeSinceLastGC = 0;
        }
        else timeSinceLastGC += Time.deltaTime;

        unitSelector.CheckForClicks();

        // TODO: REMOVE THIS TEST
        if (Input.GetKeyDown(KeyCode.S)) // Standby
        {
            foreach (Unit u in units)
            {
                if (u != null) u.Standby();
            }
        }
        if (Input.GetKeyDown(KeyCode.A)) // Attack
        {
            foreach (Unit u in units)
            {
                if (u != null) u.Attack();
            }
        }
        if (Input.GetKeyDown(KeyCode.M)) // Move
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (Unit u in units)
            {
                if (u != null) u.MoveTo(worldPos, units.Count);
            }
        }
    }
}
