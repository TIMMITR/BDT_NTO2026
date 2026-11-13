using UnityEngine;

namespace Temp
{
    public class DrawBoard : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            const int radius = 5;
            const float hexRadius = 0.25f;

            for (int q = -radius; q <= radius; q++)
            {
                int rMin = Mathf.Max(-radius, -q - radius);
                int rMax = Mathf.Min(radius, -q + radius);

                for (int r = rMin; r <= rMax; r++)
                {
                    var center = transform.position + new Vector3(
                        hexRadius * 1.5f * q,
                        0.1f,
                        hexRadius * 1.7f * (r + q * 0.5f)
                    );

                    DrawHexWire(center, hexRadius);
                }
            }
        }

        private static void DrawHexWire(Vector3 center, float radius)
        {
            for (int i = 0; i < 6; i++)
            {
                float angleA = i * 60f * Mathf.Deg2Rad;
                float angleB = (i + 1) * 60f * Mathf.Deg2Rad;

                var pA = center + new Vector3(Mathf.Cos(angleA) * radius, 0, Mathf.Sin(angleA) * radius);
                var pB = center + new Vector3(Mathf.Cos(angleB) * radius, 0, Mathf.Sin(angleB) * radius);

                Gizmos.DrawLine(pA, pB);
            }
        }
    }
}
