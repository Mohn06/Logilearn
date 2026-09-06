    using UnityEngine;
    using UnityEngine.Events;

    public class XnorGate : MonoBehaviour, ILogicInteract
    {
        [Header("Gate Visuals")]
        public Sprite Zero;
        public Sprite One;

        [Header("Output Indicator")]
        public LogicOutputDisplay outputDisplay;

        public UnityEvent<bool> onValueChange = new UnityEvent<bool>();

        private SpriteRenderer spriteRenderer;

        private bool inputA;
        private bool inputB;
        private bool inputASet;
        private bool inputBSet;

        public bool Output { get; private set; }

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetOutput(false);
        }

        public void SetInput(bool value, int inputIndex)
        {
            switch (inputIndex)
            {
                case 0:
                    inputA = value;
                    inputASet = true;
                    break;
                case 1:
                    inputB = value;
                    inputBSet = true;
                    break;
                default:
                    Debug.LogWarning("[XNOR] Invalid input index");
                    return;
            }

            Evaluate();
        }

        void Evaluate()
        {
            // If both inputs haven't been set, don't evaluate
            if (!inputASet || !inputBSet)
                return;

            // XNOR logic: true when inputs are the same
            bool result = !(inputA ^ inputB);
            SetOutput(result);
        }

        void SetOutput(bool value)
        {
            if (Output == value) return;

            Output = value;

            UpdateVisual(Output);

            if (outputDisplay != null)
                outputDisplay.SetValue(Output);

            onValueChange.Invoke(Output);

            Debug.Log($"[XNOR] A={(inputA ? 1 : 0)} B={(inputB ? 1 : 0)} → Output={(Output ? 1 : 0)}");
        }

        void UpdateVisual(bool on)
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = on ? One : Zero;
        }
    }
