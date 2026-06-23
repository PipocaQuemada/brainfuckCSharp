using LanguageExt;
using static LanguageExt.Prelude;

namespace Brainfuck.Tests;

// Tests for the evaluator (Instruction.Step / Instruction.Eval) over a StateT<Tape, IO, Unit>.
// Covers only the pure, non-blocking instructions (Incr/Decr and sequences of them).
// Read would block on stdin and Write/Loop touch real IO, so they are out of scope here.
public class EvalTests
{
    static Tape EmptyTape => new(Seq<byte>(), 0, Seq<byte>());

    // Run a single instruction against a starting tape and return the resulting tape.
    static Tape RunStep(Instruction instr, Tape start)
    {
        var (_, final) = Instruction.Step(instr).runApp.Run(start).As().Run();
        return final;
    }

    // Run a program (sequence of instructions) against a starting tape.
    static Tape RunEval(Seq<Instruction> program, Tape start)
    {
        var (_, final) = Instruction.Eval(program).runApp.Run(start).As().Run();
        return final;
    }

    [Fact]
    public void Incr_increments_the_current_cell()
    {
        Assert.Equal(1, RunStep(new Instruction.Incr(), EmptyTape).Cur);
    }

    [Fact]
    public void Decr_decrements_the_current_cell()
    {
        Assert.Equal(4, RunStep(new Instruction.Decr(), EmptyTape with { Cur = 5 }).Cur);
    }

    [Fact]
    public void Eval_runs_instructions_in_order()
    {
        var program = Seq<Instruction>(new Instruction.Incr(), new Instruction.Incr(), new Instruction.Incr());
        Assert.Equal(3, RunEval(program, EmptyTape).Cur);
    }

    [Fact]
    public void Eval_mixes_increments_and_decrements()
    {
        var program = Seq<Instruction>(new Instruction.Incr(), new Instruction.Incr(), new Instruction.Decr());
        Assert.Equal(1, RunEval(program, EmptyTape).Cur);
    }

    [Fact]
    public void Eval_of_the_empty_program_leaves_the_tape_unchanged()
    {
        Assert.Equal(EmptyTape, RunEval(Seq<Instruction>(), EmptyTape));
    }
}
