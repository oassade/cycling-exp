using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Splines
{
    [AddComponentMenu("Splines/Spline Animate")]
    [ExecuteInEditMode]
    public class SplineAnimate : SplineComponent
    {
        public enum Method { Time, Speed }
        public enum LoopMode { Once, Loop, LoopEaseInOnce, PingPong }
        public enum EasingMode { None, EaseIn, EaseOut, EaseInOut }
        public enum AlignmentMode { None, SplineElement, SplineObject, World }

        [SerializeField] SplineContainer m_Target;
        [SerializeField] bool m_PlayOnAwake = true;
        [SerializeField] LoopMode m_LoopMode = LoopMode.Loop;
        [SerializeField] Method m_Method = Method.Time;
        [SerializeField] float m_Duration = 1f;
        [SerializeField] float m_MaxSpeed = 10f;
        [SerializeField] EasingMode m_EasingMode = EasingMode.None;
        [SerializeField] AlignmentMode m_AlignmentMode = AlignmentMode.SplineElement;
        [SerializeField] AlignAxis m_ObjectForwardAxis = AlignAxis.ZAxis;
        [SerializeField] AlignAxis m_ObjectUpAxis = AlignAxis.YAxis;
        [SerializeField] float m_StartOffset;
        [SerializeField, Tooltip("Key to move forward along the spline.")] KeyCode m_ForwardKey = KeyCode.W;
        [SerializeField, Tooltip("Speed for manual control.")] float m_ManualSpeed = 5f;

        [NonSerialized] float m_StartOffsetT;
        float m_NormalizedTime;
        float m_SplineLength = -1;
        SplinePath<Spline> m_SplinePath;

        void Update()
        {
            if (m_Target == null || m_SplinePath == null) return;

            if (Input.GetKey(m_ForwardKey))
            {
                m_NormalizedTime += m_ManualSpeed * Time.deltaTime / m_SplinePath.GetLength();
                m_NormalizedTime = Mathf.Clamp01(m_NormalizedTime);
            }

            var position = m_SplinePath.EvaluatePosition(m_NormalizedTime);
            var tangent = m_SplinePath.EvaluateTangent(m_NormalizedTime);

            if (Physics.Raycast((Vector3)position + Vector3.up, Vector3.down, out RaycastHit hit))
            {
                transform.position = hit.point;
                transform.rotation = Quaternion.LookRotation(tangent, hit.normal);
            }
            else
            {
                transform.position = (Vector3)position;
                if (m_AlignmentMode != AlignmentMode.None)
                {
                    var up = m_AlignmentMode == AlignmentMode.SplineElement
                        ? (Vector3)m_SplinePath.EvaluateUpVector(m_NormalizedTime)
                        : (m_AlignmentMode == AlignmentMode.SplineObject ? transform.up : Vector3.up);
                    transform.rotation = Quaternion.LookRotation((Vector3)tangent, up);
                }
            }
        }

        void OnEnable()
        {
            if (m_Target != null)
                m_SplinePath = new SplinePath<Spline>(m_Target.Splines);
        }
    }
}
