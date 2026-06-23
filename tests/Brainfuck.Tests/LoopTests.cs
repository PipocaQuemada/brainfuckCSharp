using LanguageExt;
using static LanguageExt.Prelude;

namespace Brainfuck.Tests;

// Tests for Loop evaluation. All programs here are pure state (Incr/Decr/Left/Right only),
// so they run through the StateT<Tape, IO, _> harness without touching real IO.
public class LoopTests
{
    static Tape EmptyTape => new(Seq<byte>(), 0, Seq<byte>());

    static Tape RunEval(Seq<Instruction> program, Tape start)
    {
        var (_, final) = Instruction.Eval(program).runApp.Run(start).As().Run();
        return final;
    }

    // Convenience builders for readability.
    static Instruction Incr() => new Instruction.Incr();
    static Instruction Decr() => new Instruction.Decr();
    static Instruction Left() => new Instruction.Left();
    static Instruction Right() => new Instruction.Right();
    static Instruction Loop(params Instruction[] body) => new Instruction.Loop(toSeq(body));

    [Fact]
    public void Loop_clears_a_positive_cell()             // +++[-]
    {
        var prog = Seq(Incr(), Incr(), Incr(), Loop(Decr()));
        Assert.Equal(0, RunEval(prog, EmptyTape).Cur);
    }

    [Fact]
    public void Loop_moves_a_value_to_the_next_cell()     // +++[->+<]
    {
        var prog = Seq(Incr(), Incr(), Incr(), Loop(Decr(), Right(), Incr(), Left()));
        var final = RunEval(prog, EmptyTape);
        Assert.Equal(0, final.Cur);                  // source cell drained
        Assert.Equal(3, final.MoveRight().Cur);      // value landed in the next cell
    }

    [Fact]
    public void Loop_multiplies_via_repeated_addition()  // ++[->+++<]  => 2 * 3 = 6
    {
        var prog = Seq(Incr(), Incr(), Loop(Decr(), Right(), Incr(), Incr(), Incr(), Left()));
        var final = RunEval(prog, EmptyTape);
        Assert.Equal(0, final.Cur);
        Assert.Equal(6, final.MoveRight().Cur);
    }

    [Fact]
    public void Loop_is_skipped_when_current_cell_is_zero()  // [+] on a zero cell
    {
        var prog = Seq(Loop(Incr()));
        Assert.Equal(0, RunEval(prog, EmptyTape).Cur);
    }
}
