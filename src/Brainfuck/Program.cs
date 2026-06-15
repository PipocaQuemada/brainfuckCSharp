    static Instruction Incr() => new Instruction.Incr();
    static Instruction Decr() => new Instruction.Decr();
    static Instruction Write() => new Instruction.Write();
    static Instruction Left() => new Instruction.Left();
    static Instruction Right() => new Instruction.Right();
    static Instruction Loop(params Instruction[] body) => new Instruction.Loop(toSeq(body));

  var program = Seq(Write(), Incr(), Write(), Incr(), Write(), Incr(), Write(), Decr(), Write());
  Instruction.Eval(program).Run(Tape.InitialTape).As().Run();
