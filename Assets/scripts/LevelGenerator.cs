using UnityEngine;
using System.Collections;

public class LevelGenerator {

    private static LevelComponent[,] levelGrid;
    private static int headX;
    private static int headY;
    private static int width;
    private static int height;

    public static LevelComponent[,] Generate()
    {
        height = 10;
        width = 10;
        levelGrid = new LevelComponent[width, height];
        headX = 0;
        headY = 7;
        levelGrid[headX,headY] = new LevelComponent(LevelAtom.Hook,Direction.Right);

        while(Step())
            continue;
        AddGround();

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
        }

        levelGrid[headX, headY] = new LevelComponent(atom, direction);
        return true;
    }

    private static void AddGround()
    {
        for (int i = 0; i < levelGrid.GetLength(0); i++)
        {
            bool ground = false;
            int highest = -1;
            for (int j = 0; j < levelGrid.GetLength(1); j++)
                if (levelGrid[i, j] != null)
                {
                    if (levelGrid[i, j].Atom != LevelAtom.Floor)
                    {
                        highest = j;
                        if (levelGrid[i, j].Atom == LevelAtom.Ground)
                            ground = true;
                    }
                }
            if ((!ground) && (highest > -1) && (highest+2 < height))
                levelGrid[i, highest + 2] = new LevelComponent(LevelAtom.Ground, Direction.Down);
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
    Hook,
    Wall,
    Floor,
    Blade,
    Ground,
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

    public LevelComponent(LevelAtom atom, Direction dir)
    {
        this.atom = atom;
        this.dir = dir;
    }

}