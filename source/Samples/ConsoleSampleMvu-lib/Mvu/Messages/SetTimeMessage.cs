// using System;
// using WelterKit.Std.StaticUtilities;
// using yamvu.core.Primitives;
//
//
// namespace ConsoleSampleMvu.Mvu.Messages;
//
// public record SetTimeMessage(DateTimeOffset Time) : Message {
//    public override string ToString() => $"SetTime({Time:O})".SurroundWith('$');
//
//
//
//    public static (Model, IMvuCommand[]) Handle(Model currentModel, DateTimeOffset time)
//       => (currentModel with
//              {
//                 Time = time,
//              },
//              []
//          );
// }
//
//
// public static partial class MvuMessages {
//    public static Message SetTime(DateTimeOffset time) => new SetTimeMessage(time);
// }
