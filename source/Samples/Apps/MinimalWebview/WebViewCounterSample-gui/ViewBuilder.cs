using System;
using CounterSample.AppCore.Mvu;
using Microsoft.Extensions.Logging;
using yamvu.core;
using yamvu.MOVE_THESE_TO_LIB.HtmlNodes;



namespace MinimalWebViewCounterSample;


internal static class ViewBuilder {
   public static WebViewView BuildView(MvuMessageDispatchDelegate dispatch, Model model, ILogger? uilogger)
      => new WebViewView(buildHtml(model));


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
               Button("Increment (1)", @class("primary center"), onclick("sendMessage('msg:increment1')")),
               Button("Increment (Random)", @class("primary center"), onclick("sendMessage('msg:incrementrandom')")),

               // TODO:
               // <br /><button id="increment1Button" class="bg-indigo-600 hover:bg-indigo-800 text-white font-bold py-1 px-4 rounded-md self-center">Increment 1</button>
               // <br /><button id="incrementRandomButton" class="bg-indigo-600 hover:bg-indigo-800 text-white font-bold py-1 px-4 rounded-md self-center">Increment Random</button>

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
