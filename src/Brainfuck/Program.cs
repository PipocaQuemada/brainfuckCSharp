    static AST Incr() => new AST.Incr();
    static AST Decr() => new AST.Decr();
    static AST Write() => new AST.Write();
    static AST Left() => new AST.Left();
    static AST Right() => new AST.Right();
    static AST Loop(params AST[] body) => new AST.Loop(toSeq(body));

  var program = Seq(Write(), Incr(), Write(), Incr(), Write(), Incr(), Write(), Decr(), Write());
  AST.Eval(program).Run(Tape.InitialTape).As().Run();
