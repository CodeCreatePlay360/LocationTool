using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CodeCreatePlay
{
    namespace Utils
    {
        [System.Serializable]
        public class Triangle
        {
            public Vector3 x = new();
            public Vector3 y = new();
            public Vector3 z = new();

            public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                x = v1;
                y = v2;
                z = v3;
            }

            public Vector3 GetCentre()
            {
                // direct method
                float x = (this.x.x + this.y.x + this.z.x) / 3;
                float y = (this.x.y + this.y.y + this.z.y) / 3;
                float z = (this.x.z + this.y.z + this.z.z) / 3;

                Vector3 centrePos = new Vector3(x, y, z);

                return centrePos;
            }
        }
    }
}
