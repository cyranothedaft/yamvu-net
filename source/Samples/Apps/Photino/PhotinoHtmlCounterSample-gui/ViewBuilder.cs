using System;
using CounterMvu_lib;
using HtmlNodes;
using Microsoft.Extensions.Logging;
using yamvu.core;



namespace PhotinoHtmlCounterSample.gui;


internal static class ViewBuilder {
   public static PhotinoView BuildView(MvuMessageDispatchDelegate dispatch, Model model, ILogger? uilogger)
      => new PhotinoView(buildHtml(model));


   // private const string ViewScript = """
   //                                   function increment1() {
   //                                       ;
   //                                   }
   //                                   function incrementRandom() {
   //                                       ;
   //                                   }
   //                                   """;

   private static string buildHtml(Model model)
      => new[]
            {
               // Script(ViewScript),
               P($"Counter: ", Span(model.Counter.ToString())),
               Button("Increment (1)", @class("primary center"), onclick("window.external.sendMessage('msg:increment1')")),
               Button("Increment (Random)", @class("primary center"), onclick("window.external.sendMessage('msg:incrementrandom')")),
            }
           .Render();

   // like this:  https://github.com/codechem/CC.CSX?tab=readme-ov-file

   private static HtmlNode Button(params HtmlNode[] contents) => new HtmlTag(contents, "button", CanSelfClose: false);
   private static HtmlNode P     (params HtmlNode[] contents) => new HtmlTag(contents, "p"     , CanSelfClose: false);
   private static HtmlNode Script(params HtmlNode[] contents) => new HtmlTag(contents, "script", CanSelfClose: false);
   private static HtmlNode Span  (params HtmlNode[] contents) => new HtmlTag(contents, "span"  , CanSelfClose: false);

   private static AttributeNode @class(string value) => new AttributeNode("class", value);
   private static AttributeNode onclick(string value) => new AttributeNode("onclick", value);

   // private static ScriptNode Script() => new ScriptNode();
   // private static      PNode P()      => new PNode();
   // private static ButtonNode Button() => new ButtonNode();
   // private record ScriptNode() : HtmlNode("script", CanSelfClose: false);
   // private record      PNode() : HtmlNode("p"     , CanSelfClose: false);
   // private record ButtonNode() : HtmlNode("button", CanSelfClose: false);
}
