using UnityEngine;
using System.Collections;

public class LevelGenerator {

    private static LevelComponent[,] levelGrid;
    private static int headX;
    private static int headY;

    public static LevelGrid[,] Generate()
    {
        int width = 10;
        int height = 10;
        levelGrid = new LevelComponent[width, height];
        headX = 0;
        headY = 3;
        levelGrid(headX,headY) = new LevelComponent(LevelAtom.Hook,Direction.Right);
        Step();
        Step();
        return levelGrid;
    }

    private static void Step()
    {
        Move();
        levelGrid(headX, headY) = new LevelComponent(LevelAtom.Hook, RandomDirection());
    }

    private static Direction RandomDirection()
    {
        switch ((int)Random.Range(0, 3))
        {
            case 0:
                return Direction.Up;
            case 1:
                return Direction.Right;
            case 2:
                return Direction.Down;
        }
        return Direction.Right;
    }

    private static void Move()
    {
        switch(levelGrid(headX,headY).Direction)
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
        }
    }
}

public enum LevelAtom{
    Hook,
    Wall,
    Floor,
    Blade
}

public enum Direction
{
    Up,
    Right,
    Down
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

    LevelComponent(LevelAtom atom, Direction dir)
    {
        this.atom = atom;
        this.dir = dir;
    }

}