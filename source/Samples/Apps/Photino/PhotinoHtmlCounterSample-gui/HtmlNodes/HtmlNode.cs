using System;
using System.Linq;



namespace HtmlNodes;

public abstract record HtmlNode(params HtmlNode[] Contents) {
   public abstract string Render();

   public static implicit operator HtmlNode(string convertFrom)
      => new TextNode(convertFrom);
}


public record HtmlTag(
      HtmlNode[] Contents,
      string TagName,
      bool CanSelfClose = true
) : HtmlNode(Contents) {
   public override string Render()
      => CanSelfClose && Contents.Length == 0
               ? $"<{TagName} />"
               : $"<{TagName}>{string.Concat(Contents.Select(n => n.Render()))}</{TagName}>";
}



public record TextNode(string Text) : HtmlNode() {
   public override string Render() => Text;
}
