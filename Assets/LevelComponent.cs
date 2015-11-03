using UnityEngine;
using System.Collections;



public class LevelComponent {

    public LevelAtom[] atoms;
    public Vector2[] positions;
    public Vector2 endPosition;

    public LevelComponent(LevelAtom[] atoms, Vector2[] positions, Vector2 endPosition)
    {
        this.atoms = atoms;
        this.positions = positions;
        this.endPosition = endPosition;
    }




}
