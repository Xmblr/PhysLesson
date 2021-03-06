using UnityEngine;

namespace TexDrawLib
{
    public class AttrLinkBox : Box
    {
        public string metaKey;
        public Box baseBox;

        public static AttrLinkBox Get(Box BaseBox, string MetaKey)
        {
            var box = ObjPool<AttrLinkBox>.Get();
            box.metaKey = MetaKey;
            box.baseBox = BaseBox;

            box.Set(BaseBox.width, BaseBox.height, BaseBox.depth, 0);

            return box;
        }

        public override void Draw(DrawingContext drawingContext, float scale, float x, float y)
        {
            base.Draw(drawingContext, scale, x, y);
            float padding = TEXConfiguration.main.LinkMargin;

            var tint = drawingContext.DrawLink(
                new Rect((x - padding / 2f) * scale, (y - depth - padding / 2f) * scale,
                (width + padding) * scale, (totalHeight + padding) * scale), metaKey);

            TexContext.Color.Push(TexUtility.MultiplyColor(TexContext.Color.value, tint));
            baseBox.Draw(drawingContext, scale, x, y);
            TexContext.Color.Pop();

        }

        public override void Flush()
        {
            if (baseBox != null)
            {
                baseBox.Flush();
                baseBox = null;
            }
            ObjPool<AttrLinkBox>.Release(this);
        }
    }
}
