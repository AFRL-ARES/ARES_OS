using System.Windows;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace ARESCore.UI.Converters
{
  public class IconConverter
  {
    public static ImageSource Convert( PackIconMaterialKind kind )
    {
      var material = new PackIconMaterial()
      {
        Kind = kind,
        VerticalAlignment = VerticalAlignment.Center
      };
      var brush = new SolidColorBrush( Colors.White )
        ;      var geometryDrawing = new GeometryDrawing
      {
        Geometry = Geometry.Parse( material.Data ),
        Brush = brush,
        Pen = new Pen( brush, 0.1 )
      };

      var drawingGroup = new DrawingGroup { Children = { geometryDrawing } };

      var img = new DrawingImage { Drawing = drawingGroup };

      return img;
    }
  }
}
