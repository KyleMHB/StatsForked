using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public class ColumnDef : Def
{
    public string? labelKey;
    public string? descriptionKey;
    public string Description => description;
    public ColumnTitleXmlNode? title;
    public Widget Title => title?.ToWidget() ?? new Label(LabelCap);
#pragma warning disable CS8618
    public Type workerClass;
    public IColumnWorker Worker { get; private set; }
#pragma warning restore CS8618
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        if (labelKey?.Length > 0 && string.IsNullOrEmpty(label))
        {
            label = labelKey.Translate();
        }

        if (descriptionKey?.Length > 0 && string.IsNullOrEmpty(description))
        {
            description = descriptionKey.Translate();
        }

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            Worker = (IColumnWorker)Activator.CreateInstance(workerClass, this);
        });
    }
}

public sealed class ColumnTitleXmlNode
{
    private readonly List<Element> elements = [];
    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        foreach (var node in xmlRoot.ChildNodes)
        {
            if (node is XmlText textNode)
            {
                var text = textNode.InnerText.Trim();

                if (text.Length > 0)
                {
                    var element = new TextElement(text);

                    elements.Add(element);
                }
            }
            else if (node is XmlElement elementNode)
            {
                var element = new IconElement(elementNode);

                elements.Add(element);
            }
        }
    }
    public Widget ToWidget()
    {
        if (elements.Count == 1)
        {
            return elements[0].ToWidget();
        }

        return new HorizontalContainer(elements.Select(elem => elem.ToWidget()).ToList(), 2f);
    }

    private abstract class Element
    {
        public abstract Widget ToWidget();
    }

    private sealed class TextElement : Element
    {
        private readonly string Text;
        public TextElement(string text)
        {
            Text = text;
        }
        public override Widget ToWidget()
        {
            return new Label(Text);
        }
    }

    private sealed class IconElement : Element
    {
        public IconDef Def;
#pragma warning disable CS8618
        public IconElement(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, nameof(Def), xmlRoot.Name);
        }
#pragma warning restore CS8618
        public override Widget ToWidget()
        {
            Widget widget = new InlineTexture(Def.Texture, Def.scale);

            if (Def.color != null)
            {
                widget = widget.Color(Def.color.Value);
            }

            return widget;
        }
    }
}
