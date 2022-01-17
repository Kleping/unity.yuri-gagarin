using System.Collections.Generic;
using UnityEngine;

public class Dimensions : MonoBehaviour, IDimensions
{
    private readonly Dictionary<DimensionType, Vector3> _calculatedDimensions =
        new Dictionary<DimensionType, Vector3>();
    
    
    public Vector3 GetDimension(DimensionType type)
    {
        var cam = Camera.main;
        if (cam == null) return Vector3.zero;
        if (_calculatedDimensions.ContainsKey(type)) return _calculatedDimensions[type];
        var checkable = -(Vector2.right + Vector2.up);
        var centroid = new Vector2(checkable.x, checkable.y);

        switch (type)
        {
            case DimensionType.LeftTop:     centroid = new Vector2(0F, Screen.height);                 break;
            case DimensionType.Top:         centroid = new Vector2(Screen.width/2F, Screen.height);    break;
            case DimensionType.RightTop:    centroid = new Vector2(Screen.width, Screen.height);       break;
            case DimensionType.LeftMiddle:  centroid = new Vector2(0F, Screen.height/2F);              break;
            case DimensionType.Middle:      centroid = new Vector2(Screen.width/2F, Screen.height/2F); break;
            case DimensionType.RightMiddle: centroid = new Vector2(Screen.width, Screen.height/2F);    break;
            case DimensionType.LeftBottom:  centroid = Vector2.zero;                                         break;
            case DimensionType.Bottom:      centroid = new Vector2(Screen.width/2F, 0F);               break;
            case DimensionType.RightBottom: centroid = new Vector2(Screen.width, 0F);                  break;
        }

        if (centroid == checkable) return Vector3.zero;
        var pos = new Vector3(centroid.x, centroid.y, cam.nearClipPlane);
        _calculatedDimensions[type] = cam.ScreenToWorldPoint(pos);
        return _calculatedDimensions[type];
    }
}

public interface IDimensions
{
    Vector3 GetDimension(DimensionType type);
}

public enum DimensionType
{
    LeftTop, Top, RightTop,
    LeftMiddle, Middle, RightMiddle,
    LeftBottom, Bottom, RightBottom,
};
