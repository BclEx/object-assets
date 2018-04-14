﻿using UnityEngine;

namespace OA.Tes.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        Canvas _canvas = null;
        Transform _hudTransform = null;
        Transform _uiTransform = null;

        [Header("HUD Elements")]
        [SerializeField]
        UICrosshair _crosshair = null;
        [SerializeField]
        UIInteractiveText _interactiveText = null;

        [Header("UI Elements")]
        [SerializeField]
        UIBook _book = null;
        [SerializeField]
        UIScroll _scroll = null;

        #region Public Fields

        public UIBook Book
        {
            get { return _book; }
        }

        public UIInteractiveText InteractiveText
        {
            get { return _interactiveText; }
        }

        public UIScroll Scroll
        {
            get { return _scroll; }
        }

        public UICrosshair Crosshair
        {
            get { return _crosshair; }
        }

        public Transform HUD
        {
            get { return _hudTransform; }
        }

        public Transform UI
        {
            get { return _uiTransform; }
        }

        public bool Visible
        {
            get { return _canvas.enabled; }
            set { _canvas.enabled = value; }
        }

        public bool Active
        {
            get { return _hudTransform.gameObject.activeSelf; }
            set
            {
                _hudTransform.gameObject.SetActive(value);
                _uiTransform.gameObject.SetActive(value);
            }
        }

        #endregion

        void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _hudTransform = transform.Find("HUD");
            _uiTransform = transform.Find("UI");
        }
    }
}
