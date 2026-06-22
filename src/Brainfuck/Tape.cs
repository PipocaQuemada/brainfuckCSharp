namespace Brainfuck;

public record Tape(Seq<byte> Prev, byte Cur, Seq<byte> Next) {

  // A blank tape: the pointer sits on cell 0 with nothing written either side.
  // Cells outside the written region read as 0 (see MoveLeft/MoveRight), so the tape
  // only ever holds the cells actually visited -- it stays finite.
  public static Tape InitialTape =>
    new(Prev: Seq<byte>(), Cur: 0, Next: Seq<byte>());

  // At the edge of the written region there's no cell to step onto, so Head.IfNone(0)
  // supplies a fresh 0; .Tail of an empty Seq is just empty, so both moves are safe.
  public Tape MoveRight() =>
    new Tape( Prev: Cur.Cons(Prev), Cur: Next.Head.IfNone(0), Next: Next.Tail);

  public Tape MoveLeft() =>
    new Tape( Prev: Prev.Tail, Cur: Prev.Head.IfNone(0), Next: Cur.Cons(Next) );

  public Tape Set(byte i) =>
    this with { Cur = i };
}
