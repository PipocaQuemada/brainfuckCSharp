

class TapeUtils {
  public static IEnumerable<byte> Zeros() {
    while(true) {
      yield return 0;
    }
  }
}

public record Tape(Seq<byte> Prev, byte Cur, Seq<byte> Next) {

  // Create an infinite tape of all zeros.  N.B. - this will spin forever if we use anything that forces the whole seq.
  public static Tape InitialTape => 
    new( Next: toSeq(TapeUtils.Zeros()),
         Prev: toSeq(TapeUtils.Zeros()),
         Cur: 0);


  // Todo: what does .Tail do on an empty list?  Can we use this with empty starting lists for Prev and Next?
  public Tape MoveRight() =>
    new Tape( Prev: Cur.Cons(Prev), Cur: Next.Head.IfNone(0), Next: Next.Tail);

  public Tape MoveLeft() =>
    new Tape( Prev: Prev.Tail, Cur: Prev.Head.IfNone(0), Next: Cur.Cons(Next) );

  public Tape Set(byte i) =>
    this with { Cur = i };
}
