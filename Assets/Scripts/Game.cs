using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;
    private bool onStart = false;
    private float clearTime;
    private bool gameover;
    private int winTimes = 0;
    private int mineLeft;

    public Text statusText;
    private Board board;
    private Cell[,] state;

    [SerializeField] Text timeText;
    [SerializeField] Text winTimeCount;
    [SerializeField] Text leftMine;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0 , width*height);
    }

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        //Debug.Log("newgame");
        statusText.text = "Game Start!";
        state = new Cell[width, height];
        gameover = false;
        mineLeft = mineCount;
        leftMine.text = mineLeft.ToString();

        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);
        clearTime = 0;
        onStart = false;
    }

    private void GenerateMines()
    {
        for(int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            /*while (state[x,y].type == Cell.Type.Mine)
            {
                x++;
                if(x >= width){
                    x = 0;
                    y++;
                }
            }*/
            if(state[x, y].type == Cell.Type.Mine)
            {
                i--;
                continue;
            }

            state[x, y].type = Cell.Type.Mine;
            //state[x,y].revealed = true;//test
        }
    }

    private void GenerateCells()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }

     private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x,y];

                if (cell.type == Cell.Type.Mine) continue;

                cell.number = CountMines(x, y);

                if(cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }
                //cell.revealed = true;//test
                state[x,y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int count = 0;
        for (int adjacentX = -1; adjacentX <=1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) continue;
                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                //if (x < 0 || x >= width || y < 0 || y >= height) continue;

                if (GetCell(x,y).type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private int CountNeiF(int cellX, int cellY)
    {
        //Debug.Log("countneif");
        int count = 0;
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) continue;
                int x = cellX + adjacentX;
                int y = cellY + adjacentY;
                if (x < 0 || x >= width || y < 0 || y >= height) continue;
                Cell cell = state[x, y];
                if (cell.flagged)
                {
                    count++;
                }
            }
        }
        //Debug.Log(count);
        return count;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            NewGame();
            //onStart = false;
        } 
        if (!gameover)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }
        
    }


    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid) return;
        
        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
        if (cell.flagged) mineLeft--;
        else if (!cell.flagged) mineLeft++;
        leftMine.text = mineLeft.ToString();
    }

    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.flagged) return;
        if (onStart == false) onStart = true;
        if ( cell.revealed ) ExpandNum(cell);

        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                CheckWinCondition();
                break;
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }

        /*if(cell.type == Cell.Type.Empty) Flood(cell);

        cell.revealed = true;
        state[cellPosition.x,cellPosition.y] = cell;*/

        board.Draw(state);
    }

    private void Flood(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        if(cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x-1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x , cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));

            Flood(GetCell(cell.position.x-1, cell.position.y - 1));
            Flood(GetCell(cell.position.x-1, cell.position.y + 1));
            Flood(GetCell(cell.position.x+1, cell.position.y + 1));
            Flood(GetCell(cell.position.x+1, cell.position.y - 1));
        }
    }

    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y)) return state[x, y];
        else return new Cell();
    }

    private void ExpandNum(Cell cell)
    {
        //Debug.Log("expandnum");
        int cellx = cell.position.x;
        int celly = cell.position.y;
        if (cell.number == CountNeiF(cellx, celly))
        {
            for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
            {
                for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                {
                    if (adjacentX == 0 && adjacentY == 0) continue;
                    int x = cellx + adjacentX;
                    int y = celly + adjacentY;
                    if (x < 0 || x >= width || y < 0 || y >= height) continue;
                    Cell cellt = state[x, y];
                    if (cellt.flagged && cellt.type != Cell.Type.Mine) Explode(cell);
                    if(!cellt.revealed && cellt.type == Cell.Type.Number)
                    {
                        cellt.revealed = true;
                        state[x, y] = cellt;
                        board.Draw(state);
                    }else if(!cellt.revealed && cellt.type == Cell.Type.Empty)
                    {
                        Flood(cellt);
                        board.Draw(state);
                    }
                    //Debug.Log(cellt.position.x + "+" + cellt.position.y);
                }
            }
        }
    }

    private void Explode(Cell cell)
    {
        Debug.Log("gameover");
        statusText.text = "You Lose :(";
        onStart = false;
        gameover = true;

        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x,cell.position.y] = cell;

        for(int x=0; x < width; x++)
        {
            for(int y=0; y < height; y++)
            {
                cell = state[x,y];

                if(cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x,y] = cell;
                }
            }
        }
    }

    private void CheckWinCondition()
    {
        for(int x=0; x < width;x++)
        {
            for (int y=0; y < height;y++)
            {
                Cell cell = state[x,y];

                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }

        Debug.Log("Win!");
        statusText.text = "You Win! :)";
        winTimes++;
        winTimeCount.text = winTimes.ToString();
        gameover = true;
        onStart = false;
    }

    private bool IsValid(int x, int y)
    {
        return x>=0 && x<width && y>=0 && y<height;
    }

    //UI
    private void FixedUpdate()
    {
        if(onStart == false)
        {
            return;
        }
        else if(onStart == true)
        {
            clearTime += Time.fixedDeltaTime;
            timeText.text = System.TimeSpan.FromSeconds(value: clearTime).ToString(format: @"mm\:ss\:ff");
        }
        
        //Debug.Log(winTimes);
    }

}
