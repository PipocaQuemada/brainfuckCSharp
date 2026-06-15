
public abstract record AST {
  public sealed record Incr : AST;
  public sealed record Decr: AST;
  public sealed record Left: AST;
  public sealed record Right: AST;
  public sealed record Read: AST;
  public sealed record Write: AST;
  public sealed record Loop(Seq<AST> Body): AST;


  static readonly Stream In  = Console.OpenStandardInput();
  static readonly Stream Out = Console.OpenStandardOutput();

  static IO<Unit> WriteByte(byte b) => IO.lift(() => { Out.WriteByte(b); Out.Flush(); });
  static IO<int>  ReadByte()       => IO.lift(() => In.ReadByte());

  public static StateT<Tape, IO, Unit> Step(AST ast) {
    return ast switch {
      Incr => StateT.modify<IO, Tape>(m => m with { Cur = (byte)(m.Cur + 1)}),
      Decr => StateT.modify<IO, Tape>(m => m with { Cur = (byte)(m.Cur - 1)}),
      Left => StateT.modify<IO, Tape>(m => m.MoveLeft()),
      Right => StateT.modify<IO, Tape>(m => m.MoveRight()),
      Write => StateT.gets<IO, Tape, byte>(m => m.Cur)
        .Bind((byte b) => StateT.liftIO<Tape, IO, Unit>(WriteByte(b))),
      Read => StateT.liftIO<Tape, IO, int>(ReadByte())
        .Bind(b => new Modify<Tape>(m => m with {Cur = (byte) b})),
      Loop(var body) => 
        from cur in StateT.gets<IO, Tape, byte>(m => m.Cur)
        from _1 in when(cur != 0, from _2 in Eval(body)
                                  from curLoopEnd in StateT.gets<IO, Tape, byte>(m => m.Cur)
                                  from _3 in when(curLoopEnd != 0, Step(ast))
                                  select unit)
        select unit,
      _ => throw new ArgumentException("unexpected argument " + ast.ToString()) 
    };
  }

  public static StateT<Tape, IO, Unit> Eval(Seq<AST> code) {
   return Traversable.traverse(c => Step(c), code).As().Select(_ => unit);
  }
 
}
