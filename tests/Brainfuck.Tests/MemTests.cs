using LanguageExt;
using static LanguageExt.Prelude;

namespace Brainfuck.Tests;

// Tests for the immutable tape `Mem`.
// Convention assumed (from the field names): Prev = cells to the LEFT (head = nearest left),
// Cur = the cell under the pointer, Next = cells to the RIGHT (head = nearest right).
//   `>` / MoveRight : pointer steps right -> new Cur = Next.Head, old Cur pushed onto Prev.
//   `<` / MoveLeft  : pointer steps left  -> new Cur = Prev.Head, old Cur pushed onto Next.
public class MemTests
{
    // A finite, fully-known tape so we never force the infinite InitialTape.
    //   left:  ...2 1   cur: 3   right: 4 5...
    static Mem Sample() => new(Prev: Seq<byte>(1, 2), Cur: 3, Next: Seq<byte>(4, 5));

    [Fact]
    public void MoveRight_brings_nearest_right_cell_under_the_pointer()
    {
        Assert.Equal(4, Sample().MoveRight().Cur);
    }

    [Fact]
    public void MoveRight_pushes_old_current_onto_the_left()
    {
        var m = Sample().MoveRight();
        Assert.Equal(Seq<byte>(3, 1, 2), m.Prev);
        Assert.Equal(Seq<byte>(5), m.Next);
    }

    [Fact]
    public void MoveLeft_brings_nearest_left_cell_under_the_pointer()
    {
        Assert.Equal(1, Sample().MoveLeft().Cur);
    }

    [Fact]
    public void MoveLeft_pushes_old_current_onto_the_right()
    {
        var m = Sample().MoveLeft();
        Assert.Equal(Seq<byte>(2), m.Prev);
        Assert.Equal(Seq<byte>(3, 4, 5), m.Next);
    }

    [Fact]
    public void Right_then_Left_returns_to_the_original_tape()
    {
        Assert.Equal(Sample(), Sample().MoveRight().MoveLeft());
    }

    [Fact]
    public void Left_then_Right_returns_to_the_original_tape()
    {
        Assert.Equal(Sample(), Sample().MoveLeft().MoveRight());
    }

    [Fact]
    public void Set_replaces_only_the_current_cell()
    {
        var m = Sample().Set(9);
        Assert.Equal(9, m.Cur);
        Assert.Equal(Seq<byte>(1, 2), m.Prev);
        Assert.Equal(Seq<byte>(4, 5), m.Next);
    }

    [Fact]
    public void Set_does_not_mutate_the_original()
    {
        var original = Sample();
        _ = original.Set(9);
        Assert.Equal(3, original.Cur);
    }

    [Fact]
    public void Records_with_the_same_contents_are_equal()
    {
        Assert.Equal(new Mem(Seq<byte>(1, 2), 3, Seq<byte>(4, 5)),
                     new Mem(Seq<byte>(1, 2), 3, Seq<byte>(4, 5)));
    }
}
