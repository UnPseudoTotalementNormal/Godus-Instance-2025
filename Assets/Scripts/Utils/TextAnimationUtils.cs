using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils.TimerSystem;

namespace Utils
{
    public class TextAnimationUtils
    {
        public static void ChangeText(TMP_Text textComponent, string newText, float duration, System.Action onComplete = null)
        {
            string currentText = textComponent.text;

            if (currentText == newText)
            {
                onComplete?.Invoke();
                return;
            }

            int commonPrefixLength = 0;
            int minLength = Mathf.Min(currentText.Length, newText.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (currentText[i] != newText[i])
                {
                    break;
                }

                commonPrefixLength++;
            }

            int charsToDelete = currentText.Length - commonPrefixLength;

            if (charsToDelete > 0)
            {
                float deleteDuration = duration * 0.5f;

                DOVirtual.Float(currentText.Length, commonPrefixLength, deleteDuration, (value) =>
                {
                    int charCount = Mathf.FloorToInt(value);
                    textComponent.text = currentText.Substring(0, charCount);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    float addDuration = duration * 0.5f;

                    DOVirtual.Float(commonPrefixLength, newText.Length, addDuration, (value) =>
                    {
                        int charCount = Mathf.FloorToInt(value);
                        textComponent.text = newText.Substring(0, charCount);
                    })
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        textComponent.text = newText;
                        onComplete?.Invoke();
                    });
                });

                return;
            }

            float addDuration = duration;

            DOVirtual.Float(commonPrefixLength, newText.Length, addDuration, (value) =>
            {
                int charCount = Mathf.FloorToInt(value);
                textComponent.text = newText.Substring(0, charCount);
            })
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                textComponent.text = newText;
                onComplete?.Invoke();
            });
        }

        public static TimerSystem.TimerSystem StartTimerText(TMP_Text _waveText, TimerSystem.TimerSystem _timer, string _label = "Timer", string _format = "0.00", System.Action _onComplete = null)
        {
            ChangeText(_waveText, $"{_label}: {_timer.GetFormattedTime(_format)}", 1f, () =>
                _timer.onTimerTick += () =>
                {
                    _waveText.text = $"{_label}: {_timer.GetFormattedTime(_format)}";
                });
            return _timer;
        }
    }
}