using LanguageExt;
using static LanguageExt.Prelude;

namespace Brainfuck.Tests;

// Tests for the *infinite* tape (Mem.InitialTape), whose Prev/Next are lazy, never-ending
// zero-seqs. These exercise the configuration the interpreter actually runs on.
//
// SAFETY RULES for everything in this file:
//   * Only ever inspect `.Cur` (a scalar) or a finite prefix via `.Take(n)`.
//   * NEVER compare a whole `Mem` or a whole `Seq` for equality against an InitialTape-derived
//     value -- record/Seq equality would force the infinite seq and hang the runner.
//   * Because of the two rules above, every test here terminates by construction even if the
//     tape is genuinely infinite -- no timeout needed.
public class InfiniteTapeTests
{
    [Fact]
    public void Initial_current_cell_is_zero()
    {
        Assert.Equal(0, Mem.InitialTape.Cur);
    }

    [Fact]
    public void Initial_tape_is_zeros_to_the_right()
    {
        Assert.Equal(Seq<byte>(0, 0, 0, 0, 0), Mem.InitialTape.Next.Take(5));
    }

    [Fact]
    public void Initial_tape_is_zeros_to_the_left()
    {
        Assert.Equal(Seq<byte>(0, 0, 0, 0, 0), Mem.InitialTape.Prev.Take(5));
    }

    [Fact]
    public void Moving_right_into_unwritten_region_reads_zero()
    {
        Assert.Equal(0, Mem.InitialTape.MoveRight().Cur);
    }

    [Fact]
    public void Moving_left_into_unwritten_region_reads_zero()
    {
        Assert.Equal(0, Mem.InitialTape.MoveLeft().Cur);
    }

    [Fact]
    public void Write_then_move_away_and_back_preserves_the_value()
    {
        Assert.Equal(5, Mem.InitialTape.Set(5).MoveRight().MoveLeft().Cur);
    }

    [Fact]
    public void Consecutive_cells_hold_their_own_values()
    {
        // Write 1, 2, 3 into three adjacent cells moving rightward...
        var m = Mem.InitialTape.Set(1).MoveRight().Set(2).MoveRight().Set(3);

        // ...then read them back moving leftward.
        Assert.Equal(3, m.Cur);
        Assert.Equal(2, m.MoveLeft().Cur);
        Assert.Equal(1, m.MoveLeft().MoveLeft().Cur);
    }
}
