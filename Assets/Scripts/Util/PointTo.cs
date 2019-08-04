using UnityEngine;

public class PointTo : MonoBehaviour
{
    public PointTo createdPointTo = null;
    public bool locked = false;

    public Transform start;
    public Transform end;

    public bool useMouseForEndPosition = true;

    private RectTransform deformed;

    public void Awake()
    {
        deformed = GetComponent<RectTransform>();
        deformed.pivot = new Vector2(0.0f, 0.5f);
    }

    // Destroy all line children to this one
    private void OnDestroy()
    {
        if (createdPointTo != null) { Destroy(createdPointTo.gameObject); }
    }

    // Update line position
    public void DrawLine()
    {
        Vector3 lStartPosition = start.position;
        Vector3 lEndPosition = Vector3.one;
        if (end != null) { lEndPosition = end.position; }
        if (useMouseForEndPosition) { lEndPosition = Input.mousePosition; }

        Vector3 lDeltaPosition = lEndPosition - lStartPosition;
        lDeltaPosition.z = 0;

        deformed.position = lStartPosition;

        Vector3 lDirection = lDeltaPosition.normalized;
        deformed.rotation = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), lDirection);

        lDeltaPosition = deformed.worldToLocalMatrix * lDeltaPosition;
        deformed.sizeDelta = new Vector2(lDeltaPosition.magnitude, deformed.rect.height);
    }

    // Create a new line which follows the mouse
    public PointTo CreateLine(bool pFollowMouse = true, Transform pStartTransform = null)
    {
        // Instantiates a copy of this gameObject
        var g = Instantiate(gameObject, transform.position, Quaternion.identity);
        g.SetActive(false);
        g.transform.SetParent(gameObject.transform.parent);
        g.transform.SetAsFirstSibling();
        g.transform.localScale = Vector3.one;
        var lScript = g.GetComponent<PointTo>();
        // Create a empty point to store the new PointTo.start transform if no transform in parameters
        Transform lStartTransform = null;
        if (pStartTransform == null)
        {
            lStartTransform = new GameObject("UI line point").transform;
            lStartTransform.transform.position = Input.mousePosition;
        }
        else
        {
            lStartTransform = pStartTransform;
        }
        lScript.start = lStartTransform;
        lScript.useMouseForEndPosition = pFollowMouse;
        lScript.locked = false;
        lScript.DrawLine();
        g.SetActive(true);

        this.createdPointTo = lScript;

        return lScript;
    }

    public void SetLineWidth(float pLineWidth = 55f)
    {
        if (deformed == null) { deformed = GetComponent<RectTransform>(); }
        var lTempSizeDelta = deformed.sizeDelta;
        lTempSizeDelta = new Vector2(lTempSizeDelta.x, pLineWidth);
        deformed.sizeDelta = lTempSizeDelta;
    }

}//PointTo
