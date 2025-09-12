using UnityEngine;

public class CharacterStatePositions : MonoBehaviour
{
    [Header("State Positions")]
    public Vector3 state0Position = Vector3.zero;
    public Vector3 state1Position = Vector3.zero;
    public Vector3 state2Position = Vector3.zero;
    public Vector3 state3Position = Vector3.zero;
    public Vector3 state4Position = Vector3.zero;

    [Header("Mirror Settings")]
    public bool state0MirrorX = false;
    public bool state1MirrorX = false;
    public bool state2MirrorX = false;
    public bool state3MirrorX = false;
    public bool state4MirrorX = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Get position for specific state
    public Vector3 GetPositionForState(int state)
    {
        // ͬʱ���þ���ת
        bool mirror = GetMirrorForState(state);
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = mirror;
        }

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

    // ��������ȡĳ��״̬�ľ�������
    public bool GetMirrorForState(int state)
    {
        switch (state)
        {
            case 0: return state0MirrorX;
            case 1: return state1MirrorX;
            case 2: return state2MirrorX;
            case 3: return state3MirrorX;
            case 4: return state4MirrorX;
            default: return false;
        }
    }

    // ����������ĳ��״̬�ľ�������
    public void SetMirrorForState(int state, bool mirror)
    {
        switch (state)
        {
            case 0: state0MirrorX = mirror; break;
            case 1: state1MirrorX = mirror; break;
            case 2: state2MirrorX = mirror; break;
            case 3: state3MirrorX = mirror; break;
            case 4: state4MirrorX = mirror; break;
        }
    }
}