using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterMvu_lib;
using HtmlNodes;
using yamvu.core;



namespace PhotinoHtmlCounterSample.gui;


internal static class ViewBuilder {
   public static PhotinoView BuildView(MvuMessageDispatchDelegate dispatch, Model model)
      => new PhotinoView(buildHtml(model));
                             
                             
                             
//                             $"""
//                              <script>
//                                  var mvudiv = document.getElementById('mvudiv');
//
//                                  function increment1() {
//                                      window.external.sendMessage('mvu:StartProgram');
//                                  }
//
//                                  // window.external.receiveMessage(message => alert(message));
//
//                                  window.external.receiveMessage(message => {
//                                      mvudiv.innerHTML = message;
//                                  });
//                              </script>
//                              <p>Counter: {model.Counter}</p>
//                              <button class="primary center" onclick="increment1()">Increment (1)</button>
//                              """);// TODO
   //


   private static string buildHtml(Model model)
      => string.Concat(Script().Render(),
                       P($"Counter: ", Span(model.Counter.ToString())).Render(),
                       Button().Render()
                      );

   // like this:  https://github.com/codechem/CC.CSX?tab=readme-ov-file

   private static HtmlNode Button(params HtmlNode[] contents) => new HtmlTag(contents, "button", CanSelfClose: false);
   private static HtmlNode P     (params HtmlNode[] contents) => new HtmlTag(contents, "p"     , CanSelfClose: false);
   private static HtmlNode Script(params HtmlNode[] contents) => new HtmlTag(contents, "script", CanSelfClose: false);
   private static HtmlNode Span  (params HtmlNode[] contents) => new HtmlTag(contents, "span"  , CanSelfClose: false);

   // private static ScriptNode Script() => new ScriptNode();
   // private static      PNode P()      => new PNode();
   // private static ButtonNode Button() => new ButtonNode();
   // private record ScriptNode() : HtmlNode("script", CanSelfClose: false);
   // private record      PNode() : HtmlNode("p"     , CanSelfClose: false);
   // private record ButtonNode() : HtmlNode("button", CanSelfClose: false);
}
