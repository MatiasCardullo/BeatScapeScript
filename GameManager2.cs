
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Quitar miembros privados no utilizados", Justification = "<pendiente>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Agregar modificador de solo lectura", Justification = "<pendiente>")]

public class GameManager2 : MonoBehaviour
{
    public static byte minEnemies, maxEnemies, maxlevel;
    public GameObject itemContainerPrefab, cratePrefab, barrelPrefab;
    public GameObject stairUp, stairDown, blockStair;
    public GameObject[] enemyPrefabs, coinsPrefabs, cellPrefabs, levelObject;
    public static Item[] itemsPrefabs;

    private short minX, maxX, minY, maxY;
    private float spaceX, spaceY;
    private byte level = 0, chargedLevel;
    private ushort[] numberOfRooms;
    private bool[] instantiatedRooms;
    private uint totalEnemies, totalCrates, totalBarrel, totalNormalRooms, totalEspecialRooms;

    private List<ObjCell>[] Grid;
    private List<ObjCell> AuxToAdd = new List<ObjCell>(), ToAdd = new List<ObjCell>();
    private List<CellChild>[] Enemies, Crates, Items;

    void Awake()
    {
        maxlevel = (byte)Random.Range(60, 90);
        //maxlevel = 100;
        levelObject = new GameObject[maxlevel + 1];
        numberOfRooms = new ushort[maxlevel + 1];
        instantiatedRooms = new bool[maxlevel + 1];
        Grid = new List<ObjCell>[maxlevel + 1];
        Enemies = new List<CellChild>[maxlevel];
        Crates = new List<CellChild>[maxlevel];
        Items = new List<CellChild>[maxlevel];
        coinsPrefabs = Resources.LoadAll<GameObject>("Coins");
        cellPrefabs = Resources.LoadAll<GameObject>("Cells");
        itemsPrefabs = Resources.LoadAll<Item>("Items");
        spaceX = 8; spaceY = 8; maxEnemies = 5; minEnemies = 3;
        CreateScene();
        Debug.Log("Enemies: " + totalEnemies + "; Normal Rooms: " + totalNormalRooms + "; Especial Rooms: " + totalEspecialRooms + "; Crates: " + totalCrates + "; Barrels: " + totalBarrel + ";");
        Debug.Log("Coins aproximation= Min:" + (5 * totalEnemies + 10 * totalCrates) + " Max: " + (10 * totalEnemies + 20 * totalCrates) + ";");
    }

    void Start()
    {
        stairDown.SetActive(false);
        instantiatedRooms[0] = true; chargedLevel = 0;
        Grid[0].ForEach(i => i.InstantiateRoom());
    }

    void Update()
    {
        if (chargedLevel != User.level)
        {
            DesInstantiateLevel(chargedLevel);
            InstantiateLevel(User.level);
            chargedLevel = User.level;
        }
    }


