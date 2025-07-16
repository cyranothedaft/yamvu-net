using System;
using System.Collections.Generic;
using System.Linq;


namespace yamvu.ViewGeneration.HtmlNodes;

public abstract record HtmlNode(params IReadOnlyList<HtmlNode> Children) {
   public abstract string Render();

   public static implicit operator HtmlNode(string convertFrom)
      => new TextNode(convertFrom);
}


public static class HtmlNodeExtensions {
   public static string Render(this IEnumerable<HtmlNode> nodes)
      => string.Concat(nodes.Select(n => n.Render()));
}
