using System;

namespace ConsoleSample.View;

public record PlatformView<TView>(
      TView MvuView,
      ViewInputBindings InputBindings
      ) {

}
