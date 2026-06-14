using LanguageExt;
using static LanguageExt.Prelude;

namespace Brainfuck.Tests;

// Tests for the evaluator (AST.Step / AST.Eval) over a StateT<Mem, IO, Unit>.
// Covers only the pure, non-blocking instructions (Incr/Decr and sequences of them).
// Read would block on stdin and Write/Loop touch real IO, so they are out of scope here.
public class EvalTests
{
    static Mem EmptyTape => new(Seq<int>(), 0, Seq<int>());

    // Run a single instruction against a starting tape and return the resulting tape.
    static Mem RunStep(AST instr, Mem start)
    {
        var (_, final) = AST.Step(instr).Run(start).As().Run();
        return final;
    }

    // Run a program (sequence of instructions) against a starting tape.
    static Mem RunEval(Seq<AST> program, Mem start)
    {
        var (_, final) = AST.Eval(program).Run(start).As().Run();
        return final;
    }

    [Fact]
    public void Incr_increments_the_current_cell()
    {
        Assert.Equal(1, RunStep(new AST.Incr(), EmptyTape).Cur);
    }

    [Fact]
    public void Decr_decrements_the_current_cell()
    {
        Assert.Equal(4, RunStep(new AST.Decr(), EmptyTape with { Cur = 5 }).Cur);
    }

    [Fact]
    public void Eval_runs_instructions_in_order()
    {
        var program = Seq<AST>(new AST.Incr(), new AST.Incr(), new AST.Incr());
        Assert.Equal(3, RunEval(program, EmptyTape).Cur);
    }

    [Fact]
    public void Eval_mixes_increments_and_decrements()
    {
        var program = Seq<AST>(new AST.Incr(), new AST.Incr(), new AST.Decr());
        Assert.Equal(1, RunEval(program, EmptyTape).Cur);
    }

    [Fact]
    public void Eval_of_the_empty_program_leaves_the_tape_unchanged()
    {
        Assert.Equal(EmptyTape, RunEval(Seq<AST>(), EmptyTape));
    }
}
