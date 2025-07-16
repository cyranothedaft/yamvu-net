using System.Collections.Generic;
using System.Linq;
using System.Net;


namespace yamvu.ViewGeneration.HtmlNodes;


public abstract record ContentNode(params IReadOnlyList<HtmlNode> Children) : HtmlNode(Children);


public record HtmlTag(
      IReadOnlyList<HtmlNode> Children,
      string TagName,
      bool CanSelfClose = true) : ContentNode(Children) {

   public override string Render()
      => CanSelfClose && Children.Count == 0
               ? string.Format("<{0} />",           TagName)
               : string.Format("<{0}{1}>{2}</{3}>", TagName,
                                                    Children.OfType<AttributeNode>().Render(),
                                                    Children.OfType<ContentNode>().Render(),
                                                    TagName);
}


public record TextNode(string Text) : ContentNode() {
   public override string Render() => WebUtility.HtmlEncode(Text);
}
