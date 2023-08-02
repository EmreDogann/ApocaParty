using System;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelinePlayables.TextPopup
{
    [Serializable]
    public class TextPopupMixerBehaviour : PlayableBehaviour
    {
        private TextMeshProUGUI _textUI;
        private Vector3 _startingScale;
        private Vector3 _startingPosition;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int inputCount = playable.GetInputCount();

            // If the text binding has been changed.
            if (_textUI != playerData as TextMeshProUGUI)
            {
                _textUI = (TextMeshProUGUI)playerData;
                _startingScale = _textUI.transform.localScale;
                _startingPosition = _textUI.transform.position;

                for (int i = 0; i < inputCount; i++)
                {
                    var inputPlayable = (ScriptPlayable<TextPopupBehaviour>)playable.GetInput(i);
                    TextPopupBehaviour input = inputPlayable.GetBehaviour();

                    input.textUI = _textUI;
                    input.startingScale = _startingScale;
                }
            }

            if (!_textUI)
            {
                return;
            }

            string currentText = "";
            Vector3 currentPosition = _startingPosition;

            for (int i = 0; i < inputCount; i++)
            {
                var inputPlayable = (ScriptPlayable<TextPopupBehaviour>)playable.GetInput(i);
                TextPopupBehaviour input = inputPlayable.GetBehaviour();

                if (input.IsShown())
                {
                    currentText = input.text;
                    currentPosition = input.position;
                }
            }

            _textUI.text = currentText;
            _textUI.transform.position = currentPosition;
        }
    }
}