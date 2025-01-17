using UnityEngine;

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parNode;
    public RectInt nodeRect; //분리된 공간의 rect정보
    public RectInt roomRect; //분리된 공간 속 방의 rect정보

    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
        //방의 가운데 점. 방과 방을 이을 때 사용
    }

    public Vector2Int RightRoomSide
    {
        get
        {
            return new Vector2Int(roomRect.x, roomRect.y + roomRect.height / 2);
        }
        //오른쪽 방의 꼭짓점  길만들때 사용
    }

    public Vector2Int LeftRoomSide
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y);
        }
        //왼쪽 방의 꼭짓점 길만들때 사용
    }

    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }
}