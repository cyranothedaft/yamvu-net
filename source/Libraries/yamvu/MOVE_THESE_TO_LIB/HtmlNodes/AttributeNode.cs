using System.Net;



namespace yamvu.MOVE_THESE_TO_LIB.HtmlNodes;


public abstract record AttributeNode(string Name) : HtmlNode();


public record AttributeValueNode(string Name, string Value) : AttributeNode(Name) {
   public override string Render() => string.Format(" {0}=\"{1}\"",
                                                    Name, WebUtility.HtmlEncode(Value));
}


public record BooleanAttributeNode(string Name) : AttributeNode(Name) {
   public override string Render() => $" {Name}";
}
