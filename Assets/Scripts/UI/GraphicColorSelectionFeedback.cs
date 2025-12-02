using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GraphicColorSelectionFeedback : MonoBehaviour, ISelectionFeedback
    {
        public List<GraphicFeedbackData> graphicsData = new();
        
        public void ShowSelectionFeedback()
        {
            foreach (var _data in graphicsData)
            {
                CheckForOriginalColor(_data);
                _data.graphic.DOColor(_data.feedbackColor, _data.fadeColorDuration).SetEase(_data.fadeColorEase);
            }
        }

        public void HideSelectionFeedback()
        {
            foreach (var _data in graphicsData)
            {
                CheckForOriginalColor(_data);
                _data.graphic.DOColor(_data.originalColor, _data.fadeColorDuration).SetEase(_data.fadeColorEase);
            }
        }

        public void CheckForOriginalColor(GraphicFeedbackData _data)
        {
            if (_data.setOriginalColorAsFirstValue && !_data.hasSetOriginalColor)
            {
                _data.originalColor = _data.graphic.color;
                _data.hasSetOriginalColor = true;
            }
        }
        
        [Serializable]
        public class GraphicFeedbackData
        {
            public Graphic graphic;
            public Color originalColor;
            public Color feedbackColor;
            public bool setOriginalColorAsFirstValue;
            [HideInInspector] public bool hasSetOriginalColor;

            public float fadeColorDuration = 0.5f;
            public Ease fadeColorEase = Ease.OutQuint;
        }
    }
    
    
}