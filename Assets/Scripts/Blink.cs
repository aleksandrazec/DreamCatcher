using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blink : MonoBehaviour
{
    public GameObject upperObj;
    public RectTransform upperRectTransform;
    public CanvasGroup upperCanvasGroup;
    public GameObject lowerObj;
    public RectTransform lowerRectTransform;
    public CanvasGroup lowerCanvasGroup;
    public RectTransform upperStartTransform;
    public RectTransform lowerStartTransform;
    public RectTransform upperEndTransform;
    public RectTransform lowerEndTransform;
    private Vector2 _initialUpperPosition;
    private Vector2 _initialLowerPosition;
    private Vector2 _currentUpperPosition;
    private Vector2 _currentLowerPosition;

    public bool closed=false;

    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [Range(0, 1f)][SerializeField] private float animationDuration = 1f;
    private void Awake()
    {
        _initialLowerPosition=lowerObj.transform.position;
        _initialUpperPosition=upperObj.transform.position;
    }
    public void CloseEyes()
    {
        StartCoroutine(CloseEyesRoutine());
    }
    public void OpenEyes()
    {
        StartCoroutine(OpenEyesRoutine());
    }
    public IEnumerator CloseEyesRoutine()
    {
        if (!closed)
        {
            Debug.Log("closing eyes");
            _currentUpperPosition = upperObj.transform.position;
            _currentLowerPosition = lowerObj.transform.position;
            float elapsedTime = 0;
            Vector2 targetUpperPosition = upperEndTransform.position;
            Vector2 targetLowerPosition = lowerEndTransform.position;
            while (elapsedTime < animationDuration)
            {
                float evaluationAtTime = easingCurve.Evaluate(elapsedTime / animationDuration);
                upperObj.transform.position = Vector2.Lerp(_currentUpperPosition, targetUpperPosition, evaluationAtTime);
                lowerObj.transform.position = Vector2.Lerp(_currentLowerPosition, targetLowerPosition, evaluationAtTime);
                //upperCanvasGroup.alpha = Mathf.Lerp(0f, 1f, evaluationAtTime);
                //lowerCanvasGroup.alpha = Mathf.Lerp(0f, 1f, evaluationAtTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            upperObj.transform.position = targetUpperPosition;
            lowerObj.transform.position = targetLowerPosition;
            closed= true;
        }
    }
    public IEnumerator OpenEyesRoutine()
    {
        if (closed)
        {
            Debug.Log("opening eyes");
            _currentUpperPosition = upperObj.transform.position;
            _currentLowerPosition = lowerObj.transform.position;
            float elapsedTime = 0;
            Vector2 targetUpperPosition = upperStartTransform.position;
            Vector2 targetLowerPosition = lowerStartTransform.position;
            while (elapsedTime < animationDuration)
            {
                float evaluationAtTime = easingCurve.Evaluate(elapsedTime / animationDuration);
                upperObj.transform.position = Vector2.Lerp(_currentUpperPosition, targetUpperPosition, evaluationAtTime);
                lowerObj.transform.position = Vector2.Lerp(_currentLowerPosition, targetLowerPosition, evaluationAtTime);
                //upperCanvasGroup.alpha = Mathf.Lerp(0f, 1f, evaluationAtTime);
                //lowerCanvasGroup.alpha = Mathf.Lerp(0f, 1f, evaluationAtTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            upperObj.transform.position = targetUpperPosition;
            lowerObj.transform.position = targetLowerPosition;
            closed=false;
        }
    }
}
