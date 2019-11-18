using System;
using System.IO;

namespace ContentRuntimeLoader
{
    public static class ContentRuntime
    {
        public static string BaseDirectory => Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        
        public static string ContentDirectory => Path.Combine(BaseDirectory, "Content");
        
        public static string TetrisTheme => Path.Combine(ContentDirectory, "TetrisTheme.ogg");
        public static string BlockTexture => Path.Combine(ContentDirectory, "block.png");
        public static string Shader => Path.Combine(ContentDirectory, "Shader.fx");
    }
}