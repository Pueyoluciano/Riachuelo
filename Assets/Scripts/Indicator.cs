using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    [Header("time")]
    [SerializeField] float transitionDuration = 1f;

    [Header("States Data")]
    [SerializeField] IndicatorData offIndicator;
    [SerializeField] IndicatorData loadingIndicator;
    [SerializeField] IndicatorData fullIndicator;

    private Image image;
    private IndicatorData currentStateValues;
    private IndicatorData targetStateValues;

    public State currentState;

    private float transitionElapsedTime;
    public enum State
    {
        Off = 0,
        Loading = 1,
        Full = 2
    }

    private Dictionary<State, IndicatorData> stateDict;

    private void Awake()
    {
        image = GetComponent<Image>();
        targetStateValues = ScriptableObject.CreateInstance<IndicatorData>();
        currentStateValues = ScriptableObject.CreateInstance<IndicatorData>();

        transitionDuration = 1f;

        stateDict = new()
        {
            {State.Off ,Instantiate(offIndicator)},
            {State.Loading, Instantiate(loadingIndicator)},
            {State.Full, Instantiate(fullIndicator)},
        };

        currentStateValues.Copy(stateDict[State.Off]);
        ForceSetState(State.Off);
    }
    private void ForceSetState(State state)
    {
        currentState = state;
        targetStateValues.Copy(stateDict[state]);
        transitionElapsedTime = 0;
        currentStateValues.sprite = stateDict[state].sprite;
        currentStateValues.color = stateDict[state].color;
    }
    public void SetState(State state)
    {
        if (state == currentState)
            return;

        ForceSetState(state);
    }

    private void Update()
    {
        currentStateValues.rotationSpeed = Mathf.Lerp(currentStateValues.rotationSpeed, targetStateValues.rotationSpeed, transitionElapsedTime / transitionDuration);
        currentStateValues.localScale = Vector3.Lerp(currentStateValues.localScale, targetStateValues.localScale, transitionElapsedTime / transitionDuration);

        if (transitionElapsedTime < transitionDuration)
        {
            transitionElapsedTime += Time.deltaTime;
        }

        image.transform.eulerAngles += new Vector3(0,0, currentStateValues.rotationSpeed);
        image.transform.localScale = currentStateValues.localScale;
        image.sprite = currentStateValues.sprite;
        image.color = currentStateValues.color;
    }
}
