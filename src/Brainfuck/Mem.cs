

class MemUtils {
  public static IEnumerable<byte> Zeros() {
    while(true) {
      yield return 0;
    }
  }
}

public record Mem(Seq<byte> Prev, byte Cur, Seq<byte> Next) {

  // Create an infinite tape of all zeros.  N.B. - this will spin forever if we use anything that forces the whole seq.
  public static Mem InitialTape => 
    new( Next: toSeq(MemUtils.Zeros()),
         Prev: toSeq(MemUtils.Zeros()),
         Cur: 0);


  // Todo: what does .Tail do on an empty list?  Can we use this with empty starting lists for Prev and Next?
  public Mem MoveRight() =>
    new Mem( Prev: Cur.Cons(Prev), Cur: Next.Head.IfNone(0), Next: Next.Tail);

  public Mem MoveLeft() =>
    new Mem( Prev: Prev.Tail, Cur: Prev.Head.IfNone(0), Next: Cur.Cons(Next) );

  public Mem Set(byte i) =>
    this with { Cur = i };
}
