using LanguageExt.Parsec;
using Char = LanguageExt.Parsec.Char;
using static Instruction;

public static class Parser {

   public static Parser<Seq<Instruction>> ParseBrainfuck =>
     Prim.many(Prim.choice(ParseNonLoop, ParseLoop));

   public static Parser<Instruction>Instr(char c, Instruction i) => Char.ch(c).Select(_ => i);
   
   public static Parser<Instruction> ParseNonLoop =>
     Prim.choice(
      Instr('+', new Incr()), 
      Instr('-', new Decr()), 
      Instr('<', new Left()), 
      Instr('>', new Right()), 
      Instr('.', new Write()), 
      Instr(',', new Read())
     );
    

   public static Parser<Instruction> ParseLoop => 
     from _1 in Char.ch('[')
     from body in ParseBrainfuck
     from _2 in Char.ch(']')
     select (Instruction) new Loop(body);


}
