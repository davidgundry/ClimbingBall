using UnityEngine;
using System.Collections;

public class LevelGenerator {

    private static LevelComponent[,] levelGrid;
    private static int headX;
    private static int headY;
    public const int width = 10;
    public const int height = 10;

    public static LevelComponent[,] Generate(int chunkID)
    {
        levelGrid = new LevelComponent[width, height];
        headX = 0;
        headY = 7;

        if (chunkID == 0)
        {
            headY = 5;
            levelGrid[headX, headY] = new LevelComponent(LevelAtom.None, Direction.Right, false);
            headX++;
            levelGrid[headX, headY] = new LevelComponent(LevelAtom.Hook, Direction.Right, false);
            headX++;
            levelGrid[headX, headY] = new LevelComponent(LevelAtom.Hook, Direction.Right, false);
            headX++;
            levelGrid[headX, headY] = new LevelComponent(LevelAtom.Hook, Direction.Right, false);
            headY++;
            levelGrid[headX, headY] = new LevelComponent(LevelAtom.Hook, Direction.Up, false);
            headX++;
            levelGrid[headX, headY] = new LevelComponent(LevelAtom.Hook, Direction.Right, false);
        }
        else
            levelGrid[headX, headY] = new LevelComponent(LevelAtom.Hook, Direction.Right, false);


        while(Step())
            continue;
        //AddGround(chunkID);

        return levelGrid;
    }

    public static Vector2 LevelHead()
    {
        return new Vector2(headX, headY-7);
    }

    private static bool Step()
    {
        LevelAtom oldAtom = levelGrid[headX, headY].Atom;
        Direction oldDirection = levelGrid[headX, headY].Direction;
        Move();
        LevelAtom atom = RandomLevelAtom();
        Direction direction = new Direction();


        if (oldDirection == Direction.DownRight)
        {
            atom = LevelAtom.Floor;
            direction = Direction.UpRight;
        }
        else
        {
            if (headX < 2)
                direction = Direction.Right;
            else if (headY < 2)
                direction = Direction.Up;
            else if (headY > height - 3)
                direction = Direction.Down;
            else if (headX == width)
                return false;
            else
            {
                direction = RandomDirection();
            }

            if ((oldDirection == Direction.Up) && (direction == Direction.Down))
                direction = Direction.Right;
            else if ((oldDirection == Direction.Down) && (direction == Direction.Up))
                direction = Direction.Right;
            else if ((oldDirection == Direction.Up) && (direction == Direction.DownRight))
                direction = Direction.Right;
        }

        if (atom == LevelAtom.Ground)
        {
            if (HasAtom(headX, LevelAtom.Ground,levelGrid))
                atom = LevelAtom.Hook;
        }

        bool coin = false;
        if (Random.value > 0.9)
            coin = true;

        if ((headX < width) && (headY < height) && (headX >= 0) && (headY >= 0))
            levelGrid[headX, headY] = new LevelComponent(atom, direction, coin);
        else
            return false;
        return true;
    }

    public static int GetHighestAtomExcluding(int x, LevelAtom[] excluded, LevelComponent[,] grid)
    {
        int highest = -1;
        for (int j = 0; j < grid.GetLength(1); j++)
            if (grid[x, j] != null)
            {
                bool ignored = false;
                for (int e = 0;e<excluded.Length;e++)
                    if (grid[x, j].Atom == excluded[e])
                    {
                        ignored = true;
                    }
                if (!ignored)
                    highest = j;
            }
        return highest;
    }

    public static int GetHighestAtom(int x, LevelAtom atom, LevelComponent[,] grid)
    {
        int highest = -1;
        for (int j = 0; j < grid.GetLength(1); j++)
            if (grid[x, j] != null)
            {
                if (grid[x, j].Atom == atom)
                {
                    highest = j;
                }
            }
        return highest;
    }

    public static bool HasAtom(int x, LevelAtom atom, LevelComponent[,] grid)
    {
        for (int j = 0; j < grid.GetLength(1); j++)
            if (grid[x, j] != null)
            {
                if (grid[x, j].Atom == atom)
                    return true;
            }
        return false;
    }

    private static void AddGround(int chunkID)
    {
        for (int i = 0; i < levelGrid.GetLength(0); i++)
        {
            LevelAtom[] excluded = new LevelAtom[1];
            excluded[0] = LevelAtom.Floor;
            int highest = GetHighestAtomExcluding(i, excluded, levelGrid);
            bool ground = HasAtom(i, LevelAtom.Ground, levelGrid);

            if ((!ground) && (highest > -1) && (highest + 2 < height))
            {
                if ((Random.value > 0.25f) || (chunkID == 0))
                    levelGrid[i, highest + 2] = new LevelComponent(LevelAtom.Ground, Direction.Down, false);
                else
                    levelGrid[i, highest + 2] = new LevelComponent(LevelAtom.Waterfall, Direction.Down, false);
            }
        }
    }

    private static Direction RandomDirection()
    {
        switch ((int)Random.Range(0, 4))
        {
            case 0:
                return Direction.Up;
            case 1:
                return Direction.Right;
            case 2:
                return Direction.Down;
            case 3:
                return Direction.DownRight;
        }
        return Direction.Right;
    }

    private static LevelAtom RandomLevelAtom()
    {
        switch ((int)Random.Range(0, 3))
        {
            case 0:
                return LevelAtom.Hook;
            case 1:
                return LevelAtom.Ground;
        }
        return LevelAtom.Hook;
    }

    private static void Move()
    {
        switch(levelGrid[headX,headY].Direction)
        {
            case Direction.Up:
                headY++;
                break;
            case Direction.Right:
                headX++;
                break;
            case Direction.Down:
                headY--;
                break;
            case Direction.DownRight:
                headY--;
                headX++;
                break;
            case Direction.UpRight:
                headY++;
                headX++;
                break;
        }
    }
}

public enum LevelAtom{
    None,
    Hook,
    Wall,
    Floor,
    Blade,
    Ground,
    Waterfall,
}

public enum Direction
{
    Up,
    Right,
    Down,
    DownRight,
    UpRight
}

public class LevelComponent
{
    private readonly LevelAtom atom;
    public LevelAtom Atom
    {
        get
        {
            return atom;
        }
    }

    private readonly Direction dir;
    public Direction Direction
    {
        get
        {
            return dir;
        }
    }

    private readonly bool coin;
    public bool Coin
    {
        get
        {
            return coin;
        }
    }

    public LevelComponent(LevelAtom atom, Direction dir, bool coin)
    {
        this.atom = atom;
        this.dir = dir;
        this.coin = coin;
    }

}