namespace UnityEngine.EventSystems
{
    public static class GeometryUtils
    {
        public static Mesh CreateQuad(float sizeX, float sizeY,bool centered=true)
        {
            Mesh quad =new Mesh();
            if (centered)
            {
                quad.vertices = new Vector3[4]
                {
                    new Vector3(-sizeX/2,-sizeY/2, 0),
                    new Vector3(sizeX/2, -sizeY/2, 0),
                    new Vector3(-sizeX/2, sizeY/2, 0),
                    new Vector3(sizeX/2, sizeY/2, 0)
                };
            }
            else
            {
                quad.vertices = new Vector3[4]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(sizeX, 0, 0),
                    new Vector3(0, sizeY, 0),
                    new Vector3(sizeX, sizeY, 0)
                };
            }

            quad.uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            quad.SetIndices(new int[] {0, 2, 1, 2, 3, 1}, MeshTopology.Triangles, 0);
            return quad;
        }
    }
}