using LanguageExt;
using static LanguageExt.Prelude;

namespace Brainfuck.Tests;

// Tests for the blank starting tape (Tape.InitialTape). The tape is finite: it holds only the
// cells actually written, and unwritten cells on either side read as 0.
public class InitialTapeTests
{
    [Fact]
    public void Initial_current_cell_is_zero()
    {
        Assert.Equal(0, Tape.InitialTape.Cur);
    }

    [Fact]
    public void Initial_tape_is_empty_to_the_right()
    {
        Assert.True(Tape.InitialTape.Next.IsEmpty);
    }

    [Fact]
    public void Initial_tape_is_empty_to_the_left()
    {
        Assert.True(Tape.InitialTape.Prev.IsEmpty);
    }

    [Fact]
    public void Initial_tape_equals_a_blank_tape()
    {
        // Whole-tape equality is safe now that the tape is finite (no infinite zero-seq to force).
        Assert.Equal(new Tape(Seq<byte>(), 0, Seq<byte>()), Tape.InitialTape);
    }

    [Fact]
    public void Moving_right_into_unwritten_region_reads_zero()
    {
        Assert.Equal(0, Tape.InitialTape.MoveRight().Cur);
    }

    [Fact]
    public void Moving_left_into_unwritten_region_reads_zero()
    {
        Assert.Equal(0, Tape.InitialTape.MoveLeft().Cur);
    }

    [Fact]
    public void Write_then_move_away_and_back_preserves_the_value()
    {
        Assert.Equal(5, Tape.InitialTape.Set(5).MoveRight().MoveLeft().Cur);
    }

    [Fact]
    public void Consecutive_cells_hold_their_own_values()
    {
        // Write 1, 2, 3 into three adjacent cells moving rightward...
        var m = Tape.InitialTape.Set(1).MoveRight().Set(2).MoveRight().Set(3);

        // ...then read them back moving leftward.
        Assert.Equal(3, m.Cur);
        Assert.Equal(2, m.MoveLeft().Cur);
        Assert.Equal(1, m.MoveLeft().MoveLeft().Cur);
    }
}
