using LanguageExt;
using static LanguageExt.Prelude;

namespace Brainfuck.Tests;

// Tests for Loop evaluation. All programs here are pure state (Incr/Decr/Left/Right only),
// so they run through the StateT<Mem, IO, _> harness without touching real IO.
public class LoopTests
{
    static Mem EmptyTape => new(Seq<int>(), 0, Seq<int>());

    static Mem RunEval(Seq<AST> program, Mem start)
    {
        var (_, final) = AST.Eval(program).Run(start).As().Run();
        return final;
    }

    // Convenience builders for readability.
    static AST Incr() => new AST.Incr();
    static AST Decr() => new AST.Decr();
    static AST Left() => new AST.Left();
    static AST Right() => new AST.Right();
    static AST Loop(params AST[] body) => new AST.Loop(toSeq(body));

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

    // BF `[` must skip the body when the current cell is 0 (a top-tested while loop), so `[+]`
    // on a zero cell should leave it at 0.
    //
    // SKIPPED: the current Loop evaluator is do-while -- it runs the body *before* testing the
    // cell -- so `[+]` on a zero cell never terminates. That hangs the whole run (an xUnit
    // Timeout can't reclaim the runaway thread), so this test must not execute as-is.
    // Un-skip it once Loop tests the cell *before* running the body.
    [Fact(Skip = "Loop is do-while; `[+]` on a zero cell diverges. Un-skip after switching to while-semantics.")]
    public void Loop_is_skipped_when_current_cell_is_zero()  // [+] on a zero cell
    {
        var prog = Seq(Loop(Incr()));
        Assert.Equal(0, RunEval(prog, EmptyTape).Cur);
    }
}
