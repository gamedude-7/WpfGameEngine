using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.Classes
{
    public class Face
    {
        //[ElementProperty(0)]
        public int VertexCount { get; set; }

        //[ElementProperty(1, ListLengthProperty = "VertexCount")]        
        public uint[] VertexIndices { get; set; }

        public Face()
        {
            VertexCount = 0;
        }
    }
}
