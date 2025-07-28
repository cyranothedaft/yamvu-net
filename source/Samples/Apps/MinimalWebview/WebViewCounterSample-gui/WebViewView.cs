using System;
using yamvu.Extensions.WebView;



namespace MinimalWebViewCounterSample;

public record WebViewView(
      string Html
) : IWebViewView;
