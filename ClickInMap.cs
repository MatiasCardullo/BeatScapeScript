
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickInMap : MonoBehaviour,IPointerClickHandler
{
    //Top Down Camera
    public Camera map;
    public RawImage image;
    public GameObject point;
    public Vector2 viewportClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, eventData.position, null, out Vector2 localClick);
        localClick.y = (image.rectTransform.rect.yMin *-1) -(localClick.y * -1);

        viewportClick = new Vector2(localClick.x / image.rectTransform.rect.xMax, localClick.y / (image.rectTransform.rect.yMin *-1));

        Ray ray = map.ViewportPointToRay(new Vector3(viewportClick.x, viewportClick.y, 0));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickPoint = Instantiate(point);
            clickPoint.transform.position = hit.point;
        }
    }
}