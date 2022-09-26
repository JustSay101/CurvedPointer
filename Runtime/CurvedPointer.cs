using UnityEngine;
using Unity.Mathematics;

namespace CustomPointers
{
     public class CurvedPointer : MonoBehaviour
    {
        [SerializeField] private Sprite pointerHead;
        [SerializeField] private Sprite pointerDot;
        [SerializeField] private Sprite pointerRoot;
        
        [Space(10)]
        
        [SerializeField] [Min(1)] private int resolution;
        [SerializeField] private bool offsetPointerHead;
        [SerializeField] private bool showRoot;
        
        private (Transform transform, SpriteRenderer renderer, float point, float pointSqrt, float pointPow)[] pointerPoints;
        
        private void Start()
        {
            if (resolution < 3 && offsetPointerHead)
                offsetPointerHead = false;
            
            pointerPoints =
                new (Transform transform, SpriteRenderer renderer, float point, float pointSqrt, float pointPow)
                [offsetPointerHead ? resolution : resolution + 1];

            float step = 1f / resolution;
            float currentPoint = 0;

            for (var i = 0; i < resolution; i++)
            {
                AddPointToPointer(i, currentPoint);
                currentPoint += step;
            }

            if (!offsetPointerHead)
                AddPointToPointer(resolution, 1);
            
            TogglePointer();
        }
        
        /// <summary>
        /// Toggles pointer SpriteRenderers enabled/disabled depending on current state.
        /// </summary>
        public void TogglePointer()
        {
            for (var i = 0; i < pointerPoints.Length; i++)
            {
                pointerPoints[i].renderer.enabled = !pointerPoints[i].renderer.enabled;
            }
        }
        
        /// <summary>
        /// Updates the positions of the pointer points.
        /// </summary>
        /// <param name="from">Coordinates to start the pointer from.</param>
        /// <param name="to">Coordinates to end the pointer at.</param>
        public void UpdateCursorPosition(Vector2 from, Vector2 to)
        {
            for (var i = 0; i < pointerPoints.Length; i++)
            {
                pointerPoints[i].transform.position = CalculatePosition(pointerPoints[i], from, to);
                
                if (i < pointerPoints.Length - 1)
                {
                    pointerPoints[i].transform.up = CalculatePosition(pointerPoints[i + 1], from, to) - (Vector2)pointerPoints[i].transform.position;
                    continue;
                }
                
                pointerPoints[i].transform.up = (Vector2)pointerPoints[i].transform.position - CalculatePosition(pointerPoints[i - 1], from, to);
            }
        }
        
        private void AddPointToPointer(int i, float point)
        {
            var cursorPoint = new GameObject(
                i == 0 && pointerRoot != null ? "CursorRoot" :
                offsetPointerHead ? i == resolution - 1 ? "CursorHead" : "CursorDot" :
                i == resolution ? "CursorHead" : "CursorDot")
            {
                transform = { position = this.transform.position, parent = this.transform }
            };
            
            var cursorRenderer = cursorPoint.AddComponent<SpriteRenderer>();
            cursorRenderer.sortingOrder = 100 + i;
            
            cursorRenderer.sprite = i == 0 ? showRoot ? pointerRoot : null :
                offsetPointerHead ? i == resolution - 1 ? pointerHead : pointerDot :
                i == resolution ? pointerHead : pointerDot;
            
            pointerPoints[i].transform = cursorPoint.transform;
            pointerPoints[i].renderer = cursorRenderer;
            pointerPoints[i].point = point;
            pointerPoints[i].pointSqrt = math.sqrt(point);
            pointerPoints[i].pointPow = math.pow(point, 2);
        }
        
        private static Vector2 CalculatePosition(
            (Transform transform, SpriteRenderer renderer, float point, float pointSqrt, float pointPow) cursorPoint,
            Vector2 from, Vector2 to)
        {
            var x = cursorPoint.pointPow * (to.x - from.x) * cursorPoint.pointSqrt + from.x;
            var y = (to.y - from.y) * cursorPoint.point + from.y;
            
            return new Vector2(x, y);
        }
    }
}