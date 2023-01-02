using UnityEngine;

namespace Game.Scripts {
    public class TestMesh : MonoBehaviour{
        
        [ContextMenu("do stuff")]
        private void DoStuff() {
            Vector3[] verts = new Vector3[4];
            int[] tris = new int[6];

            verts[0] = new Vector3(0, 0);
            verts[1] = new Vector3(0, 0, 1);
            verts[2] = new Vector3(1f, 0, 1);
            verts[3] = new Vector3(1f, 0, 0);

            tris[0] = 0;
            tris[1] = 1;
            tris[2] = 2;

            tris[3] = 2;
            tris[4] = 3;
            tris[5] = 0;
            
            Mesh mesh = new Mesh {
                vertices = verts,
                triangles = tris,
                name = "Meshy"
            };

            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}