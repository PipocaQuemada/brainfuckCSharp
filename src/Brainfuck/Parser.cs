using LanguageExt.Parsec;
using Char = LanguageExt.Parsec.Char;
using static AST;

public static class Parser {

   public static Parser<Seq<AST>> ParseBrainfuck =>
     Prim.many(Prim.choice(ParseNonLoop, ParseLoop));

   public static Parser<AST>Instruction(char c, AST i) => Char.ch(c).Select(_ => i);
   
   public static Parser<AST> ParseNonLoop =>
     Prim.choice(
      Instruction('+', new Incr()), 
      Instruction('-', new Decr()), 
      Instruction('<', new Left()), 
      Instruction('>', new Right()), 
      Instruction('.', new Write()), 
      Instruction(',', new Read())
     );
    

   public static Parser<AST> ParseLoop => 
     from _1 in Char.ch('[')
     from body in ParseBrainfuck
     from _2 in Char.ch(']')
     select (AST) new Loop(body);


}
