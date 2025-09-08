using UnityEngine;

public class CharacterStatePositions : MonoBehaviour
{
    [Header("State Positions")]
    public Vector3 state0Position = Vector3.zero;
    public Vector3 state1Position = Vector3.zero;
    public Vector3 state2Position = Vector3.zero;
    public Vector3 state3Position = Vector3.zero;
    public Vector3 state4Position = Vector3.zero;
    
    // Get position for specific state
    public Vector3 GetPositionForState(int state)
    {
        switch (state)
        {
            case 0: return state0Position;
            case 1: return state1Position;
            case 2: return state2Position;
            case 3: return state3Position;
            case 4: return state4Position;
            default: return state0Position;
        }
    }
    
    // Set position for specific state
    public void SetPositionForState(int state, Vector3 position)
    {
        switch (state)
        {
            case 0: state0Position = position; break;
            case 1: state1Position = position; break;
            case 2: state2Position = position; break;
            case 3: state3Position = position; break;
            case 4: state4Position = position; break;
        }
    }
}
