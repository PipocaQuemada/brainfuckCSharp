namespace Brainfuck;



public static class AppExtensions {
  extension<A>(K<App, A> ma) {
    public App<A> As() => (App<A>)ma;
  }
}

public partial class App : 
    Deriving.Monad<App, StateT<Tape, IO>>,
  Deriving.Stateful<App, StateT<Tape, IO>, Tape>,
    MonadIO<App> 
{
    public static K<StateT<Tape, IO>, A> Transform<A>(K<App, A> fa) =>
        fa.As().runApp;

    public static K<App, A> CoTransform<A>(K<StateT<Tape, IO>, A> fa) =>
        new App<A>(fa.As());   

    public static K<App, A> LiftIO<A>(IO<A> ma) =>
      new App<A>(StateT.liftIO<Tape, IO, A>(ma));

    public static K<App, A> liftIO<A>(IO<A> ma) =>
      new App<A>(StateT.liftIO<Tape, IO, A>(ma));

    public static App<A> gets<A>(Func<Tape, A> f) => new (StateT.gets<IO, Tape, A>(f));
    public static App<Unit> modify(Func<Tape, Tape> f) => new (StateT.modify<IO, Tape>(f));
}


public record App<A>(StateT<Tape, IO, A> runApp): K<App, A> {
}

public abstract record Instruction {
  public sealed record Incr: Instruction;
  public sealed record Decr: Instruction;
  public sealed record Left: Instruction;
  public sealed record Right: Instruction;
  public sealed record Read: Instruction;
  public sealed record Write: Instruction;
  public sealed record Loop(Seq<Instruction> Body): Instruction;


  static readonly Stream In  = Console.OpenStandardInput();
  static readonly Stream Out = Console.OpenStandardOutput();

  static IO<Unit> WriteByte(byte b) => IO.lift(() => { Out.WriteByte(b); Out.Flush(); });
  static IO<int>  ReadByte()       => IO.lift(() => In.ReadByte());

  public static App<Unit> Step(Instruction ast) {
    return ast switch {
      Incr => App.modify(m => m with { Cur = (byte)(m.Cur + 1)}),
      Decr => App.modify(m => m with { Cur = (byte)(m.Cur - 1)}),
      Left => App.modify(m => m.MoveLeft()),
      Right => App.modify(m => m.MoveRight()),
      Write => App.gets(m => m.Cur)
        .Bind((byte b) => App.liftIO<Unit>(WriteByte(b))).As(),
      Read => App.liftIO<int>(ReadByte())
        .Bind(b => App.modify(m => m with {Cur = (byte) b})).As(),
      Loop(var body) => 
        (from cur in App.gets<byte>(m => m.Cur)
        from _1 in when(cur != 0, from _2 in Eval(body)
                                  from curLoopEnd in App.gets<byte>(m => m.Cur)
                                  from _3 in when(curLoopEnd != 0, Step(ast))
                                  select unit)
        select unit).As(),
      _ => throw new ArgumentException("unexpected argument " + ast.ToString()) 
    };
  }

  public static App<Unit> Eval(Seq<Instruction> code) {
   return Traversable.traverse(c => Step(c), code).As().Select(_ => unit).As();
  }
 
}
