namespace Brainfuck;



public static class AppExtensions {
  extension<A>(K<App, A> ma) {
    public App<A> As() => (App<A>)ma;
  }
}

public partial class App :
    Deriving.Monad<App, StateT<Tape, IO>>,
  Deriving.Stateful<App, StateT<Tape, IO>, Tape>,
    MonadIO<App>
{
    public static K<StateT<Tape, IO>, A> Transform<A>(K<App, A> fa) =>
        fa.As().runApp;

    public static K<App, A> CoTransform<A>(K<StateT<Tape, IO>, A> fa) =>
        new App<A>(fa.As());

    public static K<App, A> LiftIO<A>(IO<A> ma) =>
      new App<A>(StateT.liftIO<Tape, IO, A>(ma));

    public static K<App, A> liftIO<A>(IO<A> ma) =>
      new App<A>(StateT.liftIO<Tape, IO, A>(ma));

    public static App<A> gets<A>(Func<Tape, A> f) => new (StateT.gets<IO, Tape, A>(f));
    public static App<Unit> modify(Func<Tape, Tape> f) => new (StateT.modify<IO, Tape>(f));
}


public record App<A>(StateT<Tape, IO, A> runApp): K<App, A> {
}
