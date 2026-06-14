using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;

namespace Brainfuck.Tests;

// Tests for the Parsec-based Parser: source text -> Seq<AST>.
public class ParserTests
{
    // Run a parser and assert it succeeded, returning the produced value.
    static T Parse<T>(Parser<T> parser, string input)
    {
        var result = Prim.parse(parser, input);
        Assert.False(result.IsFaulted, $"parse of \"{input}\" failed: {result}");
        return result.Reply.Result;
    }

    [Theory]
    [InlineData("+")]
    [InlineData("-")]
    [InlineData("<")]
    [InlineData(">")]
    [InlineData(".")]
    [InlineData(",")]
    public void Single_instructions_parse(string src)
    {
        AST expected = src switch
        {
            "+" => new AST.Incr(),
            "-" => new AST.Decr(),
            "<" => new AST.Left(),
            ">" => new AST.Right(),
            "." => new AST.Write(),
            "," => new AST.Read(),
            _   => throw new ArgumentOutOfRangeException(nameof(src)),
        };
        Assert.Equal(expected, Parse(Parser.ParseNonLoop, src));
    }

    [Fact]
    public void A_program_parses_instructions_in_order()
    {
        var expected = Seq<AST>(
            new AST.Incr(), new AST.Decr(), new AST.Right(),
            new AST.Left(), new AST.Write(), new AST.Read());
        Assert.Equal(expected, Parse(Parser.ParseBrainfuck, "+-><.,"));
    }

    [Fact]
    public void The_empty_program_parses_to_no_instructions()
    {
        Assert.Equal(Seq<AST>(), Parse(Parser.ParseBrainfuck, ""));
    }

    [Fact]
    public void A_loop_parses_to_a_Loop_node()
    {
        var expected = Seq<AST>(new AST.Loop(Seq<AST>(new AST.Incr(), new AST.Decr())));
        Assert.Equal(expected, Parse(Parser.ParseBrainfuck, "[+-]"));
    }

    [Fact]
    public void Nested_loops_parse_to_nested_Loop_nodes()
    {
        var inner    = new AST.Loop(Seq<AST>(new AST.Incr()));
        var expected = Seq<AST>(new AST.Loop(Seq<AST>((AST)inner)));
        Assert.Equal(expected, Parse(Parser.ParseBrainfuck, "[[+]]"));
    }

    [Fact]
    public void Instructions_around_a_loop_parse()        // +[-]>
    {
        var expected = Seq<AST>(
            new AST.Incr(),
            new AST.Loop(Seq<AST>(new AST.Decr())),
            new AST.Right());
        Assert.Equal(expected, Parse(Parser.ParseBrainfuck, "+[-]>"));
    }
}
