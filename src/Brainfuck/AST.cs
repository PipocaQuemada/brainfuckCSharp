
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

  static IO<Unit> WriteByte(int b) => IO.lift(() => { Out.WriteByte((byte) b); Out.Flush(); });
  static IO<int>  ReadByte()       => IO.lift(() => In.ReadByte());

  public static StateT<Mem, IO, Unit> Step(AST ast) {
    return ast switch {
      Incr => StateT.modify<IO, Mem>(m => m with { Cur = m.Cur + 1}),
      Decr => StateT.modify<IO, Mem>(m => m with { Cur = m.Cur - 1}),
      Left => StateT.modify<IO, Mem>(m => m.MoveLeft()),
      Right => StateT.modify<IO, Mem>(m => m.MoveRight()),
      Write => StateT.gets<IO, Mem, int>(m => m.Cur)
        .Bind((int b) => StateT.liftIO<Mem, IO, Unit>(WriteByte(b))),
      Read => StateT.liftIO<Mem, IO, int>(ReadByte())
        .Bind(b => new Modify<Mem>(m => m with {Cur = b})),
      Loop(var body) => 
        from cur in StateT.gets<IO, Mem, int>(m => m.Cur)
        from _1 in when(cur != 0, from _2 in Eval(body)
                                  from curLoopEnd in StateT.gets<IO, Mem, int>(m => m.Cur)
                                  from _3 in when(curLoopEnd != 0, Step(ast))
                                  select unit)
        select unit,
      _ => throw new ArgumentException("unexpected argument " + ast.ToString()) 
    };
  }

  public static StateT<Mem, IO, Unit> Eval(Seq<AST> code) {
   return Traversable.traverse(c => Step(c), code).As().Select(_ => unit);
  }
 
}