    /// <summary>
    /// Create all the levels
    /// </summary>
    private void CreateScene()
    {
        levelObject[0] = GameObject.Find("Level 0");
        levelObject[0].transform.parent = transform;
        Grid[0] = new List<ObjCell>
        {
            CreateObjCell('_', '_', 's', '_', 'R', Vector2Int.zero, levelObject[0].transform),
            CreateObjCell('_', 'n', 's', '_', 'R', Vector2Int.down, levelObject[0].transform),
            CreateObjCell('e', 'n', 's', 'w', 'R', Vector2Int.down * 2, levelObject[0].transform),
            CreateObjCell('_', 'n', '_', '_', 'R', Vector2Int.down * 3, levelObject[0].transform),
            CreateObjCell('_', '_', '_', 'w', 'R', new Vector2Int(1, -2), levelObject[0].transform),
            CreateObjCell('e', '_', '_', '_', 'R', new Vector2Int(-1, -2), levelObject[0].transform)
        };
        totalNormalRooms = 5;

        for (byte i = 1; i < maxlevel; i++)
        {
            minX = 0; maxX = 0; minY = 0; maxY = 0;
            level = i; CreateLevel();
        }

        levelObject[maxlevel] = new GameObject("Level " + maxlevel);
        GameObject enemiesObject = new GameObject("Enemies lv" + maxlevel);
        GameObject cratesObject = new GameObject("Crates lv" + maxlevel);
        GameObject itemsObject = new GameObject("Items lv" + maxlevel);
        levelObject[maxlevel].transform.parent = transform;
        enemiesObject.transform.parent = levelObject[maxlevel].transform;
        cratesObject.transform.parent = levelObject[maxlevel].transform;
        itemsObject.transform.parent = levelObject[maxlevel].transform;
        Grid[maxlevel] = new List<ObjCell>
        {
            CreateObjCell('e', 'n', 's', 'w', 'R', Vector2Int.zero, levelObject[maxlevel].transform),
            CreateObjCell('_', 'n', '_', '_', 'R', Vector2Int.down, levelObject[maxlevel].transform),
            CreateObjCell('_', '_', 's', '_', 'R', Vector2Int.up, levelObject[maxlevel].transform),
            CreateObjCell('e', '_', '_', '_', 'R', Vector2Int.left, levelObject[maxlevel].transform),
            CreateObjCell('_', '_', '_', 'w', 'R', Vector2Int.right, levelObject[maxlevel].transform),
        };
        Enemies[maxlevel - 1] = new List<CellChild>();
        Crates[maxlevel - 1] = new List<CellChild>();
        Items[maxlevel - 1] = new List<CellChild>();
        totalNormalRooms += 4;
        levelObject[maxlevel].SetActive(false);
    }

    /// <summary>
    /// Create the level object and all the childs
    /// </summary>
    private void CreateLevel()
    {
        levelObject[level] = new GameObject("Level " + level);
        GameObject enemiesObject = new GameObject("Enemies lv" + level);
        GameObject cratesObject = new GameObject("Crates lv" + level);
        GameObject itemsObject = new GameObject("Items lv" + level);
        levelObject[level].transform.parent = transform;
        enemiesObject.transform.parent = levelObject[level].transform;
        cratesObject.transform.parent = levelObject[level].transform;
        itemsObject.transform.parent = levelObject[level].transform;
        Grid[level] = new List<ObjCell>();
        Enemies[level - 1] = new List<CellChild>();
        Crates[level - 1] = new List<CellChild>();
        Items[level - 1] = new List<CellChild>();
        ToAdd.Add(CreateObjCell('e', 'n', 's', 'w', 'R', Vector2Int.zero, levelObject[level].transform));
        while (ToAdd.Count != 0)
        {
            AdyacentRooms();
        }
        numberOfRooms[level - 1] = (ushort)Grid[level].Count();
        ExtraRooms();
        ushort count = 0;
        foreach (ObjCell item in Grid[level])
        {
            if (item.r && !item.br)
            {
                if (count < numberOfRooms[level - 1])
                {
                    CellChildSpawner(maxEnemies, minEnemies, item.xy, level, enemiesObject.transform, cratesObject.transform);
                    totalNormalRooms++;
                }
                else
                {
                    CellChildSpawner(0, 0, item.xy, level, enemiesObject.transform, cratesObject.transform);
                }
            }
            count++;
        }
        totalNormalRooms--;
        levelObject[level].SetActive(false);
    }

