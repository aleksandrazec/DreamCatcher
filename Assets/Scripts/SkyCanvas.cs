using System.Collections;
using UnityEngine;

public class SkyCanvas : MonoBehaviour
{
    public GameObject skyObj;
    public RectTransform skyRectTransform;
    public RectTransform skyStartTransform;
    public RectTransform skyEndTransform;
    private Vector2 _currentPosition;
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Range(0, 1f)][SerializeField] private float animationDuration = 1f;
    public void AnimateSky()
    {
        StartCoroutine(AnimateSkyRoutine());
    }
    public void SetSkyToStart()
    {
        skyObj.transform.position=skyStartTransform.position;
    }
    public IEnumerator AnimateSkyRoutine()
    {
        _currentPosition = skyObj.transform.position;
        float elapsedTime = 0;
        Vector2 targetPosition = skyEndTransform.position;
        while (elapsedTime < animationDuration)
        {
            float evaluationAtTime = easingCurve.Evaluate(elapsedTime / animationDuration);
            skyObj.transform.position = Vector2.Lerp(_currentPosition, targetPosition, evaluationAtTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        skyObj.transform.position = targetPosition;
    }
}
