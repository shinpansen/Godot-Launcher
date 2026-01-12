using Godot;

namespace GodotLauncher.Scripts.Extensions;

public static class TextureRectExtensions
{
    public static void SetTextureFromPath(this TextureRect rect, string path)
    {
        var image = new Image();
        var err = image.Load(path);

        if (err != Error.Ok)
        {
            GD.PushError($"Failed to load image: {path}");
            return;
        }

        var texture = ImageTexture.CreateFromImage(image);
        rect.Texture = texture;
    }
}