    /// <summary>
    /// Instance all the rooms, enemies, crates, barrels and coins of the level, also activate/deactivate the stair in first/last level
    /// </summary>
    /// <param name="levelPosition">N°level</param>
    private void InstantiateLevel(int levelPosition)
    {
        bool top = User.level == maxlevel, bottom = User.level == 0;
        if (!top && !bottom)
            blockStair.transform.localPosition = Vector2.zero;
        if (bottom)
        {
            stairDown.SetActive(false);
            blockStair.transform.localPosition = new Vector2(0, 0.055f);
        }
        else
            stairDown.SetActive(true);
        if (top)
        {
            stairUp.SetActive(false);
            blockStair.transform.localPosition = new Vector2(0, -0.045f);
        }
        else
            stairUp.SetActive(true);

        levelObject[levelPosition].SetActive(true);

        Grid[levelPosition].ForEach(i => i.InstantiateRoom());
        if (levelPosition != 0)
        {
            if (Enemies[levelPosition - 1].Any())
                Enemies[levelPosition - 1].ForEach(i => i.InstantiateCellChild());
            if (Crates[levelPosition - 1].Any())
                Crates[levelPosition - 1].ForEach(i => i.InstantiateCellChild());
            /*if (Items[levelPosition - 1].Any())
            {
                foreach (CellChild i in Items[levelPosition - 1])
                {
                    GameObject aux = (GameObject)i.InstantiateCellChild();
                    if (i.item != null)
                        aux.GetComponent<ItemContainer>().item = i.item;
                }
            }*/
        }
        instantiatedRooms[levelPosition] = true;
    }

