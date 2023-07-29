using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class TextSwitcherMixerBehaviour : PlayableBehaviour
{
    private Color m_DefaultColor;
    private float m_DefaultFontSize;
    private string m_DefaultText;

    private TextMeshProUGUI m_TrackBinding;
    private bool m_FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as TextMeshProUGUI;

        if (m_TrackBinding == null)
        {
            return;
        }

        if (!m_FirstFrameHappened)
        {
            m_DefaultColor = m_TrackBinding.color;
            m_DefaultFontSize = m_TrackBinding.fontSize;
            m_DefaultText = m_TrackBinding.text;
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();

        Color blendedColor = Color.clear;
        float blendedFontSize = 0f;
        float totalWeight = 0f;
        float greatestWeight = 0f;
        int currentInputs = 0;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<TextSwitcherBehaviour>)playable.GetInput(i);
            TextSwitcherBehaviour input = inputPlayable.GetBehaviour();

            blendedColor += input.color * inputWeight;
            blendedFontSize += input.fontSize * inputWeight;
            totalWeight += inputWeight;

            if (inputWeight > greatestWeight)
            {
                m_TrackBinding.text = input.text;
                greatestWeight = inputWeight;
            }

            if (!Mathf.Approximately(inputWeight, 0f))
            {
                currentInputs++;
            }
        }

        m_TrackBinding.color = blendedColor + m_DefaultColor * (1f - totalWeight);
        m_TrackBinding.fontSize = Mathf.RoundToInt(blendedFontSize + m_DefaultFontSize * (1f - totalWeight));
        if (currentInputs != 1 && 1f - totalWeight > greatestWeight)
        {
            var inputPlayable = (ScriptPlayable<TextSwitcherBehaviour>)playable.GetInput(currentInputs);
            TextSwitcherBehaviour input = inputPlayable.GetBehaviour();
            if (!input.keepTextOnFinish)
            {
                m_TrackBinding.text = m_DefaultText;
            }
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        m_FirstFrameHappened = false;

        if (m_TrackBinding == null)
        {
            return;
        }

        m_TrackBinding.color = m_DefaultColor;
        m_TrackBinding.fontSize = m_DefaultFontSize;
        m_TrackBinding.text = m_DefaultText;
    }
}