using System;
using System.Collections.Generic;
using System.Linq;



namespace HtmlNodes;

public abstract record HtmlNode(params HtmlNode[] Children) {
   public abstract string Render();

   public static implicit operator HtmlNode(string convertFrom)
      => new TextNode(convertFrom);
}


public record AttributeNode(string Name, string Value) : HtmlNode() {
   public override string Render() => $"{Name}=\"{Value}\"";
}



public abstract record ContentNode(params HtmlNode[] Children) : HtmlNode(Children);


public record HtmlTag(
      HtmlNode[] Children,
      string TagName,
      bool CanSelfClose = true
) : ContentNode(Children) {
   public override string Render()
      => CanSelfClose && Children.Length == 0
               ? string.Format("<{0} />",            TagName)
               : string.Format("<{0} {1}>{2}</{3}>", TagName,
                                                     renderMultiple(Children.OfType<AttributeNode>()),
                                                     renderMultiple(Children.OfType<ContentNode>()),
                                                     TagName);


   private static string renderMultiple(IEnumerable<HtmlNode> nodes)
      => string.Concat(nodes.Select(a => a.Render()));
}


public record TextNode(string Text) : ContentNode() {
   public override string Render() => Text;
}