    /// <summary>
    /// Destroy all the rooms and enemies of the level
    /// </summary>
    /// <param name="levelPosition">N°level</param>
    private void DesInstantiateLevel(int levelPosition)
    {
        if (instantiatedRooms[levelPosition])
        {
            if (levelPosition != 0 && Enemies[levelPosition - 1].Any())
            {
                Enemies[levelPosition - 1].Clear();
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    Enemies[levelPosition - 1].Add(new CellChild().DesInstantiateCellChild(item, enemyPrefabs[0]));
                }
            }
            if (levelPosition != 0 && Crates[levelPosition - 1].Any())
            {
                Crates[levelPosition - 1].Clear();
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("Crate"))
                {
                    Crates[levelPosition - 1].Add(new CellChild().DesInstantiateCellChild(item, cratePrefab));
                }
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("Barrel"))
                {
                    Crates[levelPosition - 1].Add(new CellChild().DesInstantiateCellChild(item, barrelPrefab));
                }
            }
            /*if (levelPosition != 0 && Items[levelPosition - 1].Any())
            {
                Items[levelPosition - 1].Clear();
                Item aux;
                byte index;
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("Coin"))
                {
                    index = byte.Parse(item.name.Substring(4, 1));
                    Items[levelPosition - 1].Add(new CellChild().DesInstantiateCellChild(item, coinsPrefabs[index]));
                }
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("Item"))
                {
                    aux = item.GetComponent<ItemContainer>().item;
                    Items[levelPosition - 1].Add(new CellChild().DesInstantiateCellChild(item, itemContainerPrefab,aux));
                }
            }*/
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Room"))
            {
                Destroy(item);
            }
            levelObject[levelPosition].SetActive(false);
            instantiatedRooms[levelPosition] = false;
        }
    }

    #region Room Generation

    /// <summary>
    /// Check the doors without conexions and add a random attached room
    /// </summary>
    private void AdyacentRooms()
    {
        bool brCreated; short maxRandBG = 500;
        Transform parent = levelObject[level].transform;
        char[] auxRoom = new char[5];
        int rand = Random.Range(0, maxRandBG - level * 3);
        Grid[level].AddRange(ToAdd);
        AuxToAdd.AddRange(ToAdd);
        ToAdd.Clear();
        foreach (ObjCell aux in AuxToAdd)
        {
            if (aux.xy.x > maxX)
                maxX = (short)aux.xy.x;
            else if (aux.xy.x < minX)
                minX = (short)aux.xy.x;
            if (aux.xy.y > maxY)
                maxY = (short)aux.xy.y;
            else if (aux.xy.y < minY)
                minY = (short)aux.xy.y;
            if (aux.e == true)          //if east have door
            {               //Check empty space for cell
                if (!(Grid[level].Exists(aux2 => aux2.xy == aux.xy + Vector2Int.right) || ToAdd.Exists(aux2 => aux2.xy == aux.xy + Vector2Int.right)))
                {
                    if (rand == 0 && level > 10)
                        brCreated = BigRoom('w', aux.xy + Vector2Int.right);
                    else
                        brCreated = false;
                    if (!brCreated)
                    {
                        NormalCell(aux.xy + Vector2Int.right);
                        rand = Random.Range(0, maxRandBG - level * 4);
                    }
                }
            }
            if (aux.n == true)          //if north have door
            {               //Check empty space for cell
                if (!(Grid[level].Exists(aux2 => aux2.xy == aux.xy + Vector2Int.up) || ToAdd.Exists(aux2 => aux2.xy == aux.xy + Vector2Int.up)))
                {
                    if (rand == 0 && level > 10)
                        brCreated = BigRoom('s', aux.xy + Vector2Int.up);
                    else
                        brCreated = false;
                    if (!brCreated)
                    {
                        NormalCell(aux.xy + Vector2Int.up);
                        rand = Random.Range(0, maxRandBG - level * 4);
                    }
                }
            }
            if (aux.s == true)          //if south have door
            {               //Check empty space for cell
                if (!(Grid[level].Exists(aux2 => aux2.xy == aux.xy + Vector2Int.down) || ToAdd.Exists(aux2 => aux2.xy == aux.xy + Vector2Int.down)))
                {
                    if (rand == 0 && level > 10)
                        brCreated = BigRoom('n', aux.xy + Vector2Int.down);
                    else
                        brCreated = false;
                    if (!brCreated)
                    {
                        NormalCell(aux.xy + Vector2Int.down);
                        rand = Random.Range(0, maxRandBG - level * 4);
                    }
                }
            }
            if (aux.w == true)          //if west have door
            {               //Check empty space for cell
                if (!(Grid[level].Exists(aux2 => aux2.xy == aux.xy + Vector2Int.left) || ToAdd.Exists(aux2 => aux2.xy == aux.xy + Vector2Int.left)))
                {
                    if (rand == 0 && level > 10)
                        brCreated = BigRoom('e', aux.xy + Vector2Int.left);
                    else
                        brCreated = false;
                    if (!brCreated)
                    {
                        NormalCell(aux.xy + Vector2Int.left);
                        rand = Random.Range(0, maxRandBG - level * 4);
                    }
                }
            }
        }
        AuxToAdd.Clear();
    }

    private void NormalCell(Vector2Int xy)
    {
        byte walls = 0;
        Transform parent = levelObject[level].transform;
        char[] auxRoom = new char[5];
        auxRoom[0] = EastInteraction(xy);
        auxRoom[1] = NorthInteraction(xy);
        auxRoom[2] = SouthInteraction(xy);
        auxRoom[3] = WestInteraction(xy);
        foreach (char i in auxRoom)
        {
            if (i.Equals('_'))
                walls++;
        }
        auxRoom[4] = SetCell(walls > 2);
        ToAdd.Add(CreateObjCell(auxRoom[0], auxRoom[1], auxRoom[2], auxRoom[3], auxRoom[4], xy, parent));
    }

    private bool BigRoom(char connectedRoom, Vector2Int aux)
    {
        bool created = false;
        Vector2Int vec1 = Vector2Int.zero, vec2 = Vector2Int.zero, vec3 = Vector2Int.zero, vec4 = Vector2Int.zero;
        Transform parent = levelObject[level].transform;
        switch (connectedRoom)
        {
            case 'e':
                if (!(Grid[level].Exists(aux2 => aux2.xy == aux + Vector2Int.left) || ToAdd.Exists(aux2 => aux2.xy == aux + Vector2Int.left)))
                {
                    if (!(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, 1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, 1))) &&
                        !(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(0, 1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(0, 1))))
                    {
                        vec1 = aux + new Vector2Int(-1, 1);
                        vec2 = aux + Vector2Int.up;
                        vec3 = aux + Vector2Int.left;
                        vec4 = aux;
                    }
                    else if (!(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, -1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, -1))) &&
                        !(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(0, -1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(0, -1))))
                    {
                        vec1 = aux + Vector2Int.left;
                        vec2 = aux;
                        vec3 = aux + new Vector2Int(-1, -1);
                        vec4 = aux + Vector2Int.down;
                    }
                }
                break;
            case 'n':
                if (!(Grid[level].Exists(aux2 => aux2.xy == aux + Vector2Int.down) || ToAdd.Exists(aux2 => aux2.xy == aux + Vector2Int.down)))
                {
                    if (!(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(1, -1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(1, -1))) &&
                        !(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(1, 0)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(1, 0))))
                    {
                        vec1 = aux;
                        vec2 = aux + Vector2Int.right;
                        vec3 = aux + Vector2Int.down;
                        vec4 = aux + new Vector2Int(1, -1);
                    }
                    else if (!(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, -1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, -1))) &&
                        !(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, 0)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, 0))))
                    {
                        vec1 = aux + Vector2Int.left;
                        vec2 = aux;
                        vec3 = aux + new Vector2Int(-1, -1);
                        vec4 = aux + Vector2Int.down;
                    }
                }
                break;
            case 's':
                if (!(Grid[level].Exists(aux2 => aux2.xy == aux + Vector2Int.up) || ToAdd.Exists(aux2 => aux2.xy == aux + Vector2Int.up)))
                {
                    if (!(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(1, 1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(1, 1))) &&
                        !(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(1, 0)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(1, 0))))
                    {
                        vec1 = aux + Vector2Int.up;
                        vec2 = aux + new Vector2Int(1, 1);
                        vec3 = aux;
                        vec4 = aux + Vector2Int.right;
                    }
                    else if (!(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, 1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, 1))) &&
                        !(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, 0)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(-1, 0))))
                    {
                        vec1 = aux + new Vector2Int(-1, 1);
                        vec2 = aux + Vector2Int.up;
                        vec3 = aux + Vector2Int.left;
                        vec4 = aux;
                    }
                }
                break;
            case 'w':
                if (!(Grid[level].Exists(aux2 => aux2.xy == aux + Vector2Int.right) || ToAdd.Exists(aux2 => aux2.xy == aux + Vector2Int.right)))
                {
                    if (!(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(1, 1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(1, 1))) &&
                        !(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(0, 1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(0, 1))))
                    {
                        vec1 = aux + Vector2Int.up;
                        vec2 = aux + new Vector2Int(1, 1);
                        vec3 = aux;
                        vec4 = aux + Vector2Int.right;
                    }
                    else if (!(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(1, -1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(1, -1))) &&
                        !(Grid[level].Exists(aux2 => aux2.xy == aux + new Vector2Int(0, -1)) || ToAdd.Exists(aux2 => aux2.xy == aux + new Vector2Int(0, -1))))
                    {
                        vec1 = aux;
                        vec2 = aux + Vector2Int.right;
                        vec3 = aux + Vector2Int.down;
                        vec4 = aux + new Vector2Int(1, -1);
                    }
                }
                break;
        }
        if (vec1 != Vector2Int.zero || vec2 != Vector2Int.zero || vec3 != Vector2Int.zero || vec4 != Vector2Int.zero)
        {
            ToAdd.Add(CreateObjCell('E', NorthInteraction(vec1), 'S', WestInteraction(vec1), '1', vec1, parent));
            ToAdd.Add(CreateObjCell(EastInteraction(vec2), NorthInteraction(vec2), 'S', 'W', '2', vec2, parent));
            ToAdd.Add(CreateObjCell('E', 'N', SouthInteraction(vec3), WestInteraction(vec3), '3', vec3, parent));
            ToAdd.Add(CreateObjCell(EastInteraction(vec4), 'N', SouthInteraction(vec4), 'W', '4', vec4, parent));
            created = true;
        }
        return created;

    }

    private char EastInteraction(Vector2Int input)
    {
        char output;
        bool grid = Grid[level].Exists(aux => aux.xy == input + Vector2Int.right),
            toAdd = ToAdd.Exists(aux => aux.xy == input + Vector2Int.right);
        if (grid || toAdd)
        {
            if ((grid && Grid[level].Find(aux => aux.xy == input + Vector2Int.right).w == true)
                || (toAdd && ToAdd.Find(aux => aux.xy == input + Vector2Int.right).w == true))
            {
                output = 'e';
            }
            else
            {
                output = '_';
            }
        }
        else
        {
            output = RndWll('e');
        }
        return output;
    }

    private char NorthInteraction(Vector2Int input)
    {
        char output;
        bool grid = Grid[level].Exists(aux => aux.xy == input + Vector2Int.up),
            toAdd = ToAdd.Exists(aux => aux.xy == input + Vector2Int.up);
        if (grid || toAdd)
        {
            if ((grid && Grid[level].Find(aux => aux.xy == input + Vector2Int.up).s == true)
                || (toAdd && ToAdd.Find(aux => aux.xy == input + Vector2Int.up).s == true))
            {
                output = 'n';
            }
            else
            {
                output = '_';
            }

        }
        else
        {
            output = RndWll('n');
        }
        return output;
    }

    private char SouthInteraction(Vector2Int input)
    {
        char output;
        bool grid = Grid[level].Exists(aux => aux.xy == input + Vector2Int.down),
            toAdd = ToAdd.Exists(aux => aux.xy == input + Vector2Int.down);
        if (grid || toAdd)
        {
            if ((grid && Grid[level].Find(aux => aux.xy == input + Vector2Int.down).n == true)
                || (toAdd && ToAdd.Find(aux => aux.xy == input + Vector2Int.down).n == true))
            {
                output = 's';
            }
            else
            {
                output = '_';
            }

        }
        else
        {
            output = RndWll('s');
        }
        return output;
    }

    private char WestInteraction(Vector2Int input)
    {
        char output;
        bool grid = Grid[level].Exists(aux => aux.xy == input + Vector2Int.left),
            toAdd = ToAdd.Exists(aux => aux.xy == input + Vector2Int.left);
        if (grid || toAdd)
        {
            if ((grid && Grid[level].Find(aux => aux.xy == input + Vector2Int.left).e == true)
                || (toAdd && ToAdd.Find(aux => aux.xy == input + Vector2Int.left).e == true))
            {
                output = 'w';
            }
            else
            {
                output = '_';
            }
        }
        else
        {
            output = RndWll('w');
        }
        return output;
    }

    /// <summary>
    /// Chose random between a wall or a door
    /// </summary>
    /// <param name="input">'e', 'n', 's' or 'w'</param>
    /// <returns> char posicion or '_' </returns>
    private char RndWll(char input)
    {
        //IMPORNTANT!!! Grid[level].Count is necesary to not get stuck in a infinite while in CreateLevel()
        float max = 10 + level * 0.5f + Grid[level].Count * 0.5f;
        if (Random.Range(-20, max) > 0)
        {
            input = '_';
        }
        return input;
    }

    /// <summary>
    /// Chose between a room or a corridor
    /// </summary>
    /// <param name="notRandom"> select randomness or not </param>
    /// <returns> 'R' or 'C'</returns>
    private char SetCell(bool notRandom)
    {
        if (notRandom)
        {
            return 'R';
        }
        else
        {
            if (Random.Range(-30, Grid[level].Count * 0.1f) > 0)
            {
                return 'R';
            }
            else
            {
                return 'C';
            }
        }
    }

    private void ExtraRooms()
    {
        float probability = 20
            , random = Random.Range(0f, 100f);
        if (random <= probability)
        {
            for (short x = minX; x <= maxX; x++)
            {
                for (short y = minY; y <= maxY; y++)
                {
                    if (random <= probability && SecretRoom(x, y))
                    {
                        totalEspecialRooms++;
                        Grid[level].Add(CreateObjCell('_', '_', '_', '_', 'R', new Vector2Int(x, y), levelObject[level].transform));
                        random = Random.Range(0f, 100f);
                    }
                    if (random <= probability / 4 && SuperSecretRoom(x, y))
                    {
                        totalEspecialRooms++;
                        Debug.Log(level - 1 + " SSR");
                        Grid[level - 1].Add(CreateObjCell('_', '_', '_', '_', 'R', new Vector2Int(x, y), levelObject[level - 1].transform));
                        random = Random.Range(0f, 100f);
                    }
                    if (random > probability) { return; }
                }
            }
        }
    }


    private bool SecretRoom(short x, short y)
    {
        /*bool b1, b2, b3;
        bool b4, b5, b6;
        bool b7, b8, b9;
        foreach (ObjCell item in Grid[level])
        {
            if(item.xy== new Vector2Int(x, y))
            {

            }
        }*/
        return (!Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x, y))
               && !Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x + 1, y))
               && !Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x - 1, y))
               && !Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x, y + 1))
               && !Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x, y - 1))
               && (Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x + 1, y + 1))
                || Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x - 1, y - 1))
                || Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x - 1, y + 1))
                || Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x + 1, y - 1))
                  )
               );
    }

    private bool SuperSecretRoom(int x, int y)
    {
        return Grid[level].Exists(aux2 => aux2.xy == new Vector2Int(x, y))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x, y))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x + 1, y + 1))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x + 0, y + 1))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x - 1, y + 1))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x - 1, y + 0))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x - 1, y - 1))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x + 0, y - 1))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x + 1, y - 1))
               && !Grid[level - 1].Exists(aux2 => aux2.xy == new Vector2Int(x + 1, y + 0));
    }

    /// <summary>
    /// Create ObjCell given the locations of the walls and doors, if is room or corridor, ubication in the grid and transform of the GameObject level
    /// </summary>
    /// <param name="E">'e' or '_'</param>
    /// <param name="N">'n' or '_'</param>
    /// <param name="S">'s' or '_'</param>
    /// <param name="W">'w' or '_'</param>
    /// <param name="X">'R' or 'C'</param>
    /// <param name="XY">Grid ubication</param>
    /// <param name="gridZ">Transform of the GameObject level x</param>
    /// <returns>ObjCell</returns>
    private ObjCell CreateObjCell(char E, char N, char S, char W, char X, Vector2Int XY, Transform gridZ)
    {
        bool? e, n, s, w;
        bool room = false, bigroom = false;
        if (E.Equals('E'))
            e = null;
        else
            e = E.Equals('e');
        if (N.Equals('N'))
            n = null;
        else
            n = N.Equals('n');
        if (S.Equals('S'))
            s = null;
        else
            s = S.Equals('s');
        if (W.Equals('W'))
            w = null;
        else
            w = W.Equals('w');
        if (!X.Equals('C'))
        {
            room = true;
            if (!X.Equals('R'))
                bigroom = true;
        }
        return new ObjCell(
            new PreInstance(PrefabCell(E, N, S, W, X),
            gridZ, new Vector2(spaceX * XY.x, spaceY * XY.y)),
            XY, e, n, s, w, room, bigroom
            );
    }

    /// <summary>
    /// Given the locations of the walls and doors returns the prefab of the cell
    /// </summary>
    /// <param name="E">'e' or '_'</param>
    /// <param name="N">'n' or '_'</param>
    /// <param name="S">'s' or '_'</param>
    /// <param name="W">'w' or '_'</param>
    /// <returns>Prefab</returns>
    private GameObject PrefabCell(char E, char N, char S, char W, char X)
    {
        string sRoom;
        if (X.Equals('R') || X.Equals('C'))
        {
            sRoom = new string(new char[] { E, N, S, W, ' ', X });
        }
        else
        {
            sRoom = new string(new char[] { X, ' ', E, N, S, W });
        }
        foreach (GameObject prefab in cellPrefabs)
        {
            if (sRoom == prefab.name)
                return prefab;
        }
        Debug.LogError("nullPrefab");
        return null;
    }

    #endregion

    /// <summary>
    /// Adds a enemies, crates and barrels to a room
    /// </summary>
    /// <param name="maxEnemies">max enemies</param>
    /// <param name="minEnemies">min enemies</param>
    /// <param name="grid">position of the room</param>
    /// <param name="level">N° level</param>
    /// <param name="enemiesParent">parent object enemies</param>
    /// <param name="cratesParent">parent object crates</param>
    private void CellChildSpawner(float maxEnemies, float minEnemies, Vector2 grid, byte level, Transform enemiesParent, Transform cratesParent)
    {
        //Debug.Log("CellChildSpawner");
        if (grid != Vector2Int.zero)
        {
            float roomX = (spaceX / 2) - 0.5f;
            float roomY = (spaceY / 2) - 0.5f;
            grid.x = spaceX * grid.x;
            grid.y = spaceY * grid.y;
            List<CellChild> aux = new List<CellChild>();
            if (maxEnemies > 0 && minEnemies > 0)
            {
                if (level > 8)
                {
                    minEnemies = minEnemies * level * 0.15f;
                    maxEnemies = maxEnemies * level * 0.15f;
                }
                //Group of Enemies
                for (ushort i = 0; i < Random.Range(minEnemies, maxEnemies); i++)
                {
                    aux.Add(
                        new CellChild(
                            new PreInstance(
                                enemyPrefabs[0], enemiesParent,
                                new Vector2(Random.Range(-roomX, roomX) + grid.x, Random.Range(-roomY, roomY) + grid.y)
                                )
                            )
                        );
                    totalEnemies++;
                }
                Enemies[level - 1].AddRange(aux);
                aux.Clear();
            }
            short crates = (short)Random.Range(-5, 10);
            Vector2 pointXY = new Vector2(Random.Range(-roomX, roomX) + grid.x, Random.Range(-roomY, roomY) + grid.y);
            if (crates > 2)
            {
                //Group of Crates
                for (int i = 0; i < crates; i++)
                {
                    aux.Add(
                        new CellChild(
                            new PreInstance(cratePrefab, cratesParent,
                                new Vector2(Random.Range(-0.2f, 0.2f) + pointXY.x, Random.Range(-0.2f, 0.2f) + pointXY.y)
                                )
                            )
                        );
                    totalCrates++;
                }
                //Barrel in Group of Crates
                if (Random.Range(0, 180 - level) == 0)
                {
                    aux.Add(
                        new CellChild(
                            new PreInstance(barrelPrefab, cratesParent,
                                new Vector2(Random.Range(-0.2f, 0.2f) + pointXY.x, Random.Range(-0.2f, 0.2f) + pointXY.y)
                                )
                            )
                        );
                    Debug.Log(level + " Barrel");
                    totalBarrel++;
                }
                Crates[level - 1].AddRange(aux);
                aux.Clear();
            }
            //Group of Barrels
            else if (Random.Range(0, 100000) == 0)
            {
                byte j = (byte)Random.Range(5, 10);
                for (int i = 0; i < j; i++)
                {
                    aux.Add(
                        new CellChild(
                            new PreInstance(barrelPrefab, cratesParent,
                                new Vector2(Random.Range(-0.2f, 0.2f) + pointXY.x, Random.Range(-0.2f, 0.2f) + pointXY.y)
                                )
                            )
                        );
                    totalBarrel++;
                }
                Debug.Log(level.ToString() + " " + j.ToString() + "Barrel");
                Crates[level - 1].AddRange(aux);
                aux.Clear();
            }
        }
    }
}